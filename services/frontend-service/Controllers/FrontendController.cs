using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace FrontendService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FrontendController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FrontendController> _logger;
    private static readonly Counter FrontendRequests = Metrics
        .CreateCounter("frontend_service_requests_total", "Total frontend requests");
    private static readonly Counter FrontendErrors = Metrics
        .CreateCounter("frontend_service_errors_total", "Total frontend errors");

    public FrontendController(
        IHttpClientFactory httpClientFactory,
        ILogger<FrontendController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet("catalog")]
    public async Task<IActionResult> GetCatalog()
    {
        FrontendRequests.Inc();
        try
        {
            var client = _httpClientFactory.CreateClient("ApiGateway");
            var response = await client.GetAsync("/api/apigateway/products");
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            FrontendErrors.Inc();
            _logger.LogError(ex, "Error getting catalog");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetUserOrders()
    {
        FrontendRequests.Inc();
        try
        {
            var client = _httpClientFactory.CreateClient("ApiGateway");
            var response = await client.GetAsync("/api/apigateway/orders");
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            FrontendErrors.Inc();
            _logger.LogError(ex, "Error getting user orders");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetUserNotifications()
    {
        FrontendRequests.Inc();
        try
        {
            var client = _httpClientFactory.CreateClient("ApiGateway");
            var response = await client.GetAsync("/api/apigateway/notifications");
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            FrontendErrors.Inc();
            _logger.LogError(ex, "Error getting user notifications");
            return StatusCode(500, "Internal server error");
        }
    }

    private async Task<IActionResult> HandleResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
    }
} 