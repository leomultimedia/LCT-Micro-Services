# Lear Cyber Tech E-Commerce Microservices Platform

## Author
**Dr. Libin Pallikunnel Kurian**  
[LinkedIn Profile](https://www.linkedin.com/in/dr-libin-pallikunnel-kurian-88741530/)

## Architecture Overview
The platform consists of the following microservices:

1. **Sample Data Service** (Port: 5000)
   - Provides initial sample data for development and testing
   - Manages seed data for products, users, and orders
   - Supports data reset functionality

2. **API Gateway** (Port: 5007)
   - Central entry point for all requests
   - Handles authentication and authorization using Azure AD B2C
   - Implements rate limiting and QoS
   - Routes requests to appropriate services

3. **Product Service** (Port: 5001)
   - Manages product catalog
   - Handles product CRUD operations
   - Integrates with Azure Blob Storage for product images
   - Implements health checks for database connectivity

4. **Order Service** (Port: 5003)
   - Processes orders
   - Manages order lifecycle
   - Integrates with Product Service and Payment Service
   - Handles order validation and status updates

5. **Payment Service** (Port: 5004)
   - Handles payment processing
   - Integrates with payment gateways
   - Manages transaction records
   - Implements secure payment handling

6. **Notification Service** (Port: 5006)
   - Sends notifications via multiple channels
   - Uses Azure Service Bus for message queuing
   - Supports email, SMS, and push notifications
   - Implements retry policies for reliability

7. **User Service** (Port: 5002)
   - Manages user accounts and profiles
   - Handles authentication and authorization
   - Integrates with Azure AD B2C
   - Manages user roles and permissions

8. **Inventory Service** (Port: 5005)
   - Tracks inventory levels
   - Manages stock updates
   - Integrates with Product Service
   - Implements stock reservation system

9. **Frontend Service** (Port: 5008)
   - Serves the web application
   - Provides modern React-based user interface
   - Communicates with API Gateway
   - Implements responsive design

## Prerequisites
- .NET 7.0 SDK
- PowerShell 7.0+
- Azure AD B2C tenant
- Azure Storage Account
- Azure Service Bus
- SQL Server
- Docker (optional, for containerized deployment)

## Setup Instructions

1. **Clone the Repository**
   ```powershell
   git clone https://github.com/your-username/ecommerce-microservices.git
   cd ecommerce-microservices
   ```

2. **Configure Azure AD B2C**
   - Create an Azure AD B2C tenant
   - Register applications for each service
   - Configure app registrations with appropriate scopes:
     - products.read
     - products.write
     - orders.read
     - orders.write
     - payments.process
     - users.manage
     - inventory.manage
   - Update appsettings.json with your tenant details

3. **Configure Azure Resources**
   - Create Azure Storage Account for product images
   - Create Azure Service Bus for notifications
   - Create Azure Key Vault for secrets management
   - Update connection strings in appsettings.json

4. **Start Services**
   ```powershell
   ./start-services.ps1
   ```

   Or start services individually:
   ```powershell
   # Start Sample Data Service first
   dotnet run --project services/sample-data-service/SampleDataService.csproj

   # Then start other services
   dotnet run --project services/api-gateway/ApiGateway.csproj
   dotnet run --project services/product-service/ProductService.csproj
   dotnet run --project services/order-service/OrderService.csproj
   dotnet run --project services/payment-service/PaymentService.csproj
   dotnet run --project services/notification-service/NotificationService.csproj
   dotnet run --project services/user-service/UserService.csproj
   dotnet run --project services/inventory-service/InventoryService.csproj
   dotnet run --project services/frontend-service/FrontendService.csproj
   ```

## Development

### Environment Setup
1. Install required tools:
   ```powershell
   dotnet tool install -g dotnet-ef
   dotnet tool install -g Microsoft.Web.LibraryManager.Cli
   ```

2. Set up development environment:
   ```powershell
   $env:ASPNETCORE_ENVIRONMENT = "Development"
   ```

### Running Tests
```powershell
dotnet test
```

### Building
```powershell
dotnet build
```

## Monitoring and Observability
- Prometheus metrics available at each service's /metrics endpoint
- Grafana dashboards for visualization
- Health checks available at each service's /health endpoint
- Azure Application Insights integration for production monitoring
- Distributed tracing with OpenTelemetry

## Security
- Azure AD B2C for authentication
- JWT Bearer tokens for authorization
- Role-based access control (RBAC)
- Rate limiting per client
- Circuit breaker pattern for fault tolerance
- Secure secret management with Azure Key Vault
- HTTPS enforcement
- CORS policy configuration

## Deployment
1. **Development**
   - Local SQL Server
   - Local Azure Storage Emulator
   - Local Azure Service Bus Emulator
   - Docker containers for services

2. **Production**
   - Azure SQL Database
   - Azure Storage Account
   - Azure Service Bus
   - Azure Key Vault
   - Azure App Service or Kubernetes
   - Azure Front Door for global distribution

## Troubleshooting
1. **Port Conflicts**
   - Ensure no other services are using ports 5000-5008
   - Check firewall settings
   - Verify service configurations

2. **Authentication Issues**
   - Verify Azure AD B2C configuration
   - Check token scopes
   - Validate JWT tokens
   - Check Azure AD B2C logs

3. **Database Issues**
   - Verify connection strings
   - Check database permissions
   - Run migrations if needed
   - Monitor database performance

4. **Service Communication**
   - Check service health endpoints
   - Verify network connectivity
   - Monitor logs for errors
   - Check circuit breaker status

5. **Performance Issues**
   - Monitor rate limiting
   - Check service response times
   - Verify cache configurations
   - Monitor database queries

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License
MIT License 