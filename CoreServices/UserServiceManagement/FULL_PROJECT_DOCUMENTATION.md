# Documentation Complète du Projet UserServiceManagement

## Table des Matières

- [Couche API](#couche-api)
- [Couche Domaine](#couche-domaine)
- [Couche Data](#couche-data)
- [Helpers](#helpers)
- [MediatR](#mediatr)
- [Repository](#repository)
- [Documentation](#documentation)
- [Conclusion](#conclusion)

## Couche API

### UsersController.cs
```csharp
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.MediatR;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CBS.UserServiceManagement.API.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser([FromBody] AddUserCommand command)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var responseObject = new ResponseObject(
                    data: null, 
                    statusCode: 400, 
                    description: "Invalid model state. Please check the provided data.", 
                    message: "Invalid request", 
                    status: "FAILED", 
                    errors: errors
                );
                return BadRequest(responseObject);
            }

            _logger.LogInformation("Tentative d'ajout d'un nouvel utilisateur");
            
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                _logger.LogWarning("Échec de l'ajout d'utilisateur: {Message}", result.Message);
            
            return ReturnFormattedResponse(result);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var responseObject = new ResponseObject(
                    data: null, 
                    statusCode: 400, 
                    description: "Invalid model state", 
                    message: "Invalid request", 
                    status: "FAILED", 
                    errors: errors
                );
                return BadRequest(responseObject);
            }

            _logger.LogInformation("Tentative de connexion pour l'email {Email}", command.Email);
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
```

### Startup.cs
```csharp
// --- USINGS NÉCESSAIRES ---
using CBS.UserServiceManagement.API.Helpers;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.API.Helpers.DependencyResolver;
using CBS.UserServiceManagement.API.Middlewares;
using CBS.UserServiceManagement.API.Middlewares.JwtValidator;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using CBS.UserServiceManagement.API.Middlewares.ExceptionHandlingMiddleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace CBS.UserServiceManagement.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration des services...
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configuration du pipeline...
        }
    }
}
```

### Program.cs
```csharp
using CBS.UserServiceManagement.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System;

namespace CBS.UserServiceManagement.API
{
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
                logger.Error(exception, "Arrêt du programme en raison d'une exception");
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
}
```

### JwtAuthenticationConfigurationExtension.cs
```csharp
using CBS.UserServiceManagement.API.Helpers;
using CBS.UserServiceManagement.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.API.Middlewares.JwtValidator
{
    public static class JwtAuthenticationConfigurationExtension
    {
        public static void AddJwtAuthenticationConfiguration(this IServiceCollection services, JwtSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "JwtSettings configuration is missing");
            }

            if (string.IsNullOrWhiteSpace(settings.Key))
            {
                throw new ArgumentException("JWT Key is missing in configuration. Please check appsettings.json", nameof(settings.Key));
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(settings.MinutesToExpiration)
                };
                jwtBearerOptions.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.SecurityToken is JwtSecurityToken accessToken)
                        {
                            var userInfoToken = context.HttpContext.RequestServices.GetRequiredService<UserInfoToken>();
                            userInfoToken.Id = accessToken.Claims.FirstOrDefault(a => a.Type == "Id")?.Value;
                            userInfoToken.Email = accessToken.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
                            userInfoToken.Role = accessToken.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role)?.Value;
                            userInfoToken.Token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                            
                            context.HttpContext.Items["UserId"] = userInfoToken.Id;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();
        }
    }
}

## Couche Domain

### UserContext.cs
```csharp
using CBS.UserServiceManagement.Data;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<AuditLog> AuditLogs { get; set; }
        
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

### Migrations

Les migrations Entity Framework Core sont stockées dans le dossier Migrations et contiennent l'historique des modifications du schéma de base de données. Voici un exemple de migration typique :

```csharp
// Exemple de migration (nom et contenu variables selon les besoins)
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                FirstName = table.Column<string>(maxLength: 100, nullable: false),
                LastName = table.Column<string>(maxLength: 100, nullable: false),
                Email = table.Column<string>(maxLength: 150, nullable: false),
                Role = table.Column<string>(maxLength: 50, nullable: false),
                PasswordHash = table.Column<byte[]>(nullable: false),
                PasswordSalt = table.Column<byte[]>(nullable: false),
                CreatedDate = table.Column<DateTime>(nullable: false),
                ModifiedDate = table.Column<DateTime>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Users");
    }
}

## Couche Data

### User.cs (Entity)
```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace CBS.UserServiceManagement.Data
{
    public class User : BaseEntity
    {
        [Key]
        public string Id { get; set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }

        private User() { }

        public User(string id, string firstName, string lastName, string email, string passwordHash, string role = "User")
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }

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

### UserDto.cs (DTO)
```csharp
using System;

namespace CBS.UserServiceManagement.Data
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

### UserRole.cs (Enum)
```csharp
namespace CBS.UserServiceManagement.Data
{
    public enum UserRole
    {
        User,
        Admin,
        Manager
    }
}

## Couche MediatR

### AddUserCommand.cs
```csharp
using CBS.UserServiceManagement.Data;
using MediatR;

namespace CBS.UserServiceManagement.MediatR.User.Commands
{
    public class AddUserCommand : IRequest<ServiceResponse<UserDto>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
```

### AddUserCommandHandler.cs
```csharp
using AutoMapper;
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.MediatR;
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BCrypt.Net;

namespace CBS.UserServiceManagement.MediatR
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, ServiceResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork<UserContext> _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<AddUserCommandHandler> _logger;

        public AddUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork<UserContext> uow,
            IMapper mapper,
            ILogger<AddUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserDto>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var emailExists = await _userRepository.FindBy(u => u.Email.ToLower() == request.Email.ToLower()).AnyAsync(cancellationToken);
                if (emailExists)
                {
                    var errorMessage = $"A user with the email '{request.Email}' already exists.";
                    _logger.LogWarning(errorMessage);
                    return ServiceResponse<UserDto>.Return409(errorMessage);
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var newUserId = BaseUtilities.GenerateUniqueNumber(36);

                var newUser = new User(
                    id: newUserId,
                    firstName: request.FirstName,
                    lastName: request.LastName,
                    email: request.Email,
                    passwordHash: passwordHash,
                    role: "User"
                );
                
                _userRepository.Add(newUser);
                await _uow.SaveAsync();

                var userDto = _mapper.Map<UserDto>(newUser);

                _logger.LogInformation($"Successfully created user with ID {newUser.Id}.");
                return ServiceResponse<UserDto>.ReturnResultWith201(userDto, "User created successfully.");
            }
            catch (Exception ex)
            {
                var errorMessage = "An unexpected error occurred while creating the user.";
                _logger.LogError(ex, errorMessage);
                return ServiceResponse<UserDto>.Return500(ex);
            }
        }
    }
}
```

### AddUserCommandValidator.cs
```csharp
using FluentValidation;
using CBS.UserServiceManagement.MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Le prénom est requis.")
                .MaximumLength(100).WithMessage("Le prénom ne peut pas dépasser 100 caractères.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Le nom est requis.")
                .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est requis.")
                .EmailAddress().WithMessage("Un format d'email valide est requis.")
                .MaximumLength(150).WithMessage("L'email ne peut pas dépasser 150 caractères.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est requis.")
                .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères.")
                .Matches(@"[A-Z]").WithMessage("Le mot de passe doit contenir au moins une lettre majuscule.")
                .Matches(@"[a-z]").WithMessage("Le mot de passe doit contenir au moins une lettre minuscule.")
                .Matches(@"[0-9]").WithMessage("Le mot de passe doit contenir au moins un chiffre.");
        }
    }
}

### LoginUserCommand.cs
```csharp
using MediatR;

namespace CBS.UserServiceManagement.MediatR.User.Commands
{
    public class LoginUserCommand : IRequest<ServiceResponse<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
```

### LoginUserCommandHandler.cs
```csharp
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using BCrypt.Net;

namespace CBS.UserServiceManagement.MediatR.User.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ServiceResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<LoginUserCommandHandler> _logger;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<LoginUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        public async Task<ServiceResponse<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.FindBy(u => u.Email == request.Email).FirstOrDefaultAsync(cancellationToken);
                
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Tentative de connexion échouée pour l'email {Email}", request.Email);
                    return ServiceResponse<string>.Return401("Email ou mot de passe incorrect");
                }

                var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Role);
                
                _logger.LogInformation("Connexion réussie pour l'utilisateur {Email}", user.Email);
                return ServiceResponse<string>.Return200(token, "Connexion réussie");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la tentative de connexion");
                return ServiceResponse<string>.Return500(ex);
            }
        }
    }
}
```

### LoginUserCommandValidator.cs
```csharp
using FluentValidation;

namespace CBS.UserServiceManagement.MediatR.User.Validators
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est requis")
                .EmailAddress().WithMessage("Format d'email invalide");
                
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est requis");
        }
    }
}

## Couche Repository

### IUserRepository.cs
```csharp
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

### UserRepository.cs
```csharp
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;

namespace CBS.UserServiceManagement.Repository
{
    public class UserRepository : GenericRepository<User, UserContext>, IUserRepository
    {
        public UserRepository(IUnitOfWork<UserContext> unitOfWork) : base(unitOfWork)
        {
            // Toute la logique de base est gérée par la classe GenericRepository
        }
    }
}
```

### GenericRepository.cs
```csharp
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.Common.GenericRespository
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        protected readonly IUnitOfWork<TContext> _uow;

        public GenericRepository(IUnitOfWork<TContext> unitOfWork)
        {
            _uow = unitOfWork;
        }

        public virtual void Add(TEntity entity)
        {
            _uow.Context.Set<TEntity>().Add(entity);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await _uow.Context.Set<TEntity>().FindAsync(id);
        }

        public virtual IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _uow.Context.Set<TEntity>().Where(predicate);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _uow.Context.Set<TEntity>();
        }

        public virtual void Update(TEntity entity)
        {
            _uow.Context.Set<TEntity>().Update(entity);
        }

        public virtual void Delete(object id)
        {
            var entity = _uow.Context.Set<TEntity>().Find(id);
            if (entity != null)
            {
                _uow.Context.Set<TEntity>().Remove(entity);
            }
        }
    }
}

## Conclusion

Cette documentation complète présente l'ensemble du code source du projet UserServiceManagement, structuré par couches :

1. **Couche API** : Contrôleurs, middlewares et configuration
2. **Couche Domain** : Contexte EF Core et migrations
3. **Couche Data** : Entités, DTOs et enums
4. **Couche MediatR** : Commandes, handlers et validateurs
5. **Couche Repository** : Implémentation de l'accès aux données

### Architecture globale

Le projet suit une architecture hexagonale moderne avec :
- Séparation claire des responsabilités
- Pattern CQRS via MediatR
- Injection de dépendances
- Gestion centralisée des erreurs
- Journalisation complète
- Sécurité JWT robuste

### Bonnes pratiques implémentées

1. **Sécurité** :
   - Authentification JWT
   - Hachage des mots de passe (BCrypt)
   - Validation des inputs
   - Gestion fine des rôles

2. **Qualité de code** :
   - Validation FluentValidation
   - Logging systématique
   - Gestion centralisée des erreurs
   - Tests unitaires (non montrés ici)

3. **Maintenabilité** :
   - Architecture en couches
   - Repository pattern
   - DTOs pour l'isolation
   - Documentation complète

Cette documentation peut être utilisée comme référence pour le développement futur, l'audit de code ou la formation de nouveaux développeurs.
