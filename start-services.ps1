# Start services in the correct order
Write-Host "Starting E-Commerce Microservices Platform..." -ForegroundColor Green

# Start API Gateway
Write-Host "Starting API Gateway..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/api-gateway/ApiGateway.csproj" -NoNewWindow

# Wait for API Gateway to start
Start-Sleep -Seconds 5

# Start Product Service
Write-Host "Starting Product Service..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/product-service/ProductService.csproj" -NoNewWindow

# Start Order Service
Write-Host "Starting Order Service..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/order-service/OrderService.csproj" -NoNewWindow

# Start Payment Service
Write-Host "Starting Payment Service..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/payment-service/PaymentService.csproj" -NoNewWindow

# Start User Service
Write-Host "Starting User Service..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/user-service/UserService.csproj" -NoNewWindow

# Start Notification Service
Write-Host "Starting Notification Service..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/notification-service/NotificationService.csproj" -NoNewWindow

# Start Inventory Service
Write-Host "Starting Inventory Service..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/inventory-service/InventoryService.csproj" -NoNewWindow

# Start Frontend Service
Write-Host "Starting Frontend Service..." -ForegroundColor Yellow
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/frontend-service/FrontendService.csproj" -NoNewWindow

Write-Host "All services started successfully!" -ForegroundColor Green
Write-Host "Press Ctrl+C to stop all services" -ForegroundColor Yellow

# Keep the script running
while ($true) {
    Start-Sleep -Seconds 1
} 