using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;
using Azure.Messaging.ServiceBus;
using System.Diagnostics;
using System.Text.Json;

namespace OrderService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly IProductServiceClient _productService;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderMetricsService _metricsService;

    public OrderController(
        OrderDbContext context,
        IProductServiceClient productService,
        ServiceBusClient serviceBusClient,
        ILogger<OrderController> logger,
        IOrderMetricsService metricsService)
    {
        _context = context;
        _productService = productService;
        _serviceBusClient = serviceBusClient;
        _logger = logger;
        _metricsService = metricsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var userId = GetUserId();
            var query = _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            var totalItems = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(new
            {
                TotalItems = totalItems,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            }));

            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders");
            _metricsService.RecordOrderError("get_orders");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _metricsService.RecordOrderProcessingTime(stopwatch.Elapsed.TotalSeconds);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(Guid id)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var userId = GetUserId();
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", id);
            _metricsService.RecordOrderError("get_order");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _metricsService.RecordOrderProcessingTime(stopwatch.Elapsed.TotalSeconds);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(Order order)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var userId = GetUserId();
            order.UserId = userId;
            order.CreatedAt = DateTime.UtcNow;
            order.Status = "Pending";
            order.PaymentStatus = "Pending";

            // Validate and calculate order total
            decimal totalAmount = 0;
            foreach (var item in order.Items)
            {
                var isAvailable = await _productService.CheckProductAvailabilityAsync(
                    item.ProductId, item.Quantity);

                if (!isAvailable)
                {
                    _metricsService.RecordOrderError("product_unavailable");
                    return BadRequest($"Product {item.ProductId} is not available in the requested quantity");
                }

                var price = await _productService.GetProductPriceAsync(item.ProductId);
                item.UnitPrice = price;
                item.TotalPrice = price * item.Quantity;
                totalAmount += item.TotalPrice;
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Send order created event to Service Bus
            await SendOrderCreatedEvent(order);

            _metricsService.RecordOrderCreated(order.Status);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            _metricsService.RecordOrderError("create_order");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _metricsService.RecordOrderProcessingTime(stopwatch.Elapsed.TotalSeconds);
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] string status)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var oldStatus = order.Status;
            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send order status updated event to Service Bus
            await SendOrderStatusUpdatedEvent(order);

            _metricsService.RecordOrderStatusChange(oldStatus, status);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status for order {OrderId}", id);
            _metricsService.RecordOrderError("update_status");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _metricsService.RecordOrderProcessingTime(stopwatch.Elapsed.TotalSeconds);
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID");
        }
        return userId;
    }

    private async Task SendOrderCreatedEvent(Order order)
    {
        var sender = _serviceBusClient.CreateSender("order-created");
        var message = new ServiceBusMessage(JsonSerializer.Serialize(new
        {
            OrderId = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(i => new
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            })
        }));
        await sender.SendMessageAsync(message);
    }

    private async Task SendOrderStatusUpdatedEvent(Order order)
    {
        var sender = _serviceBusClient.CreateSender("order-status-updated");
        var message = new ServiceBusMessage(JsonSerializer.Serialize(new
        {
            OrderId = order.Id,
            Status = order.Status,
            UpdatedAt = order.UpdatedAt
        }));
        await sender.SendMessageAsync(message);
    }
} 