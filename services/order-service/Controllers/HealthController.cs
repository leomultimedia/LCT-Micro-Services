using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using Azure.Messaging.ServiceBus;

namespace OrderService.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<HealthController> _logger;
    private readonly IConfiguration _configuration;

    public HealthController(
        OrderDbContext context,
        ServiceBusClient serviceBusClient,
        ILogger<HealthController> logger,
        IConfiguration configuration)
    {
        _context = context;
        _serviceBusClient = serviceBusClient;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Healthy");
    }

    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        var healthInfo = new Dictionary<string, object>
        {
            ["Status"] = "Healthy",
            ["Timestamp"] = DateTime.UtcNow,
            ["Version"] = GetType().Assembly.GetName().Version?.ToString() ?? "unknown"
        };

        try
        {
            // Check database connectivity
            var canConnect = await _context.Database.CanConnectAsync();
            if (!canConnect)
            {
                _logger.LogError("Database connection check failed");
                healthInfo["Status"] = "Unhealthy";
                healthInfo["Database"] = "Unavailable";
                return StatusCode(503, healthInfo);
            }

            // Check if we can execute a simple query
            var ordersCount = await _context.Orders.CountAsync();
            healthInfo["Database"] = "Available";
            healthInfo["OrdersCount"] = ordersCount;

            // Check Service Bus connectivity
            try
            {
                var sender = _serviceBusClient.CreateSender("health-check");
                await sender.CloseAsync();
                healthInfo["ServiceBus"] = "Available";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Bus connection check failed");
                healthInfo["Status"] = "Unhealthy";
                healthInfo["ServiceBus"] = "Unavailable";
                return StatusCode(503, healthInfo);
            }

            // Check Product Service connectivity if configured
            var productServiceUrl = _configuration["Services:ProductService"];
            if (!string.IsNullOrEmpty(productServiceUrl))
            {
                healthInfo["ProductService"] = "Configured";
            }

            return Ok(healthInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            healthInfo["Status"] = "Unhealthy";
            healthInfo["Error"] = ex.Message;
            return StatusCode(503, healthInfo);
        }
    }

    [HttpGet("startup")]
    public async Task<IActionResult> GetStartup()
    {
        try
        {
            // Check database connectivity
            var canConnect = await _context.Database.CanConnectAsync();
            if (!canConnect)
            {
                _logger.LogError("Startup check failed: Database connection check failed");
                return StatusCode(503, new { Status = "Unhealthy", Database = "Unavailable" });
            }

            // Check Service Bus connectivity
            var sender = _serviceBusClient.CreateSender("health-check");
            await sender.CloseAsync();

            return Ok(new { Status = "Healthy", Message = "Service is ready to accept traffic" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Startup check failed");
            return StatusCode(503, new { Status = "Unhealthy", Error = ex.Message });
        }
    }
} 