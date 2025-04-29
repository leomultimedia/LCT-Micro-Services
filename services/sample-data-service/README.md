# Sample Data Service

## Overview
The Sample Data Service provides initial data for development and testing purposes. It manages seed data for products, users, and orders, and supports data reset functionality.

## Configuration
- **Port**: 5000
- **Base URL**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Database**: SQL Server LocalDB

## Features
1. **Data Seeding**
   - Initial product catalog
   - Sample user accounts
   - Test orders and order items
   - Inventory records

2. **Data Reset**
   - Reset all data to initial state
   - Preserve database schema
   - Maintain referential integrity

3. **API Endpoints**
   - GET /api/products - List all products
   - GET /api/users - List all users
   - GET /api/orders - List all orders
   - POST /api/reset - Reset all data

## Sample Data
1. **Products**
   - 10 sample products
   - Various categories
   - Realistic pricing
   - Stock levels

2. **Users**
   - Admin user
   - Customer users
   - Different roles
   - Test credentials

3. **Orders**
   - Completed orders
   - Pending orders
   - Various order items
   - Different payment statuses

## Dependencies
- Microsoft.EntityFrameworkCore 7.0.0
- Microsoft.EntityFrameworkCore.SqlServer 7.0.0
- Swashbuckle.AspNetCore 6.5.0

## Setup
1. Configure database:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SampleDataService;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

2. Start the service:
   ```powershell
   dotnet run --project services/sample-data-service/SampleDataService.csproj
   ```

3. Access Swagger UI:
   - Open http://localhost:5000/swagger
   - Explore available endpoints

## Data Models
1. **Product**
   - Id (int)
   - Name (string)
   - Description (string)
   - Price (decimal)
   - Stock (int)
   - ImageUrl (string)

2. **User**
   - Id (int)
   - Username (string)
   - Email (string)
   - Role (string)
   - CreatedAt (DateTime)

3. **Order**
   - Id (int)
   - UserId (int)
   - Items (List<OrderItem>)
   - TotalAmount (decimal)
   - Status (string)
   - CreatedAt (DateTime)

4. **OrderItem**
   - Id (int)
   - OrderId (int)
   - ProductId (int)
   - Quantity (int)
   - UnitPrice (decimal)

## Troubleshooting
1. **Database Issues**
   - Verify connection string
   - Check SQL Server LocalDB installation
   - Ensure database permissions

2. **Data Reset Issues**
   - Check foreign key constraints
   - Verify seed data configuration
   - Monitor transaction logs

3. **API Issues**
   - Check service health
   - Verify endpoint configurations
   - Monitor request logs 