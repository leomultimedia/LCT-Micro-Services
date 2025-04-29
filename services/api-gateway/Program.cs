using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Prometheus;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration)
    .AddDelegatingHandler<RequestIdDelegatingHandler>();

// Add authentication
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAdB2C")
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductRead", policy => policy.RequireClaim("scope", "product.read"));
    options.AddPolicy("ProductWrite", policy => policy.RequireClaim("scope", "product.write"));
    options.AddPolicy("OrderRead", policy => policy.RequireClaim("scope", "order.read"));
    options.AddPolicy("OrderWrite", policy => policy.RequireClaim("scope", "order.write"));
    options.AddPolicy("PaymentRead", policy => policy.RequireClaim("scope", "payment.read"));
    options.AddPolicy("PaymentWrite", policy => policy.RequireClaim("scope", "payment.write"));
    options.AddPolicy("NotificationRead", policy => policy.RequireClaim("scope", "notification.read"));
    options.AddPolicy("NotificationWrite", policy => policy.RequireClaim("scope", "notification.write"));
    options.AddPolicy("UserRead", policy => policy.RequireClaim("scope", "user.read"));
    options.AddPolicy("UserWrite", policy => policy.RequireClaim("scope", "user.write"));
    options.AddPolicy("InventoryRead", policy => policy.RequireClaim("scope", "inventory.read"));
    options.AddPolicy("InventoryWrite", policy => policy.RequireClaim("scope", "inventory.write"));
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add CORS middleware
app.UseCors("AllowAll");

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Add Prometheus metrics
app.UseMetricServer();
app.UseHttpMetrics();

// Add health check endpoint
app.MapHealthChecks("/health");

// Add Ocelot middleware
await app.UseOcelot();

app.MapControllers();

app.Run();

// Request ID delegating handler
public class RequestIdDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());
        return await base.SendAsync(request, cancellationToken);
    }
} 