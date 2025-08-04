# Template de Microservice .NET - Guide de Référence

## Partie I : Fiche d'Identité Technique

### 1.1 Frameworks de Base
- **Framework Cible** : .NET 8.0
- **Framework Web** : ASP.NET Core 8.0
- **ORM** : Entity Framework Core 8.0.7
- **Architecture** : Clean Architecture à 7 couches

### 1.2 Matrice des Dépendances NuGet

| Package | Version | Rôle Principal |
|---------|---------|----------------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.13 | Authentification JWT |
| `Microsoft.EntityFrameworkCore` | 8.0.7 | ORM principal |
| `MediatR` | 12.2.0 | Pattern CQRS |
| `FluentValidation` | 12.0.0 | Validation des requêtes |
| `AutoMapper` | 13.0.1 | Mapping objet-objet |
| `NLog.Web.AspNetCore` | 5.3.9 | Logging structuré |
| `Swashbuckle.AspNetCore` | 6.5.0 | Documentation API |
| `BCrypt.Net-Next` | 4.0.3 | Hachage des mots de passe |

## Partie II : Structure du Projet

### 2.1 Architecture en Couches

```
CBS.YourServiceName.API/           # Couche API
CBS.YourServiceName.Common/        # Utilitaires communs
CBS.YourServiceName.Data/          # Modèles de données et DTOs
CBS.YourServiceName.Domain/        # Logique métier et contexte EF
CBS.YourServiceName.Helper/        # Helpers spécifiques
CBS.YourServiceName.MediatR/       # Commandes et requêtes CQRS
CBS.YourServiceName.Repository/    # Couche d'accès aux données
```

### 2.2 Configuration du Projet API

**CBS.YourServiceName.API.csproj** :
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.1" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.13" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.7" />
    <PackageReference Include="NLog.Web.AspNetCore" Version="5.3.9" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CBS.YourServiceName.Common\CBS.YourServiceName.Common.csproj" />
    <ProjectReference Include="..\CBS.YourServiceName.Data\CBS.YourServiceName.Data.csproj" />
    <ProjectReference Include="..\CBS.YourServiceName.Domain\CBS.YourServiceName.Domain.csproj" />
    <ProjectReference Include="..\CBS.YourServiceName.Helper\CBS.YourServiceName.Helper.csproj" />
    <ProjectReference Include="..\CBS.YourServiceName.MediatR\CBS.YourServiceName.MediatR.csproj" />
    <ProjectReference Include="..\CBS.YourServiceName.Repository\CBS.YourServiceName.Repository.csproj" />
  </ItemGroup>
</Project>
```

## Partie III : Configuration Principale

### 3.1 Program.cs

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        try
        {
            logger.Debug("Démarrage de l'application");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception exception)
        {
            logger.Error(exception, "Arrêt du programme");
            throw;
        }
        finally
        {
            NLog.LogManager.Shutdown();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseNLog();
}
```

### 3.2 appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourDatabase;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Key": "[GÉNÉRER_UNE_CLÉ_SÉCURISÉE]",
    "Issuer": "https://localhost:5001",
    "Audience": "CBS.YourServiceName",
    "MinutesToExpiration": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## Partie IV : Implémentation

### 4.1 Ajout d'une Nouvelle Entité

1. **Créer l'entité dans Data** :
   ```csharp
   // CBS.YourServiceName.Data/Entities/YourEntity.cs
   public class YourEntity : BaseEntity
   {
       public string Name { get; set; }
       // Autres propriétés
   }
   ```

2. **Créer le DTO** :
   ```csharp
   // CBS.YourServiceName.Data/DTOs/YourEntityDto.cs
   public class YourEntityDto
   {
       public Guid Id { get; set; }
       public string Name { get; set; }
   }
   ```

3. **Créer la commande MediatR** :
   ```csharp
   // CBS.YourServiceName.MediatR/YourEntity/Commands/CreateYourEntityCommand.cs
   public class CreateYourEntityCommand : IRequest<ServiceResponse<YourEntityDto>>
   {
       public string Name { get; set; }
   }
   ```

4. **Créer le validateur** :
   ```csharp
   // CBS.YourServiceName.MediatR/YourEntity/Validators/CreateYourEntityCommandValidator.cs
   public class CreateYourEntityCommandValidator : AbstractValidator<CreateYourEntityCommand>
   {
       public CreateYourEntityCommandValidator()
       {
           RuleFor(x => x.Name)
               .NotEmpty()
               .MaximumLength(100);
       }
   }
   ```

5. **Créer le handler** :
   ```csharp
   public class CreateYourEntityCommandHandler : IRequestHandler<CreateYourEntityCommand, ServiceResponse<YourEntityDto>>
   {
       private readonly IUnitOfWork _uow;
       private readonly IMapper _mapper;

       public CreateYourEntityCommandHandler(IUnitOfWork uow, IMapper mapper)
       {
           _uow = uow;
           _mapper = mapper;
       }

       public async Task<ServiceResponse<YourEntityDto>> Handle(CreateYourEntityCommand request, CancellationToken cancellationToken)
       {
           var entity = _mapper.Map<YourEntity>(request);
           await _uow.Repository<YourEntity>().AddAsync(entity);
           await _uow.SaveAsync();
           
           return new ServiceResponse<YourEntityDto>
           {
               Data = _mapper.Map<YourEntityDto>(entity),
               StatusCode = 201,
               Message = "Entité créée avec succès"
           };
       }
   }
   ```

6. **Ajouter le contrôleur** :
   ```csharp
   [ApiController]
   [Route("api/v1/[controller]")]
   public class YourEntityController : BaseController
   {
       private readonly IMediator _mediator;

       public YourEntityController(IMediator mediator)
       {
           _mediator = mediator;
       }

       [HttpPost]
       public async Task<IActionResult> Create([FromBody] CreateYourEntityCommand command)
       {
           var result = await _mediator.Send(command);
           return StatusCode(result.StatusCode, result);
       }
   }
   ```

## Partie V : Bonnes Pratiques

1. **Validation** :
   - Toujours valider les entrées utilisateur
   - Utiliser FluentValidation pour une validation déclarative
   - Retourner des messages d'erreur clairs

2. **Sécurité** :
   - Ne jamais exposer d'entités directement
   - Toujours utiliser des DTOs
   - Valider les tokens JWT
   - Utiliser HTTPS en production

3. **Performance** :
   - Utiliser le chargement différé (lazy loading) avec précaution
   - Implémenter le cache quand nécessaire
   - Optimiser les requêtes LINQ

4. **Logging** :
   - Logger toutes les erreurs
   - Inclure des identifiants de corrélation
   - Ne pas logger d'informations sensibles

## Partie VI : Déploiement

### 6.1 Prérequis
- .NET 8.0 SDK
- SQL Server 2019+
- Docker (optionnel)

### 6.2 Variables d'Environnement
```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://*:5000
ConnectionStrings__DefaultConnection=Server=db;Database=YourDatabase;User=sa;Password=YourStrong@Passw0rd;
JwtSettings__Key=YourSecureKeyHere
```

### 6.3 Docker Compose
```yaml
version: '3.8'

services:
  web:
    build: .
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=YourDatabase;User=sa;Password=YourStrong@Passw0rd;
    depends_on:
      - db

  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
      - MSSQL_PID=Express
    volumes:
      - sql_data:/var/opt/mssql

volumes:
  sql_data:
```

## Partie VII : Dépannage

### 7.1 Erreurs Courantes

**Problème** : Erreur de connexion à la base de données  
**Solution** : Vérifier la chaîne de connexion et que le service SQL Server est en cours d'exécution

**Problème** : Erreur JWT non valide  
**Solution** : Vérifier que la clé secrète est correctement configurée et identique sur tous les services

**Problème** : Erreur de validation  
**Solution** : Vérifier les règles de validation dans les validateurs FluentValidation

### 7.2 Journalisation
Les journaux sont disponibles dans :
- `logs/` en développement
- `var/log/yourapp/` en production
- Sortie de la console dans Docker

## Conclusion

Ce template fournit une base solide pour le développement de microservices .NET. Il intègre les meilleures pratiques en matière d'architecture, de sécurité et de performance. N'hésitez pas à l'adapter aux besoins spécifiques de votre projet.
