using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;
using Prometheus;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly NotificationDbContext _context;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<NotificationController> _logger;
    private static readonly Counter NotificationRequests = Metrics
        .CreateCounter("notification_service_requests_total", "Total notification requests");
    private static readonly Counter NotificationErrors = Metrics
        .CreateCounter("notification_service_errors_total", "Total notification errors");

    public NotificationController(
        NotificationDbContext context,
        ServiceBusClient serviceBusClient,
        ILogger<NotificationController> logger)
    {
        _context = context;
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
    {
        NotificationRequests.Inc();
        try
        {
            return await _context.Notifications.ToListAsync();
        }
        catch (Exception ex)
        {
            NotificationErrors.Inc();
            _logger.LogError(ex, "Error getting notifications");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Notification>> GetNotification(Guid id)
    {
        NotificationRequests.Inc();
        try
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return notification;
        }
        catch (Exception ex)
        {
            NotificationErrors.Inc();
            _logger.LogError(ex, "Error getting notification {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Notification>> CreateNotification(Notification notification)
    {
        NotificationRequests.Inc();
        try
        {
            notification.Id = Guid.NewGuid();
            notification.CreatedAt = DateTime.UtcNow;
            notification.Status = "Pending";

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send notification created event to Service Bus
            var sender = _serviceBusClient.CreateSender("notification-created");
            await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(notification)));

            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }
        catch (Exception ex)
        {
            NotificationErrors.Inc();
            _logger.LogError(ex, "Error creating notification");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNotification(Guid id, Notification notification)
    {
        NotificationRequests.Inc();
        try
        {
            if (id != notification.Id)
            {
                return BadRequest();
            }

            _context.Entry(notification).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            NotificationErrors.Inc();
            _logger.LogError(ex, "Error updating notification {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
} 