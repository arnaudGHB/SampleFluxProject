# Fix-UserSkillManagement.ps1
# This script fixes compilation errors in the UserSkillManagement microservice

param(
    [string]$SolutionPath = "C:\Users\FAYA COMPUTER\Documents\projets\PROJET FLUX\SampleFluxProject\CBSManagementService.sln",
    [string]$SourceMicroservice = "UserServiceManagement",
    [string]$TargetMicroservice = "UserSkillManagement"
)

# Set error action preference
$ErrorActionPreference = "Stop"
Write-Host "=== Fixing $TargetMicroservice Microservice ===" -ForegroundColor Cyan

# Function to add NuGet packages to a project
function Add-NuGetPackage {
    param(
        [string]$ProjectPath,
        [string]$PackageName,
        [string]$Version
    )
    
    Write-Host "Adding $PackageName $Version to $([System.IO.Path]::GetFileName($ProjectPath))..." -ForegroundColor Yellow
    dotnet add "$ProjectPath" package $PackageName --version $Version
}

# Add required NuGet packages to each project
$projects = @(
    @{ Path = "CoreServices\$TargetMicroservice\CBS.$TargetMicroservice.API\CBS.$TargetMicroservice.API.csproj"; 
       Packages = @(
           @{ Name = "FluentValidation.AspNetCore"; Version = "11.3.1" },
           @{ Name = "Microsoft.AspNetCore.Authentication.JwtBearer"; Version = "8.0.13" },
           @{ Name = "Microsoft.EntityFrameworkCore"; Version = "8.0.7" },
           @{ Name = "Microsoft.EntityFrameworkCore.Design"; Version = "8.0.7" },
           @{ Name = "NLog.Web.AspNetCore"; Version = "5.3.9" },
           @{ Name = "Serilog.AspNetCore"; Version = "8.0.1" },
           @{ Name = "Swashbuckle.AspNetCore"; Version = "6.5.0" }
       )
    },
    @{ Path = "CoreServices\$TargetMicroservice\CBS.$TargetMicroservice.Data\CBS.$TargetMicroservice.Data.csproj"; 
       Packages = @(
           @{ Name = "Microsoft.EntityFrameworkCore"; Version = "8.0.7" },
           @{ Name = "Microsoft.Web.Administration"; Version = "11.1.0" }
       )
    },
    @{ Path = "CoreServices\$TargetMicroservice\CBS.$TargetMicroservice.Domain\CBS.$TargetMicroservice.Domain.csproj"; 
       Packages = @(
           @{ Name = "Microsoft.EntityFrameworkCore"; Version = "8.0.7" },
           @{ Name = "Microsoft.EntityFrameworkCore.Design"; Version = "8.0.7" },
           @{ Name = "Microsoft.EntityFrameworkCore.SqlServer"; Version = "8.0.7" },
           @{ Name = "Microsoft.EntityFrameworkCore.Tools"; Version = "8.0.7" }
       )
    }
)

# Add NuGet packages to each project
foreach ($project in $projects) {
    $projectPath = Join-Path (Split-Path $SolutionPath -Parent) $project.Path
    foreach ($package in $project.Packages) {
        Add-NuGetPackage -ProjectPath $projectPath -PackageName $package.Name -Version $package.Version
    }
}

# Create required interfaces and base classes
$commonPath = "CoreServices\$TargetMicroservice\CBS.$TargetMicroservice.Common"
$dataPath = "CoreServices\$TargetMicroservice\CBS.$TargetMicroservice.Data"
$helperPath = "CoreServices\$TargetMicroservice\CBS.$TargetMicroservice.Helper"

# Create IAuditable interface
$iauditableContent = @"
using System;

namespace CBS.$TargetMicroservice.Data
{
    public interface IAuditable
    {
        DateTime CreatedDate { get; set; }
        string CreatedBy { get; set; }
        DateTime? ModifiedDate { get; set; }
        string? ModifiedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}
"@

# Create ICurrentUserService interface
$icurrentUserServiceContent = @"
using System.Security.Claims;
using System.Threading.Tasks;

namespace CBS.$TargetMicroservice.Data
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
        ClaimsPrincipal? Principal { get; }
        Task<bool> IsInRoleAsync(string role);
    }
}
"@

# Create UserInfoToken class
$userInfoTokenContent = @"
namespace CBS.$TargetMicroservice.Data.Dto
{
    public class UserInfoToken
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
"@

# Create the Dto directory if it doesn't exist
$dtoPath = Join-Path (Split-Path $SolutionPath -Parent) "$dataPath\Dto"
if (-not (Test-Path $dtoPath)) {
    New-Item -ItemType Directory -Path $dtoPath -Force | Out-Null
}

# Save the interface and class files
$iauditablePath = Join-Path (Split-Path $SolutionPath -Parent) "$dataPath\IAuditable.cs"
$icurrentUserServicePath = Join-Path (Split-Path $SolutionPath -Parent) "$dataPath\ICurrentUserService.cs"
$userInfoTokenPath = Join-Path $dtoPath "UserInfoToken.cs"

$iauditableContent | Out-File -FilePath $iauditablePath -Encoding utf8 -Force
$icurrentUserServiceContent | Out-File -FilePath $icurrentUserServicePath -Encoding utf8 -Force
$userInfoTokenContent | Out-File -FilePath $userInfoTokenPath -Encoding utf8 -Force

# Update project references in the solution
dotnet sln "$SolutionPath" add (Join-Path (Split-Path $SolutionPath -Parent) "$dataPath\CBS.$TargetMicroservice.Data.csproj")
dotnet sln "$SolutionPath" add (Join-Path (Split-Path $SolutionPath -Parent) "$commonPath\CBS.$TargetMicroservice.Common.csproj")
dotnet sln "$SolutionPath" add (Join-Path (Split-Path $SolutionPath -Parent) "$helperPath\CBS.$TargetMicroservice.Helper.csproj")

# Restore and build the solution
Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore "$SolutionPath"

Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build "$SolutionPath"

Write-Host "=== Fixes applied successfully! ===" -ForegroundColor Green
Write-Host "Please open the solution in Visual Studio and rebuild to verify all errors are resolved." -ForegroundColor Green
