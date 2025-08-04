# Script de scaffolding pour le microservice CheckManagement

# --- ÉTAPE 1 : CRÉATION DE L'ARBORESCENCE ---

Write-Host "--- [ÉTAPE 1/3] Création de l'arborescence des projets et des dossiers ---"

# 1.1 : Création du dossier principal et des 7 projets
Write-Host "INFO: Création du répertoire principal CheckManagement..."
New-Item -ItemType Directory -Force -Path ".\CheckManagement"
Set-Location ".\CheckManagement"

Write-Host "INFO: Création des 7 projets de la Clean Architecture pour .NET 8.0..."
dotnet new webapi -n CBS.CheckManagement.API --framework net8.0
dotnet new classlib -n CBS.CheckManagement.Common --framework net8.0
dotnet new classlib -n CBS.CheckManagement.Data --framework net8.0
dotnet new classlib -n CBS.CheckManagement.Domain --framework net8.0
dotnet new classlib -n CBS.CheckManagement.Helper --framework net8.0
dotnet new classlib -n CBS.CheckManagement.MediatR --framework net8.0
dotnet new classlib -n CBS.CheckManagement.Repository --framework net8.0

# 1.2 : Création des sous-dossiers internes
Write-Host "INFO: Création de la structure de dossiers interne..."
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.API\Controllers"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.API\Helpers\DependencyResolver"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.API\Helpers\MapperConfiguration"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.API\Helpers\MappingProfile"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.API\Middlewares\AuditLogMiddleware"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.API\Middlewares\ExceptionHandlingMiddleware"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.API\Middlewares\JwtValidator"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.API\Middlewares\LoggingMiddleware"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.API\Middlewares\SecurityHeadersMiddleware"

New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.Common\GenericRespository"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.Common\UnitOfWork"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.Data\Dto"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.Data\Entity"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.Data\Enum"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.Domain\Context"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.Helper\Helper"
New-Item -ItemType Directory -Force -Path ".\CBS.CheckManagement.MediatR\Behaviors"

Write-Host "SUCCESS: Arborescence complète créée."
Write-Host "----------------------------------------------------"

# --- ÉTAPE 2 : INSTALLATION DES DÉPENDANCES NUGET ---

Write-Host "--- [ÉTAPE 2/3] Installation des dépendances NuGet avec versions exactes ---"

# 2.1 : Dépendances pour la couche API
Write-Host "INFO: Installation des packages pour la couche API..."
Set-Location ".\CBS.CheckManagement.API"
dotnet add package FluentValidation.AspNetCore --version 11.3.1
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.7
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.7
dotnet add package NLog.Web.AspNetCore --version 5.3.9
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
Set-Location ".."

# 2.2 : Dépendances pour la couche MediatR
Write-Host "INFO: Installation des packages pour la couche MediatR..."
Set-Location ".\CBS.CheckManagement.MediatR"
dotnet add package MediatR --version 12.2.0
dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.9.0
dotnet add package AutoMapper --version 13.0.1
dotnet add package BCrypt.Net-Next --version 4.0.3
Set-Location ".."

# 2.3 : Dépendances pour la couche Domain
Write-Host "INFO: Installation des packages pour la couche Domain..."
Set-Location ".\CBS.CheckManagement.Domain"
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.7
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.7
Set-Location ".."

Write-Host "SUCCESS: Tous les packages NuGet sont installés."
Write-Host "----------------------------------------------------"

# --- ÉTAPE 3 : CONFIGURATION DES RÉFÉRENCES DE PROJET ---

Write-Host "--- [ÉTAPE 3/3] Configuration des références de projet (câblage des couches) ---"

# 3.1 : Câblage de la couche API
Write-Host "INFO: Configuration des références pour la couche API..."
Set-Location ".\CBS.CheckManagement.API"
dotnet add reference "..\CBS.CheckManagement.Common\CBS.CheckManagement.Common.csproj"
dotnet add reference "..\CBS.CheckManagement.Data\CBS.CheckManagement.Data.csproj"
dotnet add reference "..\CBS.CheckManagement.Domain\CBS.CheckManagement.Domain.csproj"
dotnet add reference "..\CBS.CheckManagement.Helper\CBS.CheckManagement.Helper.csproj"
dotnet add reference "..\CBS.CheckManagement.MediatR\CBS.CheckManagement.MediatR.csproj"
dotnet add reference "..\CBS.CheckManagement.Repository\CBS.CheckManagement.Repository.csproj"
# Références aux projets communs (à adapter selon la structure réelle)
# dotnet add reference "..\..\..\Common\APICallerHelper\CBS.APICaller.Helper\CBS.APICaller.Helper.csproj"
# dotnet add reference "..\..\..\Common\CustomerLoggerHelper\CBS.CustomLog.Logger\CBS.CustomLog.Logger.csproj"
# dotnet add reference "..\..\..\Common\ServiceDiscovery\CBS.ServicesDelivery.Service\CBS.ServicesDelivery.Service.csproj"
Set-Location ".."

# 3.2 : Câblage des autres couches
Write-Host "INFO: Configuration des références pour les autres couches..."

# Common
Set-Location ".\CBS.CheckManagement.Common"
dotnet add reference "..\CBS.CheckManagement.Data\CBS.CheckManagement.Data.csproj"
Set-Location ".."

# Domain
Set-Location ".\CBS.CheckManagement.Domain"
dotnet add reference "..\CBS.CheckManagement.Data\CBS.CheckManagement.Data.csproj"
Set-Location ".."

# Helper
Set-Location ".\CBS.CheckManagement.Helper"
dotnet add reference "..\CBS.CheckManagement.Data\CBS.CheckManagement.Data.csproj"
Set-Location ".."

# MediatR
Set-Location ".\CBS.CheckManagement.MediatR"
dotnet add reference "..\CBS.CheckManagement.Data\CBS.CheckManagement.Data.csproj"
dotnet add reference "..\CBS.CheckManagement.Domain\CBS.CheckManagement.Domain.csproj"
dotnet add reference "..\CBS.CheckManagement.Helper\CBS.CheckManagement.Helper.csproj"
dotnet add reference "..\CBS.CheckManagement.Repository\CBS.CheckManagement.Repository.csproj"
Set-Location ".."

# Repository
Set-Location ".\CBS.CheckManagement.Repository"
dotnet add reference "..\CBS.CheckManagement.Common\CBS.CheckManagement.Common.csproj"
dotnet add reference "..\CBS.CheckManagement.Data\CBS.CheckManagement.Data.csproj"
dotnet add reference "..\CBS.CheckManagement.Domain\CBS.CheckManagement.Domain.csproj"
Set-Location ".."

Write-Host "SUCCESS: Toutes les références de projet sont configurées."
Write-Host "----------------------------------------------------"

Write-Host "--- TÂCHE TERMINÉE : Le squelette du microservice CheckManagement est prêt. ---"
Write-Host "Emplacement : $((Get-Item .).FullName)"
