# User Service

The User Service handles user authentication, profile management, and role-based access control in the e-commerce microservices platform. It integrates with Azure AD B2C for identity management.

## Configuration

### Port Configuration
- Service runs on port 5002
- Base URL: http://localhost:5002

### Authentication
- Azure AD B2C Integration
- JWT Token Generation and Validation
- OAuth 2.0 / OpenID Connect
- Role-based Access Control (RBAC)

### User Roles
1. **Admin**
   - Full system access
   - Can manage all resources
   - Can manage other users

2. **Customer**
   - Can view and purchase products
   - Can manage own orders
   - Can view own profile

3. **Vendor**
   - Can manage products
   - Can view inventory
   - Can manage own profile

### API Endpoints

1. **Authentication**
   - POST /api/auth/login
   - POST /api/auth/register
   - POST /api/auth/refresh-token
   - POST /api/auth/logout

2. **User Management**
   - GET /api/users/{id}
   - PUT /api/users/{id}
   - DELETE /api/users/{id}
   - GET /api/users/me

3. **Role Management**
   - GET /api/roles
   - POST /api/roles
   - PUT /api/roles/{id}
   - DELETE /api/roles/{id}

## Dependencies
- Microsoft.AspNetCore.Authentication.JwtBearer 7.0.14
- Microsoft.Identity.Web 1.25.10
- Microsoft.EntityFrameworkCore 7.0.14
- Microsoft.EntityFrameworkCore.SqlServer 7.0.14
- prometheus-net.AspNetCore 8.0.0
- Swashbuckle.AspNetCore 6.5.0

## Setup Instructions

1. **Configure Azure AD B2C**
   ```json
   {
     "AzureAdB2C": {
       "Instance": "https://your-tenant.b2clogin.com",
       "ClientId": "your-client-id",
       "Domain": "your-tenant.onmicrosoft.com",
       "SignUpSignInPolicyId": "B2C_1_signupsignin",
       "ClientSecret": "your-client-secret",
       "CallbackPath": "/signin-oidc"
     }
   }
   ```

2. **Configure Database**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=UserServiceDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Start the Service**
   ```bash
   dotnet run --project services/user-service/UserService.csproj
   ```

4. **Access Swagger UI**
   - URL: http://localhost:5002/swagger

## Monitoring
- Prometheus metrics: http://localhost:5002/metrics
- Health checks: http://localhost:5002/health
- Ready checks: http://localhost:5002/health/ready

## Security Features

1. **Password Policies**
   - Minimum length: 8 characters
   - Requires uppercase and lowercase
   - Requires numbers
   - Requires special characters
   - Password history: 5 previous passwords
   - Maximum failed attempts: 5
   - Lockout duration: 15 minutes

2. **Token Configuration**
   - Access token lifetime: 1 hour
   - Refresh token lifetime: 7 days
   - Token encryption: AES-256
   - Token signing: RSA-2048

3. **Session Management**
   - Maximum concurrent sessions: 3
   - Session timeout: 30 minutes
   - Automatic session cleanup

## Troubleshooting

### Common Issues

1. **Authentication Failures**
   - Verify Azure AD B2C configuration
   - Check token validation
   - Ensure correct scopes are requested
   - Verify user credentials

2. **Database Issues**
   - Check connection string
   - Verify database permissions
   - Monitor database health

3. **Role Management Issues**
   - Verify role assignments
   - Check role permissions
   - Monitor role changes

4. **Performance Issues**
   - Monitor token generation
   - Check database queries
   - Verify cache configuration 