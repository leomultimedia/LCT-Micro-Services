using System.Text.Json;

namespace OrderService.Services;

public interface IProductServiceClient
{
    Task<bool> CheckProductAvailabilityAsync(Guid productId, int quantity);
    Task<decimal> GetProductPriceAsync(Guid productId);
}

public class ProductServiceClient : IProductServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductServiceClient> _logger;

    public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CheckProductAvailabilityAsync(Guid productId, int quantity)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/products/{productId}/availability?quantity={quantity}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking product availability for product {ProductId}", productId);
            return false;
        }
    }

    public async Task<decimal> GetProductPriceAsync(Guid productId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/products/{productId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(content);
            return product?.Price ?? throw new InvalidOperationException("Product price not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product price for product {ProductId}", productId);
            throw;
        }
    }
}

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
} 