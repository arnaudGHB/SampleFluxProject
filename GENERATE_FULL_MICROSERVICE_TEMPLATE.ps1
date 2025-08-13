# GENERATE_FULL_MICROSERVICE_TEMPLATE.ps1
# This script creates a new microservice with a 7-layer architecture
# Usage: .\GENERATE_FULL_MICROSERVICE_TEMPLATE.ps1 -ServiceName "ProjectManagement"

param (
    [Parameter(Mandatory=$true)]
    [string]$ServiceName
)

# Configuration
$baseDir = "$PSScriptRoot\CoreServices\$ServiceName"
$solutionFile = "$PSScriptRoot\CoreServices\CBS.$ServiceName.sln"
$templateVersion = "1.0.0"
$netVersion = "net8.0"

# Create base directory if it doesn't exist
if (-not (Test-Path -Path $baseDir)) {
    New-Item -ItemType Directory -Path $baseDir | Out-Null
    Write-Host "Created directory: $baseDir"
}

# Create solution file
if (-not (Test-Path -Path $solutionFile)) {
    dotnet new sln -n "CBS.$ServiceName" -o $baseDir
    Write-Host "Created solution file: $solutionFile"
}

# Function to create a new project and add to solution
function Create-Project {
    param (
        [string]$projectType,
        [string]$projectName,
        [string]$framework = $netVersion,
        [string[]]$additionalArgs = @()
    )
    
    $projectPath = "$baseDir\$projectName"
    $projectFile = "$projectPath\$projectName.csproj"
    
    if (-not (Test-Path -Path $projectPath)) {
        New-Item -ItemType Directory -Path $projectPath | Out-Null
        dotnet new $projectType -n $projectName -o $projectPath -f $framework @additionalArgs
        dotnet sln "$solutionFile" add $projectFile
        Write-Host "Created project: $projectName"
    } else {
        Write-Host "Project already exists: $projectName"
    }
}

# Create projects
Create-Project -projectType "classlib" -projectName "CBS.$ServiceName.Common"
Create-Project -projectType "classlib" -projectName "CBS.$ServiceName.Data"
Create-Project -projectType "classlib" -projectName "CBS.$ServiceName.Domain"
Create-Project -projectType "classlib" -projectName "CBS.$ServiceName.Repository"
Create-Project -projectType "classlib" -projectName "CBS.$ServiceName.Service"
Create-Project -projectType "classlib" -projectName "CBS.$ServiceName.MediatR"
Create-Project -projectType "webapi" -projectName "CBS.$ServiceName.API"

# Add project references
$apiProject = "$baseDir\CBS.$ServiceName.API\CBS.$ServiceName.API.csproj"
$serviceProject = "$baseDir\CBS.$ServiceName.Service\CBS.$ServiceName.Service.csproj"
$repositoryProject = "$baseDir\CBS.$ServiceName.Repository\CBS.$ServiceName.Repository.csproj"
$domainProject = "$baseDir\CBS.$ServiceName.Domain\CBS.$ServiceName.Domain.csproj"
$dataProject = "$baseDir\CBS.$ServiceName.Data\CBS.$ServiceName.Data.csproj"
$commonProject = "$baseDir\CBS.$ServiceName.Common\CBS.$ServiceName.Common.csproj"
$mediatrProject = "$baseDir\CBS.$ServiceName.MediatR\CBS.$ServiceName.MediatR.csproj"

# Add project references
dotnet add $apiProject reference $serviceProject $mediatrProject
dotnet add $serviceProject reference $repositoryProject
dotnet add $repositoryProject reference $domainProject
dotnet add $domainProject reference $dataProject
dotnet add $domainProject reference $commonProject

# Add NuGet packages to API project
dotnet add $apiProject package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add $apiProject package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add $apiProject package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add $apiProject package AutoMapper --version 12.0.1
dotnet add $apiProject package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
dotnet add $apiProject package MediatR --version 12.2.0
dotnet add $apiProject package MediatR.Extensions.Microsoft.DependencyInjection --version 12.1.1
dotnet add $apiProject package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore --version 8.0.0

# Add NuGet packages to Domain project
dotnet add $domainProject package Microsoft.EntityFrameworkCore --version 8.0.0

# Add NuGet packages to Repository project
dotnet add $repositoryProject package Microsoft.EntityFrameworkCore --version 8.0.0

# Create directory structure
$folders = @(
    "$baseDir\CBS.$ServiceName.API\Controllers",
    "$baseDir\CBS.$ServiceName.API\Helpers\DependencyResolver",
    "$baseDir\CBS.$ServiceName.API\Helpers\MapperConfiguration",
    "$baseDir\CBS.$ServiceName.API\Middlewares",
    "$baseDir\CBS.$ServiceName.Data\Dto",
    "$baseDir\CBS.$ServiceName.Data\Entity",
    "$baseDir\CBS.$ServiceName.Data\Enum",
    "$baseDir\CBS.$ServiceName.Domain\Context",
    "$baseDir\CBS.$ServiceName.MediatR\Project\Commands",
    "$baseDir\CBS.$ServiceName.MediatR\Project\Queries",
    "$baseDir\CBS.$ServiceName.Repository"
)

foreach ($folder in $folders) {
    if (-not (Test-Path -Path $folder)) {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
        Write-Host "Created directory: $folder"
    }
}

Write-Host "`nMicroservice $ServiceName has been created successfully!"
Write-Host "Solution file: $solutionFile"
Write-Host "Base directory: $baseDir"
Write-Host "`nNext steps:"
Write-Host "1. Open the solution in your IDE"
Write-Host "2. Implement your domain models, repositories, and services"
Write-Host "3. Configure dependency injection in the API project"
Write-Host "4. Run database migrations"
Write-Host "5. Test your API endpoints"
