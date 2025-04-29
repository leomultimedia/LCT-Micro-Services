using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApiGatewayController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiGatewayController> _logger;
    private static readonly Counter RequestsTotal = Metrics
        .CreateCounter("api_gateway_requests_total", "Total number of requests through the API Gateway");
    private static readonly Counter ErrorsTotal = Metrics
        .CreateCounter("api_gateway_errors_total", "Total number of errors in the API Gateway");

    public ApiGatewayController(
        IHttpClientFactory httpClientFactory,
        ILogger<ApiGatewayController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        RequestsTotal.Inc();
        try
        {
            var client = _httpClientFactory.CreateClient("ProductService");
            var response = await client.GetAsync("/api/product");
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            ErrorsTotal.Inc();
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders()
    {
        RequestsTotal.Inc();
        try
        {
            var client = _httpClientFactory.CreateClient("OrderService");
            var response = await client.GetAsync("/api/order");
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            ErrorsTotal.Inc();
            _logger.LogError(ex, "Error getting orders");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments()
    {
        RequestsTotal.Inc();
        try
        {
            var client = _httpClientFactory.CreateClient("PaymentService");
            var response = await client.GetAsync("/api/payment");
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            ErrorsTotal.Inc();
            _logger.LogError(ex, "Error getting payments");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications()
    {
        RequestsTotal.Inc();
        try
        {
            var client = _httpClientFactory.CreateClient("NotificationService");
            var response = await client.GetAsync("/api/notification");
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            ErrorsTotal.Inc();
            _logger.LogError(ex, "Error getting notifications");
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