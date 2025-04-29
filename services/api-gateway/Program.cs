using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Prometheus;
using System.Text.Json;
using Common.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add service ports configuration
builder.Services.AddServicePorts();
var servicePorts = builder.Services.BuildServiceProvider().GetRequiredService<ServicePorts>();

// Configure Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(servicePorts.GetServicePort("ApiGateway"));
});

// Add configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllers();

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"), jwtBearerScheme: "AzureAdB2C");

// Configure Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("product.read", policy => policy.RequireClaim("scope", "product.read"));
    options.AddPolicy("product.write", policy => policy.RequireClaim("scope", "product.write"));
    options.AddPolicy("order.read", policy => policy.RequireClaim("scope", "order.read"));
    options.AddPolicy("order.write", policy => policy.RequireClaim("scope", "order.write"));
    options.AddPolicy("payment.read", policy => policy.RequireClaim("scope", "payment.read"));
    options.AddPolicy("payment.write", policy => policy.RequireClaim("scope", "payment.write"));
    options.AddPolicy("notification.read", policy => policy.RequireClaim("scope", "notification.read"));
    options.AddPolicy("notification.write", policy => policy.RequireClaim("scope", "notification.write"));
    options.AddPolicy("user.read", policy => policy.RequireClaim("scope", "user.read"));
    options.AddPolicy("user.write", policy => policy.RequireClaim("scope", "user.write"));
    options.AddPolicy("inventory.read", policy => policy.RequireClaim("scope", "inventory.read"));
    options.AddPolicy("inventory.write", policy => policy.RequireClaim("scope", "inventory.write"));
});

// Configure Ocelot with Polly
builder.Services.AddOcelot()
    .AddPolly()
    .AddDelegatingHandler<RequestIdDelegatingHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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