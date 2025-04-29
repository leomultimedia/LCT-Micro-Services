using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Common.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add service ports configuration
builder.Services.AddServicePorts();
var servicePorts = builder.Services.BuildServiceProvider().GetRequiredService<ServicePorts>();

// Configure Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(servicePorts.GetServicePort("Inventory"));
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("inventory_health_check", () => HealthCheckResult.Healthy());

// Add Authentication
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["AzureAdB2C:Instance"];
        options.Audience = builder.Configuration["AzureAdB2C:ClientId"];
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Add Prometheus metrics
app.UseMetricServer();
app.UseHttpMetrics();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

app.Run(); 