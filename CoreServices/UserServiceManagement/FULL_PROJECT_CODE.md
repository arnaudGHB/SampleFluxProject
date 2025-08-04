# Code Complet du Projet UserServiceManagement


## FULL_PROJECT_CODE.md

```md
```

## PROJECT_DOCUMENTATION.md

```md
# Documentation du Projet UserServiceManagement

## Structure ComplÃ¨te du Projet
```plaintext
[L'arborescence complÃ¨te montrÃ©e ci-dessus]
```

## Fichiers ClÃ©s avec Extraits de Code

### EntitÃ© Utilisateur
`CBS.UserServiceManagement.Data/Entity/User.cs`
```csharp
public class User : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }

    // Constructeurs et mÃ©thodes
}
```

### Contexte EF Core
`CBS.UserServiceManagement.Domain/Context/UserContext.cs`
```csharp
public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration de l'entitÃ© User
    }
}
```

### Handler pour l'Ajout d'Utilisateur
`CBS.UserServiceManagement.MediatR/User/Handlers/AddUserCommandHandler.cs`
```csharp
public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Guid>
{
    public async Task<Guid> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        // Logique de crÃ©ation d'utilisateur
    }
}
```

## Prochaines Ã‰tapes RecommandÃ©es
1. ImplÃ©menter les contrÃ´leurs API
2. Configurer l'injection de dÃ©pendances
3. Ajouter la logique d'authentification JWT
4. DÃ©velopper les tests unitaires

> **Note** : Pour une documentation complÃ¨te avec tout le code source, veuillez consulter directement les fichiers dans l'explorateur de solutions.

```

## CBS.UserServiceManagement.API\appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}

```

## CBS.UserServiceManagement.API\appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

```

## CBS.UserServiceManagement.API\CBS.UserServiceManagement.API.csproj

```csproj
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.6" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.5" />
    <PackageReference Include="NLog.Web.AspNetCore" Version="5.3.9" />
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.1" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\..\Common\APICallerHelper\CBS.APICaller.Helper\CBS.APICaller.Helper.csproj" />
    <ProjectReference Include="..\..\..\Common\CustomerLoggerHelper\CBS.CustomLog.Logger\CBS.CustomLog.Logger.csproj" />
    <ProjectReference Include="..\..\..\Common\ServiceDiscovery\CBS.ServicesDelivery.Service\CBS.ServicesDelivery.Service.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Common\CBS.UserServiceManagement.Common.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Domain\CBS.UserServiceManagement.Domain.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Helper\CBS.UserServiceManagement.Helper.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.MediatR\CBS.UserServiceManagement.MediatR.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Repository\CBS.UserServiceManagement.Repository.csproj" />
  </ItemGroup>

</Project>

```

## CBS.UserServiceManagement.API\CBS.UserServiceManagement.API.csproj.user

```user
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <ActiveDebugProfile>https</ActiveDebugProfile>
  </PropertyGroup>
</Project>
```

## CBS.UserServiceManagement.API\CBS.UserServiceManagement.API.http

```http
@CBS.UserServiceManagement.API_HostAddress = http://localhost:5121

GET {{CBS.UserServiceManagement.API_HostAddress}}/weatherforecast/
Accept: application/json

###

```

## CBS.UserServiceManagement.API\Program.cs

```cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

```

## CBS.UserServiceManagement.API\Properties\launchSettings.json

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5121",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7089;http://localhost:5121",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}

```

## CBS.UserServiceManagement.Common\CBS.UserServiceManagement.Common.csproj

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.6" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
  </ItemGroup>

</Project>

```

## CBS.UserServiceManagement.Common\GenericRespository\GenericRespository.cs

```csharp
using CBS.UserServiceManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace CBS.UserServiceManagement.Common
{
    public class GenericRepository<TC, TContext> : IGenericRepository<TC>
        where TC : class
        where TContext : DbContext
    {
        protected readonly TContext Context;
        internal readonly DbSet<TC> DbSet;
        protected IUnitOfWork<TContext> _uow;

        protected GenericRepository(IUnitOfWork<TContext> uow)
        {
            Context = uow.Context;
            this._uow = uow;
            DbSet = Context.Set<TC>();
        }

        public IQueryable<TC> All => Context.Set<TC>();

        public void Add(TC entity)
        {
            Context.Add(entity);
        }
        public IQueryable<TC> FindBy(Expression<Func<TC, bool>> predicate)
        {
            IQueryable<TC> queryable = DbSet.AsNoTracking();
            return queryable.Where(predicate);
        }
        public IQueryable<TC> AllIncluding(params Expression<Func<TC, object>>[] includeProperties)
        {
            return GetAllIncluding(includeProperties);
        }
        public IQueryable<TC> FindByInclude(Expression<Func<TC, bool>> predicate, params Expression<Func<TC, object>>[] includeProperties)
        {
            var query = GetAllIncluding(includeProperties);
            return query.Where(predicate);
        }
        private IQueryable<TC> GetAllIncluding(params Expression<Func<TC, object>>[] includeProperties)
        {
            IQueryable<TC> queryable = DbSet.AsNoTracking();
            return includeProperties.Aggregate
              (queryable, (current, includeProperty) => current.Include(includeProperty));
        }
        public void Attach(TC entity)
        {
            Context.Set<TC>().Attach(entity);
        }
        private object GetIdValue(TC entity)
        {
            var entityType = typeof(TC);
            var keyProperties = entityType.GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);
            if (keyProperties.Count() == 0)
                throw new Exception("No key");
            if (keyProperties.Count() > 1)
                throw new Exception("Composite keys not supported");
            var keyProperty = keyProperties.First();
            return keyProperty.GetValue(entity);
        }
        public TC Find(string id)
        {
            return Context.Set<TC>().Find(id);
        }
        public async Task<TC> FindAsync(string id)
        {
            return await Context.Set<TC>().FindAsync(id);
        }
        public virtual void Update(TC entity)
        {
            Context.Update(entity);
        }
        public virtual void UpdateInCasecade(TC entity)
        {
            try
            {
                var conflictingAccount = Context.Set<TC>().Find(GetIdValue(entity));
                if (conflictingAccount != null)
                {
                    Context.Entry(conflictingAccount).CurrentValues.SetValues(entity);
                }
                else
                {
                    Context.Set<TC>().Attach(entity);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task SaveChangesAsync()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public virtual void UpdateRange(List<TC> entities)
        {
            Context.UpdateRange(entities);
        }
        public void RemoveRange(IEnumerable<TC> lstEntities)
        {
            Context.Set<TC>().RemoveRange(lstEntities);
        }
        public void AddRange(IEnumerable<TC> lstEntities)
        {
            try
            {
                Context.Set<TC>().AddRange(lstEntities);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public void InsertUpdateGraph(TC entity)
        {
            Context.Set<TC>().Add(entity);
        }
        public virtual void Delete(string id)
        {
            var entity = Context.Set<TC>().Find(id) as BaseEntity;
            if (entity != null)
            {
                entity.IsDeleted = true;
                Context.Update(entity);
            }
        }
        public virtual void Delete(TC entityData)
        {
            var entity = entityData as BaseEntity;
            if (entity != null)
            {
                entity.IsDeleted = true;
                Context.Update(entity);
            }
        }
        public virtual void Remove(TC entity)
        {
            Context.Remove(entity);
        }
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
```

## CBS.UserServiceManagement.Common\GenericRespository\IGenericRepository.cs

```cs
// Source: Votre code source exact.
using System.Linq.Expressions;

namespace CBS.UserServiceManagement.Common
{
    public interface IGenericRepository<TC> where TC : class
    {
        IQueryable<TC> All { get; }
        void Attach(TC entity);
        IQueryable<TC> AllIncluding(params Expression<Func<TC, object>>[] includeProperties);
        IQueryable<TC> FindByInclude(Expression<Func<TC, bool>> predicate, params Expression<Func<TC, object>>[] includeProperties);
        IQueryable<TC> FindBy(Expression<Func<TC, bool>> predicate);
        TC Find(string id);
        Task<TC> FindAsync(string id);
        void Add(TC entity);
        void Update(TC entity);
        void UpdateInCasecade(TC entity);
        void UpdateRange(List<TC> entities);
        void Delete(string id);
        void Delete(TC entity);
        void Remove(TC entity);
        void InsertUpdateGraph(TC entity);
        void RemoveRange(IEnumerable<TC> lstEntities);
        void AddRange(IEnumerable<TC> lstEntities);
        Task SaveChangesAsync();
    }
}
```

## CBS.UserServiceManagement.Common\UnitOfWork\IUnitOfWork.cs

```cs
// Source: Votre code source exact.
using Microsoft.EntityFrameworkCore;

namespace CBS.UserServiceManagement.Common
{
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        int Save();
        Task<int> SaveAsync();
        Task<int> SavingMigrationAsync(string branchId);
        TContext Context { get; }
    }
}
```

## CBS.UserServiceManagement.Common\UnitOfWork\UnitOfWork.cs

```cs
// Source: Votre code source exact.
using CBS.UserServiceManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.Common
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
         where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly ILogger<UnitOfWork<TContext>> _logger;
        private readonly UserInfoToken _userInfoToken;

        public UnitOfWork(
            TContext context,
            ILogger<UnitOfWork<TContext>> logger,
            UserInfoToken userInfoToken)
        {
            _context = context;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        public int Save()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    SetModifiedInformation();
                    var retValu = _context.SaveChanges();
                    transaction.Commit();
                    return retValu;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e, e.Message);
                    return 0;
                }
            }
        }
        public async Task<int> SaveAsync()
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        SetModifiedInformation();
                        var val = await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return val;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var conflictingEntry = ex.Entries.Single(e => e.State == EntityState.Modified || e.State == EntityState.Deleted);
                        var conflictingEntity = conflictingEntry.Entity;
                        return 1;
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        await transaction.RollbackAsync();
                        string values = GetInnerExceptionMessages(ex);
                        throw new DbUpdateException($"Failed to persist to the database. Error: {values}");
                    }
                    catch (Exception ex)
                    {
                        string message = GetInnerExceptionMessages(ex);
                        _logger.LogError(ex, ex.Message);
                        await transaction.RollbackAsync();
                        throw new HttpRequestException($"Failed to persist to the database. Error: {ex.Message}");
                    }
                }
            });
        }
        public async Task<int> SavingMigrationAsync(string b)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        SetModifiedInformationForMigration(b);
                        var val = await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return val;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }
        public string GetInnerExceptionMessages(Exception exception)
        {
            if (exception == null) return string.Empty;
            var innerMessage = "";
            if (exception.InnerException != null)
            {
                innerMessage = "\n" + GetInnerExceptionMessages(exception.InnerException);
            }
            return exception.Message + innerMessage;
        }
        public TContext Context => _context;
        public void Dispose()
        {
            _context.Dispose();
        }
        private void SetModifiedInformation()
        {
            foreach (var entry in Context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.CreatedBy = _userInfoToken.Id;
                    entry.Entity.ModifiedBy = _userInfoToken.Id;
                    entry.Entity.IsDeleted = false;
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity.IsDeleted)
                    {
                        entry.Entity.DeletedBy = _userInfoToken.Id;
                        entry.Entity.DeletedDate = DateTime.Now;
                    }
                    else
                    {
                        entry.Entity.ModifiedBy = _userInfoToken.Id;
                        entry.Entity.ModifiedDate = DateTime.Now;
                    }
                }
            }
        }
        private void SetModifiedInformationForMigration(string b)
        {
            // Logic for migration...
        }
    }
}
```

## CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.6" />
    <PackageReference Include="Microsoft.Web.Administration" Version="11.1.0" />
  </ItemGroup>

  <ItemGroup>
    <Folder Include="Enum\" />
  </ItemGroup>

</Project>

```

## CBS.UserServiceManagement.Data\Dto\UserDto.cs

```cs
// Source: Modèle original, corrigé pour être cohérent avec l'entité User.
using System;

namespace CBS.UserServiceManagement.Data.Dto // J'ajuste le namespace pour la cohérence
{
    public class UserDto
    {
        // Correction : Le type de l'ID doit correspondre à celui de l'entité (string).
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

```

## CBS.UserServiceManagement.Data\Dto\UserInfoToken.cs

```cs
namespace CBS.UserServiceManagement.Data
{
    public class UserInfoToken
    {
        public string Id { get; set; } = string.Empty; // Initialisation pour éviter les nulls
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; }
    }
}
```

## CBS.UserServiceManagement.Data\Entity\BaseEntity.cs

```cs

using Microsoft.Web.Administration;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.UserServiceManagement.Data.Entity
{


    public abstract class BaseEntity
    {
        private DateTime _createdDate;

        public DateTime CreatedDate
        {
            get => _createdDate.ToLocalTime();
            set => _createdDate = value.ToLocalTime();
        }
        private string _bankId;

        public string BankId
        {
            get => _bankId;
            set => _bankId = value;
        }
        private string _branchId;

        public string BranchId
        {
            get => _branchId;
            set => _branchId = value;
        }
        public string CreatedBy { get; set; } = "b956f743-f846-4d83-8804-4046f259b5cf";

        public string? FullName
        {
            get => _fullName;
            set => _fullName = value;
        }

        private DateTime _modifiedDate;

        public DateTime ModifiedDate
        {
            get => _modifiedDate.ToLocalTime();
            set => _modifiedDate = value.ToLocalTime();
        }

        public string ModifiedBy { get; set; } = "b956f743-f846-4d83-8804-4046f259b5cf";
        private DateTime _deletedDate;

        public DateTime DeletedDate
        {
            get => _deletedDate.ToLocalTime();
            set => _deletedDate = value.ToLocalTime();
        }

        public string? DeletedBy { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public bool IsDeleted { get; set; } = false;

        private string _TempData = "";
        private string? _fullName;

        public string TempData
        {
            get => _TempData;
            set => _TempData = value;
        }
        public BaseEntity GetBaseEntity()
        {
            return this;
        }

    }

    public abstract class BaseEntityNotification
    {
        private DateTime _createdDate;

        public DateTime CreatedDate
        {
            get => _createdDate.ToLocalTime();
            set => _createdDate = value.ToLocalTime();
        }
        private string _bankId;

        public string BankId
        {
            get => _bankId;
            set => _bankId = value;
        }


        public string CreatedBy { get; set; } = "b956f743-f846-4d83-8804-4046f259b5cf";

        private DateTime _modifiedDate;

        public DateTime ModifiedDate
        {
            get => _modifiedDate.ToLocalTime();
            set => _modifiedDate = value.ToLocalTime();
        }

        public string ModifiedBy { get; set; } = "b956f743-f846-4d83-8804-4046f259b5cf";
        private DateTime? _deletedDate;

        public DateTime? DeletedDate
        {
            get => _deletedDate?.ToLocalTime();
            set => _deletedDate = value?.ToLocalTime();
        }

        public string? DeletedBy { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public bool IsDeleted { get; set; } = false;


    }

}
```

## CBS.UserServiceManagement.Data\Entity\User.cs

```cs
// Source: Modèle implicite, avec l'ID défini directement dans l'entité.
using System;
using System.ComponentModel.DataAnnotations; // Ajout pour l'attribut [Key]

namespace CBS.UserServiceManagement.Data.Entity
{
    public class User : BaseEntity
    {
        [Key] // Déclare cette propriété comme étant la clé primaire de l'entité User.
        public string Id { get; set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }

        // Constructeur privé pour EF Core
        private User() { }

        // Constructeur public pour la création, incluant l'ID.
        public User(string id, string firstName, string lastName, string email, string passwordHash, string role = "User")
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }

        // Méthodes pour mettre à jour l'entité de manière contrôlée
        public void UpdateProfile(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }
    }
}
```

## CBS.UserServiceManagement.Domain\CBS.UserServiceManagement.Domain.csproj

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.6" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.6">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
  </ItemGroup>

</Project>

```

## CBS.UserServiceManagement.Domain\Context\UserContext.cs

```cs
using CBS.UserServiceManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CBS.UserServiceManagement.Domain
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
            // L'injection de la chaîne de connexion se fera dans Program.cs
            // pour plus de flexibilité, mais le principe reste le même.
        }

        // Déclarez un DbSet pour chaque entité gérée par ce microservice.
        // Ici, nous n'avons que l'entité User.
        public DbSet<User> Users { get; set; }
        
        // Si vous aviez une entité AuditLog dans ce service, elle serait ici.
        // public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des contraintes et relations pour l'entité User.
            modelBuilder.Entity<User>(entity =>
            {
                // S'assurer que l'ID est bien la clé primaire.
                // Votre modèle de BaseEntity n'a pas d'Id, donc nous le précisons ici.
                // Si l'ID était dans BaseEntity, cette ligne serait inutile.
                entity.HasKey(e => e.Id);

                // Définir l'index unique pour l'email.
                entity.HasIndex(u => u.Email).IsUnique();

                // Configurer les propriétés pour éviter les longueurs infinies en base de données.
                entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
                entity.Property(u => u.Role).HasMaxLength(50).IsRequired();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Activation du logging des données sensibles (très utile en développement).
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}

```

## CBS.UserServiceManagement.Helper\CBS.UserServiceManagement.Helper.csproj

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\..\Common\APICallerHelper\CBS.APICaller.Helper\CBS.APICaller.Helper.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
  </ItemGroup>

</Project>

```

## CBS.UserServiceManagement.Helper\DataModel\AuditTrailLogger.cs

```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CBS.SystemConfiguration.Helper.DataModel
{
    public class AuditTrailLogger
    {
        public string? Action { get; set; } // Action performed (Create, Update, Delete, etc.)
        public string? UserName { get; set; } // User responsible for the action
        public string? MicroServiceName { get; set; }
        public string StringifyObject { get; set; }// Entity affected by the action
        public string? DetailMessage { get; set; }
        public string? Level { get; set; }
      
        public int StatusCode { get; set; }
        public AuditTrailLogger(string action, string userName, string microService, string stringifyObject, string detailMessage, string level, int statusCode)
        {
            Action = action;
            UserName = userName;
            MicroServiceName = microService;
            StringifyObject = stringifyObject;
            DetailMessage = detailMessage;
            Level = level;
            StatusCode = statusCode;
        }

       
    }
    //public enum LogAction
    //{
    //    Create, Update, Delete, Read, Download, Upload, Login
    //}
   

}

```

## CBS.UserServiceManagement.Helper\DataModel\Authentication\Claim.cs

```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.SystemConfiguration.Helper
{
    public class Claim
    {
        public string claimType { get; set; }
        public string claimValue { get; set; }
    }
}

```

## CBS.UserServiceManagement.Helper\DataModel\Authentication\LoginDto.cs

```cs
namespace CBS.SystemConfiguration.Helper
{
    public class LoginDto
    {
        public LoginDto(string id, string userName, string firstName, string lastName, string email, int expirationTime, string phoneNumber, string bearerToken, bool isAuthenticated, string profilePhoto, List<Claim> claims)
        {
            this.id = id;
            this.userName = userName;
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.expirationTime = expirationTime;
            this.phoneNumber = phoneNumber;
            this.bearerToken = bearerToken;
            this.isAuthenticated = isAuthenticated;
            this.profilePhoto = profilePhoto;
            this.claims = claims;
        }

        public string id { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public int expirationTime { get; set; }
        public string phoneNumber { get; set; }
        public DateTime expirationDate { get; set; }
        public string bearerToken { get; set; }
        public bool isAuthenticated { get; set; }
        public string profilePhoto { get; set; }
        public List<Claim> claims { get; set; }
    }
}
```

## CBS.UserServiceManagement.Helper\Helper\APICallHelper.cs

```cs
using CBS.UserServiceManagement.Helper.DataModel;
using CBS.UserServiceManagement.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CBS.UserServiceManagement.Helper;

using CBS.APICaller.Helper;
 
using CBS.APICaller.Helper.LoginModel.Authenthication;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Collections.Generic;
using System.Diagnostics;

namespace CBS.UserServiceManagement.Helper
{
    public abstract class APICallHelper
    {
      


        public static async Task AuditLogger<T>(string userName, string action, T objectToSerialize, string detailMessage, string level, int statuscode, string token)
            where T : class
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string baseUrl = configuration.GetSection("Logging:AuditTrails:BaseUrl").Value;
            string auditTrailEndpoint = configuration.GetSection("Logging:AuditTrails:AuditTrailEndpoint").Value;
            string microserviceName = configuration.GetSection("Logging:AuditTrails:MicroserviceName").Value;
            string stringifyObject = objectToSerialize != null ? JsonConvert.SerializeObject(objectToSerialize) : null;
            AuditTrailLogger logger = new AuditTrailLogger(action, userName, microserviceName, stringifyObject, detailMessage, level, statuscode);

            var ApiCallerHelper = new ApiCallerHelper(baseUrl, token);
            await ApiCallerHelper.PostAsync(auditTrailEndpoint, logger);
        }
        /// <summary>
        /// Logs an action to the audit trail by sending log information to the specified audit trail API.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize for the audit log.</typeparam>
        /// <param name="action">A string representing the action being logged.</param>
        /// <param name="objectToSerialize">The object to be serialized and included in the audit log.</param>
        /// <param name="detailMessage">A detailed message providing context for the action being logged.</param>
        /// <param name="level">A string representing the log level (e.g., Information, Warning, Error).</param>
        /// <param name="statuscode">An integer representing the HTTP status code associated with the action.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task AuditLogger<T>(string action, T objectToSerialize, string detailMessage, string level, int statuscode)
            where T : class
        {
            // Assuming you have access to HttpContext
            var httpContext = new HttpContextAccessor().HttpContext;

            // Retrieve userName and token from session
            string userName = httpContext.Session.GetString("UserName"); // Adjust key as needed
            string token = httpContext.Session.GetString("Token"); // Adjust key as needed

            // Build configuration settings from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            // Retrieve API endpoint details from configuration
            string baseUrl = configuration.GetSection("Logging:AuditTrails:BaseUrl").Value;
            string auditTrailEndpoint = configuration.GetSection("Logging:AuditTrails:AuditTrailEndpoint").Value;
            string microserviceName = configuration.GetSection("Logging:AuditTrails:MicroserviceName").Value;
            string stringifyObject = objectToSerialize != null ? JsonConvert.SerializeObject(objectToSerialize) : null;
            AuditTrailLogger logger = new AuditTrailLogger(action, userName, microserviceName, stringifyObject, detailMessage, level, statuscode);

            try
            {
                // Call the API to post the audit log
                var apiCallerHelper = new ApiCallerHelper(baseUrl, token);
                await apiCallerHelper.PostAsync(auditTrailEndpoint, logger);
            }
            catch (Exception ex)
            {
                // Log the exception using Debug for monitoring failures in the audit logging process
                Debug.WriteLine($"Failed to log audit trail: {ex.Message}");
            }
        }


        public static async Task<Branch> GetBankInfos(PathHelper _pathHelper, UserInfoToken userInfoToken,string branchId, string token)
        {
            try
            {
                //var login = await AuthenthicationFromIdentityServer(_pathHelper);
                string url = string.Format(_pathHelper.BankManagement_GetBranchInfoEndPoint, branchId);
                var ApiCallerHelper = new ApiCallerHelper(_pathHelper.BankManagementBaseUrl, token);
                var responseobj = await ApiCallerHelper.GetAsync<Branch>(url);
                return responseobj;
            }
            catch (Exception ex)
            {
                await AuditLogger<Branch>(userInfoToken.Email, "GetBranchInfo", null, ex.Message, "Error", 500, userInfoToken.Token);
                throw (ex);
            }
        
        }

        public static async Task<List<Branch>> GetAllBranchInfos(PathHelper _pathHelper, UserInfoToken userInfoToken)
        {
            try
            {
                //var login = await AuthenthicationFromIdentityServer(_pathHelper);
                string url = string.Format(_pathHelper.BankManagement_GetAllBranchInfo);
                var ApiCallerHelper = new ApiCallerHelper(url, userInfoToken.Token);
                var responseobj = await ApiCallerHelper.GetAsync<ServiceResponse<List<Branch>>>(url);
                return responseobj.Data;
            }
            catch (Exception ex)
            {
                await AuditLogger<List<Branch>>(userInfoToken.Email, "GetBranchInfo", null, ex.Message, "Error", 500, userInfoToken.Token);
                throw (ex);
            }

        }


        public static async Task<T> PostCashInOrCashOperation<T>(PathHelper _pathHelper, string token, string stringifyJsonObject, string endPoint)
        {
            var ApiCallerHelper = new ApiCallerHelper(_pathHelper.BankTransactionCashINAndCashOutURL, token);
        
            var responseobj = await ApiCallerHelper.PostAsync<T>(endPoint, stringifyJsonObject);
            return responseobj;

        }

        public static async Task<CallBackRespose> UploadExcelDocument(AddDocumentUploadedCommand command,string apiUrl,string token)
        {
            try
            {
                var additionalParams = new Dictionary<string, string>
                {
                    { "OperationID", command.OperationID },
                    { "DocumentId", command.DocumentId ?? "" },
                    { "DocumentType", command.DocumentType },
                    { "ServiceType", command.ServiceType },
                    { "CallBackBaseUrl", command.CallBackBaseUrl },
                    { "CallBackEndPoint", command.CallBackEndPoint },
                    { "RemoteFilePath", command.RemoteFilePath },
                    { "IsSynchronus", command.IsSynchronus.ToString() }
                };
                var ApiCallerHelper = new ApiCallerHelper(apiUrl, token);

                var response = await ApiCallerHelper.PostFilesAndParamsAsync<CallBackRespose>(apiUrl, additionalParams, command.FormFiles);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading Document: {ex.Message}");
                throw ex;
            }
        }


        public static async Task<List<Branch>> GetAllBankInfos(PathHelper _pathHelper, UserInfoToken userInfoToken,   string token)
        {
            try
            {
                //var login = await AuthenthicationFromIdentityServer(_pathHelper);
                string url = string.Format(_pathHelper.BankManagement_GetAllBranchInfo);
                var ApiCallerHelper = new ApiCallerHelper(url, token);
                var responseobj = await ApiCallerHelper.GetAsync<List<Branch>>(url);
                return responseobj;
            }
            catch (Exception ex)
            {
                await AuditLogger<Branch>(userInfoToken.Email, "GetBranchInfo", null, ex.Message, "Error", 500, userInfoToken.Token);
                throw (ex);
            }


        }
 

 

    }
}
```

## CBS.UserServiceManagement.Helper\Helper\BaseUtilities.cs

```cs
using System.Security.Cryptography;
using System.Text;

namespace CBS.UserServiceManagement.Helper
{
    public static class BaseUtilities
    {
        /// <summary>
        /// Generates a unique insurance number with a specific prefix and maximum size.
        /// </summary>
        /// <param name="maxSize">Maximum size of the generated number.</param>
        /// <param name="prefix">Prefix for the generated number.</param>
        /// <returns>Generated insurance number.</returns>
        public static string GenerateInsuranceUniqueNumber(int maxSize, string prefix)
        {
            const string chars = "1234567890";
            return GenerateUniqueNumber(maxSize, chars, prefix);
        }
        public static DateTime UtcToLocal()
        {
            var utcDateTime = DateTime.UtcNow;
            var localDateTime = utcDateTime.ToLocalTime();
            return localDateTime;
        }


        /// <summary>
        /// Generates a unique number with a specific maximum size.
        /// </summary>
        /// <param name="maxSize">Maximum size of the generated number.</param>
        /// <returns>Generated number.</returns>
        public static string GenerateUniqueNumber(int maxSize)
        {
            const string chars = "1234567890";
            return GenerateUniqueNumber(maxSize, chars);
        }

        /// <summary>
        /// Generates a unique number up to 15 digits.
        /// </summary>
        /// <returns>Generated number.</returns>
        public static string GenerateUniqueNumber()
        {
            const string chars = "1234567890";
            return GenerateUniqueNumber(15, chars);
        }
        /// <summary>
        /// Logs an action and audits it by sending the log information to the audit trail API.
        /// </summary>
        /// <param name="message">A descriptive message detailing the action being logged.</param>
        /// <param name="request">The request object associated with the action, which may contain relevant data.</param>
        /// <param name="statusCode">The HTTP status code representing the outcome of the action, defined in <see cref="HttpStatusCodeEnum"/>.</param>
        /// <param name="logAction">The specific action being logged, represented as a <see cref="LogAction"/> enumeration.</param>
        /// <param name="logLevel">The severity level of the log entry, represented as a <see cref="LogLevelInfo"/> enumeration.</param>
        public static async Task LogAndAuditAsync(string message, object request, HttpStatusCodeEnum statusCode, LogAction logAction, LogLevelInfo logLevel)
        {
            // Call the AuditLogger method to record the action and associated details asynchronously.
            await APICallHelper.AuditLogger(logAction.ToString(), request, message, logLevel.ToString(), (int)statusCode);
        }
        /// <summary>
        /// Adds '237' prefix to the MSISDN if it doesn't already start with it.
        /// </summary>
        /// <param name="msisdn">Mobile number without prefix.</param>
        /// <returns>MSISDN with '237' prefix.</returns>
        public static string Add237Prefix(string msisdn)
        {
            const string prefix = "237";
            if (!msisdn.StartsWith(prefix))
            {
                msisdn = $"{prefix}{msisdn}";
            }
            return msisdn;
        }
        /// <summary>
        /// Gets all the inner exception messages
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetInnerExceptionMessages(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var innerMessage = "";

            if (exception.InnerException != null)
            {
                innerMessage = "\n" + GetInnerExceptionMessages(exception.InnerException);
            }

            return exception.Message + innerMessage;
        }
        private static string GenerateUniqueNumber(int maxSize, string allowedChars, string prefix = "")
        {
            char[] chars = allowedChars.ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return $"{prefix}{result.ToString()}";
        }

        /// <summary>
        /// Converts a DateTime to UTC if it's in Unspecified kind.
        /// </summary>
        /// <param name="dateTime">The DateTime value.</param>
        /// <returns>UTC representation of the input DateTime.</returns>
        public static DateTime ToUtc(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return dateTime.ToUniversalTime();
        }

        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset,
          DateTimeOffset? dateOfDeath)
        {
            var dateToCalculateTo = DateTime.UtcNow;

            if (dateOfDeath != null)
            {
                dateToCalculateTo = dateOfDeath.Value.UtcDateTime;
            }

            int age = dateToCalculateTo.Year - dateTimeOffset.Year;

            if (dateToCalculateTo < dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }

        /// <summary>
        /// Converts a nullable DateTime from UTC to local time if it's in Unspecified kind.
        /// </summary>
        /// <param name="dateTime">The nullable DateTime value in UTC.</param>
        /// <returns>Local time representation of the input DateTime, or null if input is null.</returns>
        public static DateTime UtcToLocal(this DateTime? dateTime)
        {
            if (dateTime.HasValue && dateTime.Value.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc).ToLocalTime();
            }
            return dateTime.Value;
        }
    }

    public static class InsuranceCodeGenerator
    {
        // Function to generate a unique 7-digit insurance code
        public static string GenerateInsuranceCode(List<string> existingCodes)
        {
            string newCode = GenerateRandomCode();

            // Check if the generated code already exists in the provided list
            while (existingCodes.Contains(newCode))
            {
                newCode = GenerateSequentialCode(existingCodes);
            }

            return newCode;
        }

        // Helper function to generate a random 7-digit code
        private static string GenerateRandomCode()
        {
            Random random = new Random();
            int code = random.Next(1000000, 10000000); // Generates a random number between 1,000,000 and 9,999,999
            return code.ToString();
        }

        // Helper function to generate a sequential code based on existing codes
        private static string GenerateSequentialCode(List<string> existingCodes)
        {
            int lastCode = existingCodes.Count > 0 ? int.Parse(existingCodes.Last()) : 9999999;
            int nextCode = lastCode + 1;
            return nextCode.ToString();
        }
    }
}
```

## CBS.UserServiceManagement.Helper\Helper\Enums.cs

```cs
namespace CBS.SystemConfiguration.Helper
{
    public enum SMSTypes
    {
        Subscription,
        Saving,
        Claim,
        Cashout
    }

    public enum ServiceTypes
    {
        ClientMicroService,
        LoanMicroService,
        AccountMicroService,
        ClaimMicroService
    }

    public enum SubscriptionStatus
    {
        Awaiting_Customer_Momo_Validation,
        Unsubscrbed,
        Failed, Subscribed, ReSubcription,
        Unsubscribed,
        ReSubscription
    }

    public enum ResultStatus
    {
        Ok,
        Failed
    }
    /// <summary>
    /// Enumeration representing various HTTP status codes and their corresponding descriptions.
    /// </summary>
    public enum HttpStatusCodeEnum
    {
        /// <summary>
        /// 200 OK: The request has succeeded.
        /// </summary>
        OK = 200,

        /// <summary>
        /// 201 Created: The request has been fulfilled and a new resource has been created.
        /// </summary>
        Created = 201,

        /// <summary>
        /// 204 No Content: The server successfully processed the request and is not returning any content.
        /// </summary>
        NoContent = 204,

        /// <summary>
        /// 400 Bad Request: The server could not understand the request due to invalid syntax.
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// 401 Unauthorized: The request requires user authentication.
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// 403 Forbidden: The server understood the request, but refuses to authorize it.
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 404 Not Found: The server can not find the requested resource.
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// 409 Conflict: The request could not be completed due to a conflict with the current state of the target resource.
        /// </summary>
        Conflict = 409,

        /// <summary>
        /// 500 Internal Server Error: The server has encountered a situation it doesn't know how to handle.
        /// </summary>
        InternalServerError = 500,

        /// <summary>
        /// 502 Bad Gateway: The server, while acting as a gateway or proxy, received an invalid response from the upstream server.
        /// </summary>
        BadGateway = 502,

        /// <summary>
        /// 503 Service Unavailable: The server is not ready to handle the request, often due to being overloaded or down for maintenance.
        /// </summary>
        ServiceUnavailable = 503
    }
    /// <summary>
    /// Represents the various actions that can be logged in the application.
    /// </summary>
    public enum LogAction
    {
        /// <summary>
        /// Indicates that a new resource has been created.
        /// </summary>
        Create,

        /// <summary>
        /// Indicates that an existing resource has been updated.
        /// </summary>
        Update,

        /// <summary>
        /// Indicates that a resource has been deleted.
        /// </summary>
        Delete,

        /// <summary>
        /// Indicates that a resource has been read or retrieved.
        /// </summary>
        Read,

        /// <summary>
        /// Indicates that a file or data has been downloaded.
        /// </summary>
        Download,

        /// <summary>
        /// Indicates that a file or data has been uploaded.
        /// </summary>
        Upload,

        /// <summary>
        /// Indicates that a user has logged in to the system.
        /// </summary>
        Login,

        /// <summary>
        /// Indicates that an accounting posting has been made.
        /// </summary>
        AccountingPosting,

        /// <summary>
        /// Indicates that a user has logged out of the system.
        /// </summary>
        Logout,

        /// <summary>
        /// Indicates that a user has registered for an account.
        /// </summary>
        Register,

        /// <summary>
        /// Indicates that a user has changed their password.
        /// </summary>
        ChangePassword,

        /// <summary>
        /// Indicates that a user has requested a password reset.
        /// </summary>
        ResetPassword,

        /// <summary>
        /// Indicates that an account has been locked due to failed login attempts.
        /// </summary>
        AccountLock,

        /// <summary>
        /// Indicates that an account has been unlocked.
        /// </summary>
        AccountUnlock,

        /// <summary>
        /// Indicates that a user has updated their profile information.
        /// </summary>
        ProfileUpdate,

        /// <summary>
        /// Indicates that funds have been transferred between accounts.
        /// </summary>
        TransferFunds,

        /// <summary>
        /// Indicates that a transaction has failed.
        /// </summary>
        TransactionFailed,

        /// <summary>
        /// Indicates that a transaction has been successful.
        /// </summary>
        TransactionSuccess,

        /// <summary>
        /// Indicates that a user session has started.
        /// </summary>
        SessionStart,

        /// <summary>
        /// Indicates that a user session has ended.
        /// </summary>
        SessionEnd,

        /// <summary>
        /// Indicates that notifications have been sent to users.
        /// </summary>
        NotificationSent,

        /// <summary>
        /// Indicates that permissions have been granted to a user.
        /// </summary>
        PermissionGranted,

        /// <summary>
        /// Indicates that permissions have been revoked from a user.
        /// </summary>
        PermissionRevoked,

        /// <summary>
        /// Indicates that data has been exported from the application.
        /// </summary>
        DataExport,

        /// <summary>
        /// Indicates that data has been imported into the application.
        /// </summary>
        DataImport,

        /// <summary>
        /// Indicates that a user has logged in using two-factor authentication.
        /// </summary>
        TwoFactorLogin,

        /// <summary>
        /// Indicates that a user has completed an onboarding process.
        /// </summary>
        OnboardingComplete,

        /// <summary>
        /// Indicates that a user has submitted feedback or a support ticket.
        /// </summary>
        FeedbackSubmitted,

        /// <summary>
        /// Indicates that a user has updated their notification preferences.
        /// </summary>
        NotificationPreferencesUpdated,

        /// <summary>
        /// Indicates that a user has requested account deletion.
        /// </summary>
        AccountDeletionRequested,

        /// <summary>
        /// Indicates that a user has accepted terms and conditions.
        /// </summary>
        TermsAccepted,

        /// <summary>
        /// Indicates that an administrator has made changes to system settings.
        /// </summary>
        SettingsUpdated,

        /// <summary>
        /// Indicates that a user has subscribed to a service.
        /// </summary>
        SubscriptionCreated,

        /// <summary>
        /// Indicates that a user has unsubscribed from a service.
        /// </summary>
        SubscriptionCanceled,

        /// <summary>
        /// Indicates that a customer has made an account inquiry.
        /// </summary>
        AccountInquiry,

        /// <summary>
        /// Indicates that a customer has requested a withdrawal.
        /// </summary>
        WithdrawalRequested,

        /// <summary>
        /// Indicates that a withdrawal has been processed.
        /// </summary>
        WithdrawalProcessed,

        /// <summary>
        /// Indicates that a customer has made a deposit.
        /// </summary>
        DepositMade,

        /// <summary>
        /// Indicates that a deposit has been processed.
        /// </summary>
        DepositProcessed,

        /// <summary>
        /// Indicates that a customer has requested a statement printout.
        /// </summary>
        StatementPrintRequested,

        /// <summary>
        /// Indicates that a checkbook has been ordered.
        /// </summary>
        CheckbookOrdered,


        /// <summary>
        /// Indicates that a loan application form has been provided to a customer.
        /// </summary>
        LoanApplicationFormProvided,

        /// <summary>
        /// Indicates that a customer has made a currency exchange.
        /// </summary>
        CurrencyExchangeMade,

        /// <summary>
        /// Indicates that a safe deposit box has been accessed.
        /// </summary>
        SafeDepositBoxAccessed,

        /// <summary>
        /// Indicates that a user has processed a new account opening request.
        /// </summary>
        NewAccountOpeningProcessed,

        /// <summary>
        /// Indicates that a user has updated a customer's contact information.
        /// </summary>
        CustomerContactInfoUpdated,

        /// <summary>
        /// Indicates that a user has provided financial advice to a customer.
        /// </summary>
        FinancialAdviceProvided,

        /// <summary>
        /// Indicates that a customer has made a request for a financial product.
        /// </summary>
        FinancialProductRequestMade,

        /// <summary>
        /// Indicates that a customer has made a payment on a loan or account.
        /// </summary>
        PaymentReceived,

        /// <summary>
        /// Indicates that a customer has submitted Countryation for verification.
        /// </summary>
        CountryationSubmitted,

        /// <summary>
        /// Indicates that a customer has inquired about a credit card application.
        /// </summary>
        CreditCardInquiry,

        /// <summary>
        /// Indicates that a credit card application has been submitted.
        /// </summary>
        CreditCardApplicationSubmitted,

        /// <summary>
        /// Indicates that a customer has requested a product demonstration.
        /// </summary>
        ProductDemonstrationRequested,

        /// <summary>
        /// Indicates that a customer has expressed interest in a new banking service.
        /// </summary>
        ServiceInterestExpressed,

        /// <summary>
        /// Indicates that a customer has filed a complaint.
        /// </summary>
        ComplaintFiled,

        /// <summary>
        /// Indicates that a customer has provided feedback on service.
        /// </summary>
        ServiceFeedbackReceived,

        /// <summary>
        /// Indicates that a user has escalated a customer issue to management.
        /// </summary>
        CustomerIssueEscalated,

        /// <summary>
        /// Indicates that a cash withdrawal operation has failed.
        /// </summary>
        CashWithdrawalFailed,

        /// <summary>
        /// Indicates that a cash deposit operation has failed.
        /// </summary>
        CashDepositFailed,

        /// <summary>
        /// Indicates that a cash transaction has been flagged for review.
        /// </summary>
        CashTransactionFlagged,

        /// <summary>
        /// Indicates that a fund transfer operation has failed.
        /// </summary>
        TransferFailed,

        /// <summary>
        /// Indicates that a fund transfer operation has succeeded.
        /// </summary>
        TransferSuccess,

        /// <summary>
        /// Indicates that a transaction has been reversed.
        /// </summary>
        Reversal,

        /// <summary>
        /// Indicates that a loan application has been submitted.
        /// </summary>
        LoanApplicationSubmitted,

        /// <summary>
        /// Indicates that a loan has been disbursed to a customer.
        /// </summary>
        LoanDisbursement,

        /// <summary>
        /// Indicates that a loan repayment has been made by a customer.
        /// </summary>
        LoanRepayment,

        /// <summary>
        /// Indicates that a loan has been approved.
        /// </summary>
        LoanApproved,

        /// <summary>
        /// Indicates that a loan application has been rejected.
        /// </summary>
        LoanApplicationRejected,

        /// <summary>
        /// Indicates that a loan has been extended.
        /// </summary>
        LoanExtended,

        /// <summary>
        /// Indicates that a loan has been rescheduled.
        /// </summary>
        LoanRescheduled,

        /// <summary>
        /// Indicates that a loan has been closed.
        /// </summary>
        LoanClosed,

        /// <summary>
        /// Indicates that a loan has been flagged for review.
        /// </summary>
        LoanFlagged,

        /// <summary>
        /// Indicates that a customer has requested information about their loan.
        /// </summary>
        LoanInquiryRequested,

        /// <summary>
        /// Indicates that a customer has received a loan statement.
        /// </summary>
        LoanStatementReceived
    }

    public enum LogLevelInfo
    {
        Information,   // General informational messages
        Warning,       // Potential issues or important events
        Error,         // Error events that might still allow the application to continue running
        Critical,      // Serious errors that require immediate attention
        Debug,         // Detailed information for debugging purposes
        Trace          // Fine-grained informational events for tracking the flow of the application
    }

}
```

## CBS.UserServiceManagement.Helper\Helper\PathHelper.cs

```cs
using CBS.UserServiceManagement.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CBS.UserServiceManagement.Helper
{
    public class PathHelper
    {
        public IConfiguration _configuration;

        public PathHelper(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        #region Initial system pathelper

        public string CountryPath
        {
            get
            {
                return _configuration["CountryPath"];
            }
        }

        public string SMSAPIBaseEdnPoint
        {
            get
            {
                return _configuration["SMSSettings:SMSAPIBaseEdnPoint"];
            }
        }

        public string KYCAPIBaseEdnPoint
        {
            get
            {
                return _configuration["KYCSettings:KYCAPIBaseEdnPoint"];
            }
        }

        public string KYCEndPointURL
        {
            get
            {
                return _configuration["KYCSettings:KYCEndPointURL"];
            }
        }

        public string SubscriptionBaseEndPoint
        {
            get
            {
                return _configuration["SubcriptionSettings:SubscriptionBaseEndPoint"];
            }
        }

        public string SubscriptionURL
        {
            get
            {
                return _configuration["SubcriptionSettings:SubscriptionURL"];
            }
        }

        public string SubscriptionCallBackURL
        {
            get
            {
                return _configuration["SubcriptionSettings:SubscriptionCallBackURL"];
            }
        }

        public string ComapanyName
        {
            get
            {
                return _configuration["SMSSettings:ComapanyName"];
            }
        }

        public string SpUserName
        {
            get
            {
                return _configuration["BankSettings:SpUserName"];
            }
        }

        public string URLToCompleteSubscription
        {
            get
            {
                return _configuration["WebSIteURL:URLToCompleteSubscription"];
            }
        }

        public string UserName
        {
            get
            {
                return _configuration["Authentication:UserName"];
            }
        }

        public string Password
        {
            get
            {
                return _configuration["Authentication:Password"];
            }
        }

        public string IdentityServerBaseUrl
        {
            get
            {
                return _configuration["Authentication:IdentityServerBaseUrl"];
            }
        }
        public string FileUploadEndpointURL
        {
            get
            {
                return _configuration["FileUploadServer:IdentityServerBaseUrl"] + _configuration["FileUploadServer:FileUpload"];
            }
        }
 

        public string WebSIteURL
        {
            get
            {
                return _configuration["WebSiteSettings:WebSIteURL"];
            }
        }

        public int SubscriptionAmount
        {
            get
            {
                return Convert.ToInt32(_configuration["BankSettings:SubscriptionAmount"]);
            }
        }

        public string SpPassword
        {
            get
            {
                return _configuration["BankSettings:SpPassword"];
            }
        }

        public string SendSMSURL
        {
            get
            {
                return _configuration["SMSSettings:SendSMS"];
            }
        }

        public string SMSSenderName
        {
            get
            {
                return _configuration["SMSSettings:SMSSenderName"];
            }
        }

        public string UserProfilePath
        {
            get
            {
                return _configuration["UserProfilePath"];
            }
        }

        public string UserIDCardFrontSidePath
        {
            get
            {
                return _configuration["UserIDCardFrontSidePath"];
            }
        }

        public string UserIDCardBackSidePath
        {
            get
            {
                return _configuration["UserIDCardBackSidePath"];
            }
        }

        public string OpenAPIServiceType
        {
            get
            {
                return _configuration["OpenAPISettings:OpenAPIServiceType"];
            }
        }

        public string OpenAPIBaseEdnPoint
        {
            get
            {
                return _configuration["OpenAPISettings:OpenAPIBaseEdnPoint"];
            }
        }

        public string OpenAPISubscriptionURL
        {
            get
            {
                return _configuration["OpenAPISettings:OpenAPISubscriptionURL"];
            }
        }

        public string OpenAPISubscriptionCallBackURL
        {
            get
            {
                return _configuration["OpenAPISettings:OpenAPISubscriptionCallBackURL"];
            }
        }

        public string OpenAPIBankID
        {
            get
            {
                return _configuration["OpenAPISettings:OpenAPIBankID"];
            }
        }

        public string LoggerPath
        {
            get
            {
                return _configuration["LoogerSetting:LoggerPath"];
            }
        }
        #endregion

        /// <summary>
        /// Endpoint to read client reference information during the creation of customer account.
        /// </summary>
        public string CustomerManagement_GetCustomerReferenceInfo
        {
            get
            {
                return _configuration["CustomerManagement:GetCustomerReferenceInfoEndpoint"];
            }
        }
        /// <summary>
        /// Endpoint to get branch information by BrancId.
        /// </summary>
        public string BankManagement_GetBranchInfoEndPoint
        {
            get
            {
                var model =  _configuration["BankManagement:GetBranchInfoEndpoint"];
                return model;
            }
        }
        public string BankManagement_BankCode
        {
            get
            {
                var model = _configuration["BankManagement:BankCode"];
                return model;
            }
        }
  
        /// <summary>
        /// Endpoint to get branch information by BrancId.
        /// </summary>
        public string BankManagementBaseUrl
        {
            get
            {
                var model = _configuration["BankManagement:BaseUrl"] ;
                return model;
            }
        }
        /// <summary>
        /// Endpoint to get all branch information.
        /// </summary>
        public string BankManagement_GetAllBranch
        {
            get
            {
                var model = _configuration["BankManagement:BaseUrl"] + _configuration["BankManagement:GetAllBranch"];
                return model;
            }
        }
        public string TransactionManagement_GetAllTransaction
        {
            get
            {
                var model = _configuration["TransactionManagement:BaseUrl"] + _configuration["TransactionManagement:GetAllTransactionInfoEndpoint"];
                return model;
            }
        }
        public string BankManagement_GetAllBranchInfo
        {
            get
            {
                var model = _configuration["BankManagement:BaseUrl"] + _configuration["BankManagement:GetAllBranchInfoEndpoint"];
                return model;
            }
        }
        public string BankManagement_GetAllBranchInfoGWl
        {
            get
            {
                var model = _configuration["BankManagement:BaseUrl"] + _configuration["BankManagement:GetBranchEndpoint"];
                return model;
            }
        }
        /// <summary>
        /// Endpoint to read transaction Call back end point during cashreplenishment request.
        /// </summary>
        public string TellerProvisioning_EndPoint
        {
            get
            {
                var model = _configuration["TellerProvisioning:BaseUrl"] + _configuration["TellerProvisioning:GetPrimaryEndpoint"];
                return model;
            }
        }
        public string TransferFromTellerToNonMember_EventAttributIdForTransit
        {
            get
            {
                var model = _configuration["TransferFromTellerToNonMember:EventAttributIdForTransit"];
                return model;
            }
        }
        public string TransferFromTellerToNonMember_EventAttributIdForRevenue
        {
            get
            {
                var model = _configuration["TransferFromTellerToNonMember:EventAttributIdForRevenue"];
                return model;
            }
        }

        public string TransferFromTellerToNonMember_EventAttributIdForCashInHand
        {
            get
            {
                var model = _configuration["TransferFromTellerToNonMember:EventAttributIdForCashInHand"];
                return model;
            }
        }
        public string TransferFromMemberToNonMember_EventAttributIdForRevenue
        {
            get
            {
                var model = _configuration["TransferFromTellerToNonMember:EventAttributIdForRevenue"];
                return model;
            }
        }
        

        public string TransferFromMemberToNonMember_EventAttributIdForTransit
        {
            get
            {
                var model = _configuration["TransferFromTellerToNonMember:EventAttributIdForTransit"];
                return model;
            }
        }
        public string WithdrawalTransferFundToNonMemberViaTeller_EventAttributIdForTransit
        {
            get
            {
                var model = _configuration["WithdrawalTransferFundToNonMemberViaTeller:EventAttributIdForTransit"];
                return model;
            }
        }
        public string WithdrawalTransferFundToNonMemberViaTeller_EventAttributIdForCashInHand
        {
            get
            {
                var model = _configuration["WithdrawalTransferFundToNonMemberViaTeller:EventAttributIdForCashInHand"];
                return model;
            }
        }

        /// <summary>
        /// Endpoint to read client reference information during the creation of customer account.
        /// </summary>
        public string FileUpload_MigrationPath
        {
            get
            {
                return _configuration["FileUpload:MigrationPath"];
            }
        }


        /// <summary>
        /// Endpoint to read client reference information during the creation of customer account.
        /// </summary>
        public string FileUpload_MigrationValidatedPath
        {
            get
            {
                return _configuration["FileUpload:MigrationValidated"];
            }
        }
        /// <summary>
        /// Endpoint to read client reference information during the creation of customer account.
        /// </summary>
        public string DomainName
        {
            get
            {
                return _configuration["FileUpload:DomainName"];
            }
        }


        public string BankTransactionCashINAndCashOutURL
        {
            get
            {
                ///
                var model = _configuration["Transaction:BankTransactionCashINAndCashOutURL"];
                return model;
            }
        }

    }
}
```

## CBS.UserServiceManagement.Helper\Helper\ServiceResponse.cs

```cs
using CBS.UserServiceManagement.Data;

namespace CBS.UserServiceManagement.Helper
{
    public class ServiceResponse<T>  
    {
        public T Data { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        // Constructor for a successful response with default values.
        private ServiceResponse(T data)
        {
            Data = data;

        }

        private ServiceResponse()
        {
        }

        // Constructor for a failed response with status code and error messages.
        private ServiceResponse(int statusCode, List<string> errors)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
      
        // Constructor for a failed response with status code and error messages.
        /// <summary>
        /// Initializes a new instance of the ServiceResponse class with the specified status code and data payload.
        /// </summary>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <param name="data">The data payload of the response.</param>
        private ServiceResponse(int statusCode, T data)
        {
            StatusCode = statusCode;
            Data = data;
        }

        // Constructor for a failed response with status code and a single error message.
        private ServiceResponse(int statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            Errors = new List<string> { errorMessage };
        }

        // Constructor for a successful response with custom message.
        private ServiceResponse(int statusCode, T data, string message)
        {
            StatusCode = statusCode;
            Data = data;
            Message = message;
        }

        // Constructor for a successful response with custom message, description, and status.
        private ServiceResponse(int statusCode, T data, string message, string description, string status)
        {
            StatusCode = statusCode;
            Data = data;
            Message = message;
            Description = description;
            Status = status;
        }

        // Constructor for a response with custom status code, message, description, and status.
        private ServiceResponse(int statusCode, string message, string description, string status)
        {
            StatusCode = statusCode;
            Message = message;
            Description = description;
            Status = status;
        }

        // Checks if the response is successful (no errors).
        public bool Success => Errors == null || Errors.Count == 0;

        // Factory method to create a response for an exception.
        public static ServiceResponse<T> ReturnException(Exception ex)
        {
            return new ServiceResponse<T>(500, $"An unexpected fault happened. Error: {ex.Message}", "Failed with Internal Server Error", "FAILED");
        }

        // Factory method to create a response for a failed operation with status code and errors.
        public static ServiceResponse<T> ReturnFailed(int statusCode, List<string> errors)
        {
            return new ServiceResponse<T>(statusCode, errors);
        }

        // Factory method to create a response for a failed operation with a single error message.
        public static ServiceResponse<T> ReturnFailed(int statusCode, string errorMessage)
        {
            return new ServiceResponse<T>(statusCode, errorMessage);
        }

        // Factory method to create a success response with default values.
        public static ServiceResponse<T> ReturnSuccess()
        {
            return new ServiceResponse<T>(default(T));
        }

        // Factory method to create a success response with a custom message.
        public static ServiceResponse<T> ReturnSuccess(string message)
        {
            return new ServiceResponse<T>(200, message, "Transaction was successful", "SUCCESS");
        }

        // Factory method to create a success response with status code and data.
        public static ServiceResponse<T> ReturnResultWith200(T data)
        {
            try
            {
                return new ServiceResponse<T>(200, data, string.Empty, "Transaction was successful", "SUCCESS");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static ServiceResponse<T> ReturnResultWith200(T data, string message)
        {
            return new ServiceResponse<T>(200, data, message, "Transaction was successful", "SUCCESS");
        }

        // Factory method to create a success response with status code, data, and a custom message.
        public static ServiceResponse<T> ReturnResultWith201(T data, string message)
        {
            return new ServiceResponse<T>(201, data, message, "Transaction was successful", "SUCCESS");
        }

        // Factory method to create a success response with a status code and no data.
        public static ServiceResponse<T> ReturnResultWith204(T data)
        {
            return new ServiceResponse<T>(204, data, string.Empty, "Transaction was successful", "SUCCESS");
        }

        // Factory method to create a 500 Internal Server Error response.
        public static ServiceResponse<T> Return500()
        {
            return new ServiceResponse<T>(500, "An unexpected fault happened. Try again later.");
        }
      

        // Factory method to create a 500 Internal Server Error response with an exception.
        public static ServiceResponse<T> Return500(Exception ex, string message = null)
        {
            return new ServiceResponse<T>(500, $"An unexpected fault happened. {message} Error: {ex.Message}", "Failed with Internal Server Error", "FAILED");
        }

        public static ServiceResponse<T> Return500(string message)
        {
            return new ServiceResponse<T>(500, $"{message}", "Failed with Internal Server Error", "FAILED");
        }

      
        // Factory method to create a 409 Conflict response with a custom message.
        public static ServiceResponse<T> Return409(string message)
        {
            return new ServiceResponse<T>(409, message, "Failed, Record already exists. Operation was cancelled", "FAILED");
        }

        // Factory method to create a 409 Conflict response with a default message.
        public static ServiceResponse<T> Return409()
        {
            return new ServiceResponse<T>(409, "Record already exists", "Failed, Record already exists. Operation was cancelled", "FAILED");
        }

        // Factory method to create a 422 Unprocessable Entity response with a custom message.
        public static ServiceResponse<T> Return422(string message)
        {
            return new ServiceResponse<T>(422, new List<string> { "Unprocessable Entity", message, "Failed. Process terminated.", "FAILED"});
        }

        // Factory method to create a 404 Not Found response with a default message.
        public static ServiceResponse<T> Return404()
        {
            return new ServiceResponse<T>(404, "Record not found", "Failed. Process terminated.", "FAILED");
        }
       
        // Factory method to create a 404 Not Found response with a custom message.
        public static ServiceResponse<T> Return404(string message)
        {
            return new ServiceResponse<T>(404, message, "Failed. Process terminated.", "FAILED");
        }
        // Factory method to create a 401 Not Authourized response with a custom message.
        public static ServiceResponse<T> Return401(string message)
        {
            return new ServiceResponse<T>(401, message, "Failed.User not authourized Process terminated.", "FAILED");
        }
        // Factory method to create a Forbiden response with a custom message.
        public static ServiceResponse<T> Return403(string message)
        {
            return new ServiceResponse<T>(403, message, "Operation is forbidden. Process terminated.", "FAILED");
        }

        public static ServiceResponse<T> Return403(T data)
        {
            return new ServiceResponse<T>(403, "Operation is forbidden.", "Failed. Process terminated.", "FAILED");
        }

        // Factory method to create a Forbiden response with a custom message.
        public static ServiceResponse<T> Return403()
        {
            return new ServiceResponse<T>(403, "Operation is forbidden.", "Failed. Process terminated.", "FAILED");
        }

        public static ServiceResponse<T> Return403(T data, string message)
        {
            return new ServiceResponse<T>(403, data, message, "Operation is forbidden.", "FAILED");
        }

       
        
    }

    // Represents a generic response object with data, errors, and status information.
    public class ResponseObject
    {
        public object Data { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; } = 200;
        public string StatusDescription { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }

        // Constructor for creating a response object with optional parameters.
        public ResponseObject(object data = null, int statusCode = 200, string description = null, string message = null, string status = null, List<string> errors = null)
        {
            Data = data;
            StatusCode = statusCode;
            StatusDescription = description;
            Status = status;
            Message = message;
            Errors = errors;
        }
    }
}
```

## CBS.UserServiceManagement.MediatR\CBS.UserServiceManagement.MediatR.csproj

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="FluentValidation" Version="11.9.0" />
    <PackageReference Include="MediatR" Version="12.2.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Domain\CBS.UserServiceManagement.Domain.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Helper\CBS.UserServiceManagement.Helper.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Repository\CBS.UserServiceManagement.Repository.csproj" />
  </ItemGroup>

</Project>

```

## CBS.UserServiceManagement.MediatR\User\Commands\AddUserCommand.cs

```cs
using MediatR;
using System;

namespace CBS.UserServiceManagement.MediatR.User.Commands
{
    public class AddUserCommand : IRequest<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
    }
}

```

## CBS.UserServiceManagement.MediatR\User\Commands\LoginUserCommand.cs

```cs
using MediatR;

namespace CBS.UserServiceManagement.MediatR.User.Commands
{
    public class LoginUserCommand : IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

```

## CBS.UserServiceManagement.MediatR\User\Handlers\AddUserCommandHandler.cs

```cs
using System;
using System.Threading;
using System.Threading.Tasks;
using CBS.UserServiceManagement.Common.UnitOfWork;
using CBS.UserServiceManagement.Data.Entity;
using CBS.UserServiceManagement.Domain.Context;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.UserServiceManagement.MediatR.User.Handlers
{
    public class AddUserCommandHandler : IRequestHandler<Commands.AddUserCommand, Guid>
    {
        private readonly IUnitOfWork<UserContext> _uow;
        private readonly ILogger<AddUserCommandHandler> _logger;

        public AddUserCommandHandler(
            IUnitOfWork<UserContext> uow,
            ILogger<AddUserCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Guid> Handle(Commands.AddUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password, // Note: Le mot de passe doit Ãªtre hashÃ© avant d'Ãªtre stockÃ©
                request.Role);

            await _uow.Context.Users.AddAsync(user, cancellationToken);
            await _uow.SaveAsync(cancellationToken);

            _logger.LogInformation($"User created with ID: {user.Id}");
            return user.Id;
        }
    }
}

```

## CBS.UserServiceManagement.MediatR\User\Handlers\GetAllUsersQueryHandler.cs

```cs
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CBS.UserServiceManagement.Common.UnitOfWork;
using CBS.UserServiceManagement.Data.Dto;
using CBS.UserServiceManagement.Domain.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.UserServiceManagement.MediatR.User.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<Queries.GetAllUsersQuery, List<UserDto>>
    {
        private readonly IUnitOfWork<UserContext> _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(
            IUnitOfWork<UserContext> uow,
            IMapper mapper,
            ILogger<GetAllUsersQueryHandler> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<UserDto>> Handle(Queries.GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _uow.Context.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .ToListAsync(cancellationToken);

            _logger.LogInformation($"Retrieved {users.Count} users");
            return _mapper.Map<List<UserDto>>(users);
        }
    }
}

```

## CBS.UserServiceManagement.MediatR\User\Handlers\GetUserByIdQueryHandler.cs

```cs
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CBS.UserServiceManagement.Common.UnitOfWork;
using CBS.UserServiceManagement.Data.Dto;
using CBS.UserServiceManagement.Domain.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.UserServiceManagement.MediatR.User.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<Queries.GetUserByIdQuery, UserDto>
    {
        private readonly IUnitOfWork<UserContext> _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(
            IUnitOfWork<UserContext> uow,
            IMapper mapper,
            ILogger<GetUserByIdQueryHandler> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto> Handle(Queries.GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _uow.Context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning($"User not found with ID: {request.Id}");
                return null;
            }

            _logger.LogInformation($"Retrieved user with ID: {request.Id}");
            return _mapper.Map<UserDto>(user);
        }
    }
}

```

## CBS.UserServiceManagement.MediatR\User\Handlers\LoginUserCommandHandler.cs

```cs
using System;
using System.Threading;
using System.Threading.Tasks;
using CBS.UserServiceManagement.Common.UnitOfWork;
using CBS.UserServiceManagement.Data.Entity;
using CBS.UserServiceManagement.Domain.Context;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.UserServiceManagement.MediatR.User.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<Commands.LoginUserCommand, Commands.LoginResponse>
    {
        private readonly IUnitOfWork<UserContext> _uow;
        private readonly ILogger<LoginUserCommandHandler> _logger;

        public LoginUserCommandHandler(
            IUnitOfWork<UserContext> uow,
            ILogger<LoginUserCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Commands.LoginResponse> Handle(Commands.LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _uow.Context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning($"Login attempt failed for email: {request.Email} - User not found");
                return new Commands.LoginResponse { Success = false, Message = "Invalid credentials" };
            }

            // VÃ©rifiez le mot de passe (utilisez une bibliothÃ¨que de hachage sÃ©curisÃ©e)
            // var passwordValid = VerifyPasswordHash(request.Password, user.PasswordHash);
            // if (!passwordValid)
            // {
            //     return new Commands.LoginResponse { Success = false, Message = "Invalid credentials" };
            // }

            // GÃ©nÃ©rez un jeton JWT ici
            // var token = GenerateJwtToken(user);
            // var refreshToken = GenerateRefreshToken();

            _logger.LogInformation($"User logged in: {user.Email}");
            
            return new Commands.LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Token = "jwt-token-here",
                RefreshToken = "refresh-token-here"
            };
        }
    }
}

```

## CBS.UserServiceManagement.MediatR\User\Queries\GetAllUsersQuery.cs

```cs
using MediatR;
using System.Collections.Generic;

namespace CBS.UserServiceManagement.MediatR.User.Queries
{
    public class GetAllUsersQuery : IRequest<List<UserDto>>
    {
        // Vous pouvez ajouter des paramÃ¨tres de pagination ou de filtrage ici si nÃ©cessaire
    }
}

```

## CBS.UserServiceManagement.MediatR\User\Queries\GetUserByIdQuery.cs

```cs
using MediatR;
using System;

namespace CBS.UserServiceManagement.MediatR.User.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public Guid Id { get; set; }
    }
}

```

## CBS.UserServiceManagement.MediatR\User\Validators\AddUserCommandValidator.cs

```cs
using FluentValidation;
using CBS.UserServiceManagement.MediatR.User.Commands;

namespace CBS.UserServiceManagement.MediatR.User.Validators
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Le prÃ©nom est requis")
                .MaximumLength(100).WithMessage("Le prÃ©nom ne peut pas dÃ©passer 100 caractÃ¨res");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Le nom est requis")
                .MaximumLength(100).WithMessage("Le nom ne peut pas dÃ©passer 100 caractÃ¨res");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est requis")
                .EmailAddress().WithMessage("Un email valide est requis")
                .MaximumLength(150).WithMessage("L'email ne peut pas dÃ©passer 150 caractÃ¨res");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est requis")
                .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractÃ¨res")
                .Matches(@"[A-Z]+").WithMessage("Le mot de passe doit contenir au moins une majuscule")
                .Matches(@"[a-z]+").WithMessage("Le mot de passe doit contenir au moins une minuscule")
                .Matches(@"[0-9]+").WithMessage("Le mot de passe doit contenir au moins un chiffre");
        }
    }
}

```

## CBS.UserServiceManagement.MediatR\User\Validators\LoginUserCommandValidator.cs

```cs
using FluentValidation;

namespace CBS.UserServiceManagement.MediatR.User.Validators
{
    public class LoginUserCommandValidator : AbstractValidator<Commands.LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est requis")
                .EmailAddress().WithMessage("Un email valide est requis");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est requis");
        }
    }
}

```

## CBS.UserServiceManagement.Repository\CBS.UserServiceManagement.Repository.csproj

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\CBS.UserServiceManagement.Common\CBS.UserServiceManagement.Common.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Domain\CBS.UserServiceManagement.Domain.csproj" />
  </ItemGroup>

</Project>

```

## CBS.UserServiceManagement.Repository\User\IUserRepository.cs

```cs
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
       
    }
}

```

## CBS.UserServiceManagement.Repository\User\UserRepository.cs

```cs
// Source: Votre modÃ¨le CountryRepository appliquÃ© Ã  notre contexte.
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;


namespace CBS.UserServiceManagement.Repository
{
    public class UserRepository : GenericRepository<User, UserContext>, IUserRepository
    {
        public UserRepository(IUnitOfWork<UserContext> unitOfWork) : base(unitOfWork)
        {
            // LE CONSTRUCTEUR EST VIDE, COMME DANS LE MODÃˆLE.
            // Toute la logique de base est gÃ©rÃ©e par la classe GenericRepository.
        }
    }
}
```

```

## Écarts par Rapport aux Standards

### Couche Repository
- **État Actuel** : IUserRepository et UserRepository dans sous-dossier User/
- **Standard** : Doivent être à la racine de la couche Repository

### Dependency Injection
- **État Actuel** : Configuration sur assembly API
- **Standard** : Doit pointer vers assembly MediatR (AddUserCommand)

### Program.cs/Startup.cs
- **État Actuel** : Configuration basique
- **Standard** : Doit inclure logging robuste, try-catch et pipeline middleware complet
