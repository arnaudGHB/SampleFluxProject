# Script de scaffolding pour le template de microservice MODELE

# --- ÉTAPE 1 : CRÉATION DE L'ARBORESCENCE ---

Write-Host "--- [ÉTAPE 1/3] Création de l'arborescence des projets et des dossiers ---"

# 1.1 : Création du dossier principal et des 7 projets
Write-Host "INFO: Création du répertoire principal MODELE..."
New-Item -ItemType Directory -Force -Path ".\MODELE"
Set-Location ".\MODELE"

Write-Host "INFO: Création des 7 projets de la Clean Architecture pour .NET 8.0..."
dotnet new webapi -n CBS.MODELE.API --framework net8.0
dotnet new classlib -n CBS.MODELE.Common --framework net8.0
dotnet new classlib -n CBS.MODELE.Data --framework net8.0
dotnet new classlib -n CBS.MODELE.Domain --framework net8.0
dotnet new classlib -n CBS.MODELE.Helper --framework net8.0
dotnet new classlib -n CBS.MODELE.MediatR --framework net8.0
dotnet new classlib -n CBS.MODELE.Repository --framework net8.0

# 1.2 : Création des sous-dossiers internes
Write-Host "INFO: Création de la structure de dossiers interne..."
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.API\Controllers"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.API\Helpers\DependencyResolver"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.API\Helpers\MapperConfiguration"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.API\Helpers\MappingProfile"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.API\Middlewares\AuditLogMiddleware"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.API\Middlewares\ExceptionHandlingMiddleware"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.API\Middlewares\JwtValidator"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.API\Middlewares\LoggingMiddleware"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.API\Middlewares\SecurityHeadersMiddleware"

New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.Common\GenericRespository"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.Common\UnitOfWork"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.Data\Dto"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.Data\Entity"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.Data\Enum"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.Domain\Context"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.Helper\Helper"
New-Item -ItemType Directory -Force -Path ".\CBS.MODELE.MediatR\Behaviors"

Write-Host "SUCCESS: Arborescence complète créée."
Write-Host "----------------------------------------------------"

# --- ÉTAPE 2 : INSTALLATION DES DÉPENDANCES NUGET ---

Write-Host "--- [ÉTAPE 2/3] Installation des dépendances NuGet avec versions exactes ---"

# 2.1 : Dépendances pour la couche API
Write-Host "INFO: Installation des packages pour la couche API..."
Set-Location ".\CBS.MODELE.API"
dotnet add package FluentValidation.AspNetCore --version 11.3.1
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.7
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.7
dotnet add package NLog.Web.AspNetCore --version 5.3.9
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
Set-Location ".."

# 2.2 : Dépendances pour la couche MediatR
Write-Host "INFO: Installation des packages pour la couche MediatR..."
Set-Location ".\CBS.MODELE.MediatR"
dotnet add package MediatR --version 12.2.0
dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.9.0
dotnet add package AutoMapper --version 13.0.1
dotnet add package BCrypt.Net-Next --version 4.0.3
Set-Location ".."

# 2.3 : Dépendances pour la couche Domain
Write-Host "INFO: Installation des packages pour la couche Domain..."
Set-Location ".\CBS.MODELE.Domain"
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.7
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.7
Set-Location ".."

Write-Host "SUCCESS: Tous les packages NuGet sont installés."
Write-Host "----------------------------------------------------"

# --- ÉTAPE 3 : CONFIGURATION DES RÉFÉRENCES DE PROJET ---

Write-Host "--- [ÉTAPE 3/3] Configuration des références de projet (câblage des couches) ---"

# 3.1 : Câblage de la couche API
Write-Host "INFO: Configuration des références pour la couche API..."
Set-Location ".\CBS.MODELE.API"
dotnet add reference "..\CBS.MODELE.Common\CBS.MODELE.Common.csproj"
dotnet add reference "..\CBS.MODELE.Data\CBS.MODELE.Data.csproj"
dotnet add reference "..\CBS.MODELE.Domain\CBS.MODELE.Domain.csproj"
dotnet add reference "..\CBS.MODELE.Helper\CBS.MODELE.Helper.csproj"
dotnet add reference "..\CBS.MODELE.MediatR\CBS.MODELE.MediatR.csproj"
dotnet add reference "..\CBS.MODELE.Repository\CBS.MODELE.Repository.csproj"
# Références aux projets communs (à décommenter et adapter si nécessaire)
# dotnet add reference "..\..\..\Common\APICallerHelper\CBS.APICaller.Helper\CBS.APICaller.Helper.csproj"
# dotnet add reference "..\..\..\Common\CustomerLoggerHelper\CBS.CustomLog.Logger\CBS.CustomLog.Logger.csproj"
# dotnet add reference "..\..\..\Common\ServiceDiscovery\CBS.ServicesDelivery.Service\CBS.ServicesDelivery.Service.csproj"
Set-Location ".."

# 3.2 : Câblage des autres couches
Write-Host "INFO: Configuration des références pour les autres couches..."

# Common
Set-Location ".\CBS.MODELE.Common"
dotnet add reference "..\CBS.MODELE.Data\CBS.MODELE.Data.csproj"
Set-Location ".."

# Domain
Set-Location ".\CBS.MODELE.Domain"
dotnet add reference "..\CBS.MODELE.Data\CBS.MODELE.Data.csproj"
Set-Location ".."

# Helper
Set-Location ".\CBS.MODELE.Helper"
dotnet add reference "..\CBS.MODELE.Data\CBS.MODELE.Data.csproj"
Set-Location ".."

# MediatR
Set-Location ".\CBS.MODELE.MediatR"
dotnet add reference "..\CBS.MODELE.Data\CBS.MODELE.Data.csproj"
dotnet add reference "..\CBS.MODELE.Domain\CBS.MODELE.Domain.csproj"
dotnet add reference "..\CBS.MODELE.Helper\CBS.MODELE.Helper.csproj"
dotnet add reference "..\CBS.MODELE.Repository\CBS.MODELE.Repository.csproj"
Set-Location ".."

# Repository
Set-Location ".\CBS.MODELE.Repository"
dotnet add reference "..\CBS.MODELE.Common\CBS.MODELE.Common.csproj"
dotnet add reference "..\CBS.MODELE.Data\CBS.MODELE.Data.csproj"
dotnet add reference "..\CBS.MODELE.Domain\CBS.MODELE.Domain.csproj"
Set-Location ".."

Write-Host "SUCCESS: Toutes les références de projet sont configurées."
Write-Host "----------------------------------------------------"

Write-Host "--- TÂCHE TERMINÉE : Le squelette du template de microservice MODELE est prêt. ---"
Write-Host "Emplacement : $((Get-Item .).FullName)"
