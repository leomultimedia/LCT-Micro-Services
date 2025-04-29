# Database Setup Guide

<div align="center">
  <img src="../images/database-architecture.png" alt="Database Architecture" width="800"/>
  <p><em>Figure 1: Database Architecture Overview</em></p>
</div>

## About This Guide
This guide is part of the Lear Cyber Tech E-Commerce Microservices Platform documentation. It provides detailed instructions for setting up and managing databases for the platform.

**Author**: Dr. Libin Pallikunnel Kurian - *Technical Lead* - [LinkedIn](https://www.linkedin.com/in/dr-libin-pallikunnel-kurian-88741530/)

**Company**: Lear Cyber Tech

**Last Updated**: April 2024

## Overview
This guide covers the setup and management of databases for the e-commerce microservices platform. Each service has its own database, and they are managed using Entity Framework Core.

## Database Configuration

<div align="center">
  <img src="../images/database-configuration.png" alt="Database Configuration" width="600"/>
  <p><em>Figure 2: Database Configuration Overview</em></p>
</div>

### 1. Connection Strings
```json
{
  "ConnectionStrings": {
    "SampleDataDb": "Server=(localdb)\\mssqllocaldb;Database=SampleDataService;Trusted_Connection=True;MultipleActiveResultSets=true",
    "ProductsDb": "Server=(localdb)\\mssqllocaldb;Database=ProductsService;Trusted_Connection=True;MultipleActiveResultSets=true",
    "OrdersDb": "Server=(localdb)\\mssqllocaldb;Database=OrdersService;Trusted_Connection=True;MultipleActiveResultSets=true",
    "PaymentsDb": "Server=(localdb)\\mssqllocaldb;Database=PaymentsService;Trusted_Connection=True;MultipleActiveResultSets=true",
    "UsersDb": "Server=(localdb)\\mssqllocaldb;Database=UsersService;Trusted_Connection=True;MultipleActiveResultSets=true",
    "InventoryDb": "Server=(localdb)\\mssqllocaldb;Database=InventoryService;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 2. Database Contexts
1. **Sample Data Context**
   ```csharp
   public class SampleDataContext : DbContext
   {
       public DbSet<Product> Products { get; set; }
       public DbSet<User> Users { get; set; }
       public DbSet<Order> Orders { get; set; }
       public DbSet<OrderItem> OrderItems { get; set; }
   }
   ```

2. **Products Context**
   ```csharp
   public class ProductsDbContext : DbContext
   {
       public DbSet<Product> Products { get; set; }
       public DbSet<Category> Categories { get; set; }
   }
   ```

3. **Orders Context**
   ```csharp
   public class OrdersDbContext : DbContext
   {
       public DbSet<Order> Orders { get; set; }
       public DbSet<OrderItem> OrderItems { get; set; }
   }
   ```

## Database Setup

<div align="center">
  <img src="../images/database-setup-flow.png" alt="Database Setup Flow" width="600"/>
  <p><em>Figure 3: Database Setup Process</em></p>
</div>

### 1. Initial Setup
1. Create databases:
   ```sql
   CREATE DATABASE SampleDataService;
   CREATE DATABASE ProductsService;
   CREATE DATABASE OrdersService;
   CREATE DATABASE PaymentsService;
   CREATE DATABASE UsersService;
   CREATE DATABASE InventoryService;
   ```

2. Run migrations:
   ```powershell
   # Sample Data Service
   cd services/sample-data-service
   dotnet ef migrations add InitialCreate
   dotnet ef database update

   # Product Service
   cd ../product-service
   dotnet ef migrations add InitialCreate
   dotnet ef database update

   # Repeat for other services
   ```

### 2. Seed Data
1. Sample Data Service:
   ```powershell
   cd services/sample-data-service
   dotnet run --seed
   ```

2. Verify data:
   ```sql
   USE SampleDataService;
   SELECT COUNT(*) FROM Products;
   SELECT COUNT(*) FROM Users;
   SELECT COUNT(*) FROM Orders;
   ```

## Database Management

<div align="center">
  <img src="../images/database-management.png" alt="Database Management" width="600"/>
  <p><em>Figure 4: Database Management Overview</em></p>
</div>

### 1. Migrations
1. Create migration:
   ```powershell
   dotnet ef migrations add <MigrationName> -p <ProjectPath> -s <StartupProjectPath>
   ```

2. Update database:
   ```powershell
   dotnet ef database update -p <ProjectPath> -s <StartupProjectPath>
   ```

3. Remove migration:
   ```powershell
   dotnet ef migrations remove -p <ProjectPath> -s <StartupProjectPath>
   ```

### 2. Backup and Restore
1. Backup database:
   ```sql
   BACKUP DATABASE SampleDataService
   TO DISK = 'C:\Backups\SampleDataService.bak'
   WITH FORMAT, MEDIANAME = 'SampleDataServiceBackup';
   ```

2. Restore database:
   ```sql
   RESTORE DATABASE SampleDataService
   FROM DISK = 'C:\Backups\SampleDataService.bak'
   WITH REPLACE;
   ```

### 3. Maintenance
1. Update statistics:
   ```sql
   EXEC sp_updatestats;
   ```

2. Rebuild indexes:
   ```sql
   ALTER INDEX ALL ON Products REBUILD;
   ```

3. Check database integrity:
   ```sql
   DBCC CHECKDB('SampleDataService') WITH NO_INFOMSGS;
   ```

## Performance Optimization

<div align="center">
  <img src="../images/database-performance.png" alt="Database Performance" width="600"/>
  <p><em>Figure 5: Database Performance Optimization</em></p>
</div>

### 1. Indexing
1. Create indexes:
   ```sql
   CREATE INDEX IX_Products_Name ON Products(Name);
   CREATE INDEX IX_Orders_UserId ON Orders(UserId);
   ```

2. Monitor index usage:
   ```sql
   SELECT OBJECT_NAME(i.object_id) AS TableName,
          i.name AS IndexName,
          ius.user_seeks,
          ius.user_scans,
          ius.user_lookups
   FROM sys.indexes i
   INNER JOIN sys.dm_db_index_usage_stats ius
   ON i.object_id = ius.object_id AND i.index_id = ius.index_id;
   ```

### 2. Query Optimization
1. Use execution plans:
   ```sql
   SET STATISTICS IO ON;
   SET STATISTICS TIME ON;
   ```

2. Monitor slow queries:
   ```sql
   SELECT TOP 10
          qs.execution_count,
          qs.total_logical_reads,
          qs.total_logical_writes,
          qs.total_elapsed_time,
          qs.last_execution_time,
          qt.text
   FROM sys.dm_exec_query_stats qs
   CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
   ORDER BY qs.total_elapsed_time DESC;
   ```

## Troubleshooting

<div align="center">
  <img src="../images/database-troubleshooting.png" alt="Database Troubleshooting" width="600"/>
  <p><em>Figure 6: Database Troubleshooting Process</em></p>
</div>

### 1. Connection Issues
1. Check SQL Server service:
   ```powershell
   Get-Service -Name MSSQLSERVER
   ```

2. Test connection:
   ```powershell
   Test-NetConnection -ComputerName localhost -Port 1433
   ```

3. Verify login:
   ```sql
   SELECT name, type_desc FROM sys.server_principals;
   ```

### 2. Performance Issues
1. Check blocking:
   ```sql
   SELECT blocking_session_id, wait_duration_ms, wait_type
   FROM sys.dm_os_waiting_tasks
   WHERE blocking_session_id IS NOT NULL;
   ```

2. Monitor deadlocks:
   ```sql
   SELECT *
   FROM sys.dm_tran_locks
   WHERE resource_type = 'OBJECT';
   ```

### 3. Data Issues
1. Check constraints:
   ```sql
   SELECT name, type_desc
   FROM sys.check_constraints;
   ```

2. Verify foreign keys:
   ```sql
   SELECT name, type_desc
   FROM sys.foreign_keys;
   ```

## Best Practices

### 1. Design
- Use appropriate data types
- Implement proper indexing
- Normalize data structure
- Use constraints effectively

### 2. Security
- Implement least privilege
- Use parameterized queries
- Encrypt sensitive data
- Regular security audits

### 3. Maintenance
- Regular backups
- Monitor performance
- Update statistics
- Maintain indexes

## Resources
- [SQL Server Documentation](https://docs.microsoft.com/en-us/sql/sql-server/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Database Design Best Practices](https://docs.microsoft.com/en-us/sql/relational-databases/database-design/)
- [Performance Tuning](https://docs.microsoft.com/en-us/sql/relational-databases/performance/)

<div align="center">
  <p>
    <em>© 2024 Lear Cyber Tech. All rights reserved.</em>
  </p>
</div> 