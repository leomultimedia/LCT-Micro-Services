# Sample Data Service Management Script

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('Start', 'Stop', 'Restart', 'Status', 'Build', 'Test')]
    [string]$Action = 'Start',

    [Parameter(Mandatory=$false)]
    [string]$Environment = 'Development',

    [Parameter(Mandatory=$false)]
    [int]$Port = 5001
)

$ErrorActionPreference = "Stop"
$serviceName = "SampleDataService"
$projectPath = "..\SampleDataService.csproj"
$publishPath = "..\publish"

function Start-Service {
    Write-Host "Starting $serviceName..." -ForegroundColor Green
    dotnet run --project $projectPath --environment $Environment --urls "http://localhost:$Port"
}

function Stop-Service {
    Write-Host "Stopping $serviceName..." -ForegroundColor Yellow
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | 
        Where-Object { $_.CommandLine -like "*$serviceName*" } | 
        Stop-Process -Force
}

function Get-ServiceStatus {
    $process = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | 
        Where-Object { $_.CommandLine -like "*$serviceName*" }
    
    if ($process) {
        Write-Host "$serviceName is running (PID: $($process.Id))" -ForegroundColor Green
    } else {
        Write-Host "$serviceName is not running" -ForegroundColor Red
    }
}

function Build-Service {
    Write-Host "Building $serviceName..." -ForegroundColor Cyan
    dotnet build $projectPath -c Release
}

function Test-Service {
    Write-Host "Running tests for $serviceName..." -ForegroundColor Cyan
    dotnet test $projectPath
}

function Publish-Service {
    Write-Host "Publishing $serviceName..." -ForegroundColor Cyan
    if (Test-Path $publishPath) {
        Remove-Item $publishPath -Recurse -Force
    }
    dotnet publish $projectPath -c Release -o $publishPath
}

switch ($Action) {
    'Start' {
        Start-Service
    }
    'Stop' {
        Stop-Service
    }
    'Restart' {
        Stop-Service
        Start-Sleep -Seconds 2
        Start-Service
    }
    'Status' {
        Get-ServiceStatus
    }
    'Build' {
        Build-Service
    }
    'Test' {
        Test-Service
    }
    default {
        Write-Host "Invalid action specified" -ForegroundColor Red
        exit 1
    }
} 