<#
.SYNOPSIS
    Starts all microservices in the E-Commerce platform in the correct order.

.DESCRIPTION
    This script starts all microservices in the correct sequence to ensure proper initialization
    and communication between services. It handles port assignments and service dependencies.

.AUTHOR
    Dr. Libin Pallikunnel Kurian
    LinkedIn: https://www.linkedin.com/in/dr-libin-pallikunnel-kurian-88741530/

.PARAMETER Environment
    The environment to run the services in (Development, Staging, Production).
    Default is Development.

.EXAMPLE
    .\start-services.ps1
    Starts all services in Development environment.

.EXAMPLE
    .\start-services.ps1 -Environment Production
    Starts all services in Production environment.

.NOTES
    Port Assignments:
    - API Gateway: 5001
    - Product Service: 5002
    - Order Service: 5003
    - Payment Service: 5004
    - User Service: 5005
    - Notification Service: 5006
    - Inventory Service: 5007
    - Frontend Service: 5000
#>

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('Development', 'Staging', 'Production')]
    [string]$Environment = 'Development'
)

# Define services and their ports
$services = @(
    @{ Name = "ApiGateway"; Port = 5001; Path = "services/api-gateway" },
    @{ Name = "ProductService"; Port = 5002; Path = "services/product-service" },
    @{ Name = "OrderService"; Port = 5003; Path = "services/order-service" },
    @{ Name = "PaymentService"; Port = 5004; Path = "services/payment-service" },
    @{ Name = "NotificationService"; Port = 5005; Path = "services/notification-service" },
    @{ Name = "UserService"; Port = 5006; Path = "services/user-service" },
    @{ Name = "InventoryService"; Port = 5007; Path = "services/inventory-service" },
    @{ Name = "FrontendService"; Port = 5000; Path = "services/frontend-service" }
)

# Function to start a service
function Start-Service {
    param(
        [string]$Name,
        [int]$Port,
        [string]$Path
    )
    
    Write-Host "Starting $Name on port $Port..."
    $process = Start-Process -FilePath "dotnet" -ArgumentList "run --project $Path/$Name.csproj" -PassThru
    Start-Sleep -Seconds 5
    return $process
}

# Start services in order
$processes = @()
foreach ($service in $services) {
    $process = Start-Service -Name $service.Name -Port $service.Port -Path $service.Path
    $processes += $process
}

Write-Host "All services started. Press Ctrl+C to stop all services."

# Wait for Ctrl+C
try {
    while ($true) {
        Start-Sleep -Seconds 1
    }
}
finally {
    # Cleanup on exit
    foreach ($process in $processes) {
        if ($process -and -not $process.HasExited) {
            Stop-Process -Id $process.Id -Force
        }
    }
} 