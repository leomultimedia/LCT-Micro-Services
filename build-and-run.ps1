# Build all services
Write-Host "Building all services..."
dotnet build

# Start services in background with correct ports
Write-Host "Starting services..."

# API Gateway
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/api-gateway/ApiGateway.csproj --urls=http://*:5001" -NoNewWindow

# Product Service
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/product-service/ProductService.csproj --urls=http://*:5011" -NoNewWindow

# Order Service
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/order-service/OrderService.csproj --urls=http://*:5012" -NoNewWindow

# Payment Service
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/payment-service/PaymentService.csproj --urls=http://*:5013" -NoNewWindow

# Notification Service
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/notification-service/NotificationService.csproj --urls=http://*:5014" -NoNewWindow

# User Service
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/user-service/UserService.csproj --urls=http://*:5015" -NoNewWindow

# Inventory Service
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/inventory-service/InventoryService.csproj --urls=http://*:5018" -NoNewWindow

# Frontend Service
Start-Process -FilePath "dotnet" -ArgumentList "run --project services/frontend-service/FrontendService.csproj --urls=http://*:5019" -NoNewWindow

Write-Host "All services started. Press Ctrl+C to stop all services."
Write-Host "API Gateway: http://localhost:5001"
Write-Host "Product Service: http://localhost:5011"
Write-Host "Order Service: http://localhost:5012"
Write-Host "Payment Service: http://localhost:5013"
Write-Host "Notification Service: http://localhost:5014"
Write-Host "User Service: http://localhost:5015"
Write-Host "Inventory Service: http://localhost:5018"
Write-Host "Frontend Service: http://localhost:5019"

# Wait for user input to stop
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 