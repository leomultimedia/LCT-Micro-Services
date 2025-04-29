using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductService.Data;
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
    options.ListenAnyIP(servicePorts.GetServicePort("Products"));
});

// Add services to the container
builder.Services.AddControllers();

// Configure Azure AD B2C authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAdB2C", options);
    },
    options => 
    {
        builder.Configuration.Bind("AzureAdB2C", options);
    });

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Service API", Version = "v1" });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["AzureAdB2C:AuthorizationUrl"]),
                TokenUrl = new Uri(builder.Configuration["AzureAdB2C:TokenUrl"]),
                Scopes = new Dictionary<string, string>
                {
                    { "product.read", "Read products" },
                    { "product.write", "Write products" }
                }
            }
        }
    });
});

// Add SQL Server DbContext
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Azure Blob Storage
builder.Services.AddSingleton(x => new BlobServiceClient(
    builder.Configuration.GetConnectionString("AzureStorage")));

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("product_health_check", () => HealthCheckResult.Healthy());

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API V1");
        c.OAuthClientId(builder.Configuration["AzureAdB2C:ClientId"]);
    });
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

builder.WebHost.UseUrls("http://*:5011");

app.Run(); 