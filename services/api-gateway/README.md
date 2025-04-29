# API Gateway Service

## Author
**Dr. Libin Pallikunnel Kurian**  
[LinkedIn Profile](https://www.linkedin.com/in/dr-libin-pallikunnel-kurian-88741530/)

## Overview
The API Gateway serves as the central entry point for all client requests in the e-commerce platform. It handles routing, authentication, and various cross-cutting concerns.

## Configuration
- **Port**: 5007
- **Base URL**: http://localhost:5007
- **Swagger UI**: http://localhost:5007/swagger

## Authentication
The API Gateway uses Azure AD B2C for authentication:
- JWT token validation
- Role-based access control (RBAC)
- Scopes:
  - products.read
  - products.write
  - orders.read
  - orders.write
  - payments.process
  - users.manage
  - inventory.manage

## Service Routes
| Service | Port | Path | Methods | Scope |
|---------|------|------|---------|-------|
| Products | 5001 | /products | GET, POST, PUT, DELETE | products.read, products.write |
| Orders | 5003 | /orders | GET, POST, PUT | orders.read, orders.write |
| Payments | 5004 | /payments | POST | payments.process |
| Notifications | 5006 | /notifications | GET, POST | notifications.read, notifications.write |
| Users | 5002 | /users | GET, POST, PUT, DELETE | users.manage |
| Inventory | 5005 | /inventory | GET, PUT | inventory.manage |
| Frontend | 5008 | / | GET | - |

## Rate Limiting
- Client ID header: X-Client-Id
- Rate limit message: "Rate limit exceeded. Please try again later."
- Default limits:
  - 100 requests per minute for authenticated users
  - 20 requests per minute for anonymous users

## Quality of Service (QoS)
- Exceptions allowed before breaking: 5
- Duration of break: 30 seconds
- Timeout value: 30 seconds
- Cache duration: 5 minutes

## Dependencies
- Ocelot 20.0.0
- Microsoft.AspNetCore.Authentication.JwtBearer 7.0.0
- Polly 7.2.3
- Prometheus 7.0.0
- HealthChecks.UI 7.0.0

## Setup
1. Configure Azure AD B2C:
   ```json
   {
     "AzureAdB2C": {
       "Instance": "https://your-tenant.b2clogin.com",
       "ClientId": "your-client-id",
       "Domain": "your-tenant.onmicrosoft.com",
       "SignUpSignInPolicyId": "B2C_1_SignUpSignIn"
     }
   }
   ```

2. Start the service:
   ```powershell
   dotnet run --project services/api-gateway/ApiGateway.csproj
   ```

3. Access Swagger UI:
   - Open http://localhost:5007/swagger
   - Authorize using your Azure AD B2C credentials

## Monitoring
- Prometheus metrics: /metrics
- Health checks: /health
- Circuit breaker status: /circuit-breaker
- Request logs: /logs

## Troubleshooting
1. **Authentication Issues**
   - Verify Azure AD B2C configuration
   - Check token scopes
   - Validate JWT tokens

2. **Routing Issues**
   - Check service availability
   - Verify route configurations
   - Monitor circuit breaker status

3. **Rate Limiting**
   - Check client ID header
   - Monitor request counts
   - Adjust rate limits if needed

4. **QoS Issues**
   - Check service health
   - Monitor timeout values
   - Adjust circuit breaker settings 