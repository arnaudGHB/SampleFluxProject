# Microservice Template Generator - Simplified Version

param(
    [string]$ServiceName = "UserServiceMGT",
    [string]$SourceService = "UserServiceManagement"
)

# Configuration
$BasePath = "$PSScriptRoot\CoreServices"
$DestPath = "$BasePath\$ServiceName"

# Check if destination exists
if (Test-Path $DestPath) {
    Write-Host "Error: Directory $DestPath already exists." -ForegroundColor Red
    exit 1
}

# Create directory structure
Write-Host "Creating project structure..."
New-Item -ItemType Directory -Path $DestPath -Force | Out-Null

# Create .NET projects
$projects = @(
    "webapi -n CBS.$ServiceName.API --framework net8.0",
    "classlib -n CBS.$ServiceName.Common --framework net8.0",
    "classlib -n CBS.$ServiceName.Data --framework net8.0",
    "classlib -n CBS.$ServiceName.Domain --framework net8.0",
    "classlib -n CBS.$ServiceName.Helper --framework net8.0",
    "classlib -n CBS.$ServiceName.MediatR --framework net8.0",
    "classlib -n CBS.$ServiceName.Repository --framework net8.0"
)

Push-Location $DestPath
foreach ($project in $projects) {
    Write-Host "Creating project: $project"
    dotnet new $project
}
Pop-Location

# Create essential directories
$dirs = @(
    "CBS.$ServiceName.API\Controllers",
    "CBS.$ServiceName.API\Middlewares",
    "CBS.$ServiceName.API\Helpers",
    "CBS.$ServiceName.Common\GenericRepository",
    "CBS.$ServiceName.Data\Entity"
)

foreach ($dir in $dirs) {
    $fullPath = "$DestPath\$dir"
    New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
}

Write-Host "Project structure created successfully!" -ForegroundColor Green

# Add NuGet packages
Write-Host "Adding NuGet packages..."

# API packages
dotnet add "$DestPath\CBS.$ServiceName.API\CBS.$ServiceName.API.csproj" package FluentValidation.AspNetCore --version 11.3.1
dotnet add "$DestPath\CBS.$ServiceName.API\CBS.$ServiceName.API.csproj" package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.13
dotnet add "$DestPath\CBS.$ServiceName.API\CBS.$ServiceName.API.csproj" package Swashbuckle.AspNetCore --version 6.5.0

# MediatR packages
dotnet add "$DestPath\CBS.$ServiceName.MediatR\CBS.$ServiceName.MediatR.csproj" package MediatR --version 12.2.0
dotnet add "$DestPath\CBS.$ServiceName.MediatR\CBS.$ServiceName.MediatR.csproj" package AutoMapper --version 13.0.1

# Domain packages
dotnet add "$DestPath\CBS.$ServiceName.Domain\CBS.$ServiceName.Domain.csproj" package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.7

# Add project references
Write-Host "Adding project references..."

# API references
$apiRefs = @("Common", "Data", "Domain", "Helper", "MediatR", "Repository")
foreach ($ref in $apiRefs) {
    dotnet add "$DestPath\CBS.$ServiceName.API\CBS.$ServiceName.API.csproj" reference "$DestPath\CBS.$ServiceName.$ref\CBS.$ServiceName.$ref.csproj"
}

# Common references
dotnet add "$DestPath\CBS.$ServiceName.Common\CBS.$ServiceName.Common.csproj" reference "$DestPath\CBS.$ServiceName.Data\CBS.$ServiceName.Data.csproj"

# Domain references
dotnet add "$DestPath\CBS.$ServiceName.Domain\CBS.$ServiceName.Domain.csproj" reference "$DestPath\CBS.$ServiceName.Data\CBS.$ServiceName.Data.csproj"

# Create BaseEntity.cs
$baseEntityContent = @"
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.$ServiceName.Data.Entity
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
"@

# Save BaseEntity.cs
$baseEntityPath = "$DestPath\CBS.$ServiceName.Data\Entity\BaseEntity.cs"
Set-Content -Path $baseEntityPath -Value $baseEntityContent -Force

# Add projects to solution
Write-Host "Adding projects to solution..."
$solutionPath = "$PSScriptRoot\CBSManagementService.sln"
Get-ChildItem -Path $DestPath -Recurse -Filter "*.csproj" | ForEach-Object {
    dotnet sln $solutionPath add $_.FullName
}

Write-Host "Microservice template created successfully!" -ForegroundColor Green
Write-Host "Location: $DestPath" -ForegroundColor Cyan
