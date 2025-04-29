using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;
using Prometheus;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly PaymentDbContext _context;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<PaymentController> _logger;
    private static readonly Counter PaymentRequests = Metrics
        .CreateCounter("payment_service_requests_total", "Total payment requests");
    private static readonly Counter PaymentErrors = Metrics
        .CreateCounter("payment_service_errors_total", "Total payment errors");

    public PaymentController(
        PaymentDbContext context,
        ServiceBusClient serviceBusClient,
        ILogger<PaymentController> logger)
    {
        _context = context;
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
    {
        PaymentRequests.Inc();
        try
        {
            return await _context.Payments.ToListAsync();
        }
        catch (Exception ex)
        {
            PaymentErrors.Inc();
            _logger.LogError(ex, "Error getting payments");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetPayment(Guid id)
    {
        PaymentRequests.Inc();
        try
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return payment;
        }
        catch (Exception ex)
        {
            PaymentErrors.Inc();
            _logger.LogError(ex, "Error getting payment {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Payment>> CreatePayment(Payment payment)
    {
        PaymentRequests.Inc();
        try
        {
            payment.Id = Guid.NewGuid();
            payment.CreatedAt = DateTime.UtcNow;
            payment.Status = "Pending";

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Send payment created event to Service Bus
            var sender = _serviceBusClient.CreateSender("payment-created");
            await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(payment)));

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }
        catch (Exception ex)
        {
            PaymentErrors.Inc();
            _logger.LogError(ex, "Error creating payment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePayment(Guid id, Payment payment)
    {
        PaymentRequests.Inc();
        try
        {
            if (id != payment.Id)
            {
                return BadRequest();
            }

            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            PaymentErrors.Inc();
            _logger.LogError(ex, "Error updating payment {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
} 