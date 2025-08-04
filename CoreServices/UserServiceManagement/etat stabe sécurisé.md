CoreServices/UserServiceManagement/
│
├── 📁 CBS.UserServiceManagement.API/                # Couche API
│   ├── 📁 Controllers/                             # Contrôleurs
│   │   ├── BaseController.cs
│   │   └── UsersController.cs
│   ├── 📁 Helpers/                                 # Utilitaires
│   │   ├── 📁 DependencyResolver/
│   │   │   └── DependencyInjectionExtension.cs
│   │   ├── 📁 MapperConfiguration/
│   │   │   └── MapperConfig.cs
│   │   ├── 📁 MappingProfile/
│   │   │   └── UserMappingProfile.cs
│   │   └── JwtSettings.cs
│   ├── 📁 Middlewares/                             # Middlewares
│   │   ├── 📁 AuditLogMiddleware/
│   │   │   └── AuditLogMiddleware.cs
│   │   ├── 📁 ExceptionHandlingMiddleware/
│   │   │   └── ExceptionHandlingMiddleware.cs
│   │   ├── 📁 JwtValidator/
│   │   │   └── JwtAuthenticationConfigurationExtension.cs
│   │   ├── 📁 LoggingMiddleware/
│   │   │   └── RequestResponseLoggingMiddleware.cs
│   │   └── 📁 SecurityHeadersMiddleware/
│   │       └── SecurityHeadersMiddleware.cs
│   ├── 📁 Properties/
│   │   └── launchSettings.json
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── nlog.config
│   ├── Program.cs
│   └── Startup.cs
│
├── 📁 CBS.UserServiceManagement.Common/             # Couche commune
│   ├── 📁 GenericRespository/
│   │   ├── GenericRespository.cs
│   │   └── IGenericRepository.cs
│   └── 📁 UnitOfWork/
│       ├── IUnitOfWork.cs
│       └── UnitOfWork.cs
│
├── 📁 CBS.UserServiceManagement.Data/               # Couche données
│   ├── 📁 Dto/                                     # DTOs
│   │   ├── UserDto.cs
│   │   └── ...
│   ├── 📁 Entity/                                  # Entités
│   │   ├── User.cs
│   │   └── ...
│   └── 📁 Enum/                                    # Enums
│       ├── UserRole.cs
│       └── ...
│
├── 📁 CBS.UserServiceManagement.Domain/             # Couche domaine
│   ├── 📁 Context/                                 # Contexte EF Core
│   │   └── UserContext.cs
│   └── 📁 Migrations/                              # Migrations
│       ├── 202407XXXXXX_InitialCreate.cs
│       ├── 202407XXXXXX_AddUser.cs
│       └── ...
│
├── 📁 CBS.UserServiceManagement.Helper/             # Helpers
│   ├── 📁 DataModel/                               # Modèles de données
│   │   ├── UserInfoToken.cs
│   │   └── ...
│   └── 📁 Helper/                                  # Utilitaires
│       ├── USClaim.cs (ex-CustomClaim)
│       └── ...
│
├── 📁 CBS.UserServiceManagement.MediatR/            # Pattern MediatR (CQRS)
│   └── 📁 User/
│       ├── 📁 Commands/                            # Commandes
│       │   ├── AddUserCommand.cs
│       │   ├── LoginUserCommand.cs
│       │   └── ...
│       ├── 📁 Handlers/                            # Handlers
│       │   ├── AddUserCommandHandler.cs
│       │   ├── LoginUserCommandHandler.cs
│       │   └── ...
│       └── 📁 Validators/                          # Validateurs
│           ├── AddUserCommandValidator.cs
│           └── ...
│
├── 📁 CBS.UserServiceManagement.Repository/         # Répertoires
│   └── 📁 User/
│       ├── IUserRepository.cs
│       └── UserRepository.cs


using CBS.UserServiceManagement.Helper;
using Microsoft.AspNetCore.Mvc;

namespace CBS.UserServiceManagement.API
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T ApiResponseData { get; set; }
    }

    public class BaseController : ControllerBase
    {
        // Method to format and return API responses.
        // Parameters:
        // - result: ServiceResponse containing data and response details.
        // Returns:
        // - IActionResult representing the formatted API response.
        public IActionResult ReturnFormattedResponse<T>(ServiceResponse<T> result)
        {
            // Create a response object using the provided ServiceResponse data.
            ResponseObject responseObject = new ResponseObject(
                data: result.Data,
                statusCode: result.StatusCode,
                description: result.Description,
                message: result.Message,
            status: result.Status,
                errors: result.Errors
            );

            // Check if the operation was successful.
            if (result.Status == "SUCCESS")
            {
                // Return a 200 OK response with the formatted response object.
                return Ok(responseObject);
            }

            // Return an appropriate status code with the formatted response object.
            return StatusCode(responseObject.StatusCode, responseObject);
        }

        public IActionResult ReturnFormattedResponseObject<T>(ServiceResponse<T> result)
        {
            // Create a response object using the provided ServiceResponse data.
            ApiResponse<T> responseObject = new ApiResponse<T>
            {
                ApiResponseData = result.Data,
                IsSuccess = result.Status == "SUCCESS",

                Message = result.Status == "SUCCESS" ? result.Message : GetMessage(result.Errors)
            };

            // Check if the operation was successful.
            if (result.Status == "SUCCESS")
            {
                // Return a 200 OK response with the formatted response object.
                return Ok(responseObject);
            }
            else
            {
                responseObject.Message = result.Message;
                return BadRequest(responseObject);
            }
        }

        private string GetMessage(List<string> errors)
        {
            string message = "";
            foreach (var error in errors)
                message = message + error + " ";
            return message;
        }

        //public DateTime ConvertStringToDateTime(string dateString)
        //{
        //    CultureInfo culture = new CultureInfo("en-US");

        //    DateTime parsedDate = DateTime.Parse(dateString, culture, DateTimeStyles.None);

        //    return parsedDate;

        //}
    }
}

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
        [Authorize(Roles = "Admin")]  // Restriction aux admins
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 403)]
        [ProducesResponseType(typeof(ResponseObject), 409)]
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

using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace CBS.UserServiceManagement.API.Helpers.DependencyResolver
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            // Enregistrement du UnitOfWork (Scoped: une instance par requête HTTP)
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

            // Enregistrement de notre repository spécifique (Scoped)
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Enregistrement de UserInfoToken (Scoped)
            services.AddScoped<UserInfoToken>();
        }
    }
}

using AutoMapper;
using CBS.UserServiceManagement.API.Helpers;

namespace CBS.UserServiceManagement.API.Helpers
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserMappingProfile());
            });
            return mappingConfig.CreateMapper();
        }
    }
}

using AutoMapper;
using CBS.UserServiceManagement.Data;


namespace CBS.UserServiceManagement.API
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate));
        }
    }
}

// Source: Déduit de votre appsettings.json modèle et de la logique de validation JWT.
namespace CBS.UserServiceManagement.API.Helpers
{
    public class JwtSettings
    {
        /// <summary>
        /// La clé secrète utilisée pour signer et valider le token.
        /// Doit être suffisamment longue et complexe.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// L'émetteur du token (l'autorité qui a généré le token).
        /// Ex: "https://identity.bapcculcbs.com/"
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// L'audience attendue du token (le service à qui le token est destiné).
        /// Ex: "CBS.UserServiceManagement"
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// La durée de validité du token en minutes.
        /// </summary>
        public int MinutesToExpiration { get; set; }
    }
}

using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.API
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserContext dbContext)
        {
            context.Request.EnableBuffering();
            
            await _next(context);

            var request = context.Request;
            if (request.Method == "POST" || request.Method == "PUT" || request.Method == "DELETE")
            {
                if (context.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is string userId)
                {
                    var auditLog = new AuditLog
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        UserEmail = context.Items["Email"]?.ToString(),
                        EntityName = request.RouteValues["controller"]?.ToString(),
                        Action = request.Method,
                        Timestamp = DateTime.UtcNow,
                        Changes = await GetChangesAsync(request),
                        IPAddress = context.Connection.RemoteIpAddress?.ToString(),
                        Url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}"
                    };

                    dbContext.AuditLogs.Add(auditLog);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task<string> GetChangesAsync(HttpRequest request)
        {
            if (request.Method == "DELETE")
            {
                return $"Deleted resource with ID: {request.RouteValues["id"]?.ToString()}";
            }

            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var bodyAsText = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return bodyAsText;
        }
    }
}
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CBS.UserServiceManagement.API.Middlewares.ExceptionHandlingMiddleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur non gérée s'est produite");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("Une erreur interne s'est produite. Veuillez réessayer plus tard.");
            }
        }
    }
}
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
using CBS.UserServiceManagement.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.API
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            var builder = new StringBuilder();
            var request = await FormatRequest(context.Request);
            builder.Append("Request: ").AppendLine(request);
            
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            
            await _next(context);

            var response = await FormatResponse(context.Response);
            builder.Append("Response: ").AppendLine(response);
            _logger.LogInformation(builder.ToString());
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            var formattedRequest = $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {body}";
            request.Body.Position = 0;
            return formattedRequest;
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"{response.StatusCode}: {text}";
        }
    }
}
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.API.Middlewares.SecurityHeadersMiddleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Ajout des en-têtes de sécurité
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Content-Security-Policy"] = "default-src 'self'";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            context.Response.Headers["Permissions-Policy"] = "geolocation=()";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

            await _next(context);
        }
    }
}
{
  "ConnectionStrings": {
    "DefaultConnection": "data source=localhost;Initial Catalog=CBSUserServiceMGTDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Key": "8m2YG5c3UYBcNhFDOzvf5jQaup2qXTTHUpKvbuufyS4=",
    "issuer": "https://localhost:5001",
    "audience": "CBS.UserServiceManagement",
    "minutesToExpiration": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    },
    "AuditTrails": {
      "BaseUrl": "http://localhost:5000/"
    }
  },
  "ConsulConfig": {
    "consulAddress": "http://localhost:8500",
    "ServiceHost": "localhost",
    "servicePort": 7089
  }
}
{
  "ConnectionStrings": {
    "DefaultConnection": "data source=localhost;Initial Catalog=CBSUserServiceMGTDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "issuer": "https://localhost:5001", // URL du serveur d'identité local
    "audience": "CBS.UserServiceManagement"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug", // Niveau de log plus verbeux en développement
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    },
    "AuditTrails": {
      "BaseUrl": "http://localhost:5000/" // URL de l'API Gateway locale
    }
  },
  "ConsulConfig": {
    "consulAddress": "http://localhost:8500",
    "ServiceHost": "localhost",
    "servicePort": 7089 // Port HTTPS de notre service en local
  }
}

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
            // Initialisation de NLog
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
                // Assure le vidage et l'arrêt des timers internes avant la fin de l'application
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseNLog(); // Activation de NLog pour le logging
    }
}
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

        // Cette méthode est appelée par le runtime pour ajouter des services au conteneur.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration CORS sécurisée
            services.AddCors(options =>
            {
                options.AddPolicy("SecurePolicy", builder =>
                {
                    builder.WithOrigins(
                        "https://localhost:5001",  // Swagger HTTPS
                        "http://localhost:5000",   // Swagger HTTP
                        "http://localhost:5121" // Votre domaine
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            // 1. CONFIGURATION DE LA BASE DE DONNÉES
            services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // 2. INJECTION DES DÉPENDANCES D'INFRASTRUCTURE (Repositories, UnitOfWork)
            services.AddInfrastructureServices();

            // 3. INJECTION DES SERVICES APPLICATIFS
            // Enregistre MediatR en scannant l'assembly où se trouve AddUserCommand
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddUserCommand).Assembly));
            // Enregistre AutoMapper en utilisant notre configuration centralisée
            services.AddSingleton(provider => MapperConfig.GetMapperConfigs());
            // Enregistre FluentValidation en scannant l'assembly où se trouvent nos validateurs
            services.AddValidatorsFromAssembly(typeof(AddUserCommandValidator).Assembly);

            // 4. CONFIGURATION DE LA SÉCURITÉ (Authentification externe)
            var jwtSettings = new JwtSettings();
            Configuration.Bind("JwtSettings", jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.SecurityToken is System.IdentityModel.Tokens.Jwt.JwtSecurityToken accessToken)
                        {
                            var userInfoToken = context.HttpContext.RequestServices.GetRequiredService<UserInfoToken>();
                            userInfoToken.Id = accessToken.Claims.FirstOrDefault(a => a.Type == "Id")?.Value;
                            userInfoToken.Email = accessToken.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
                            userInfoToken.Role = accessToken.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role)?.Value;
                            userInfoToken.Token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                            context.HttpContext.Items["UserId"] = userInfoToken.Id;
                            context.HttpContext.Items["Email"] = userInfoToken.Email;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("Admin"));
            });

            // 5. AJOUT DES SERVICES DE BASE DE L'API
            services.AddControllers();
            services.AddHttpContextAccessor(); // Important pour accéder à HttpContext depuis les services
            services.AddEndpointsApiExplorer();

            // 6. CONFIGURATION DE SWAGGER POUR GÉRER L'AUTHENTIFICATION JWT
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserServiceManagement API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
        }

        // Cette méthode est appelée par le runtime pour configurer le pipeline de requêtes HTTP.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Middleware de gestion des exceptions
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Affiche des pages d'erreur détaillées en développement
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserServiceManagement API v1"));
            }
            else
            {
                // En production, activer HSTS pour forcer HTTPS
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // Activer CORS avant Authentication
            app.UseCors("SecurePolicy");

            // --- ORDRE CRUCIAL DES MIDDLEWARES ---

            // 1. Logging des requêtes/réponses : en premier pour tout capturer.
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            // 2. Authentification : identifie l'utilisateur à partir du token.
            app.UseAuthentication();

            // 3. Autorisation : vérifie si l'utilisateur identifié a les droits.
            app.UseAuthorization();

            // 4. Audit : se déclenche après l'authentification pour savoir qui a fait l'action.
            app.UseMiddleware<AuditLogMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

// Source: Votre code source exact.
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

// Source: Modèle original, corrigé pour être cohérent avec l'entité User.
using System;

namespace CBS.UserServiceManagement.Data // J'ajuste le namespace pour la cohérence
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

using System;
using System.ComponentModel.DataAnnotations;

namespace CBS.UserServiceManagement.Data
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? EntityName { get; set; }
        public string? Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Changes { get; set; }
        public string? IPAddress { get; set; }
        public string? Url { get; set; }
    }
}
using Microsoft.Web.Administration;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.UserServiceManagement.Data
{


    public abstract class BaseEntity
    {
        private DateTime _createdDate;

        public DateTime CreatedDate
        {
            get => _createdDate.ToLocalTime();
            set => _createdDate = value.ToLocalTime();
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
        
        private string _createdBy;

        public string CreatedBy
        {
            get => _createdBy;
            set => _createdBy = value;
        }
        private DateTime? _modifiedDate;

        public DateTime? ModifiedDate
        {
            get => _modifiedDate?.ToLocalTime();
            set => _modifiedDate = value?.ToLocalTime();
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
// Source: Modèle implicite, avec l'ID défini directement dans l'entité.
using System;
using System.ComponentModel.DataAnnotations; // Ajout pour l'attribut [Key]

namespace CBS.UserServiceManagement.Data
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.Helper
{
    public class USClaim
    {
        public string claimType { get; set; }
        public string claimValue { get; set; }
    }
}
namespace CBS.UserServiceManagement.Helper
{
    public class LoginDto
    {
        public LoginDto(string id, string userName, string firstName, string lastName, string email, int expirationTime, string phoneNumber, string bearerToken, bool isAuthenticated, string profilePhoto, List<USClaim> claims)
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
        public List<USClaim> claims { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.Helper.DataModel
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
namespace CBS.UserServiceManagement.Helper
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
         new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}
namespace CBS.UserServiceManagement.Helper
{
    public static class CountryType
    {
        public static string GetCountryType(string extension)
        {
            string type = string.Empty;
            switch (extension)
            {
                case ".pdf":
                    type = "pdf";
                    break;

                case ".docx":
                    type = "mammoth";
                    break;

                case ".pptx":
                    type = "office";
                    break;

                default:
                    type = "";
                    break;
            }
            return type;
        }

        public static string Get64ContentStartText(string extension)
        {
            string type = string.Empty;
            switch (extension)
            {
                case ".pdf":
                    type = "data:application/pdf;base64,";
                    break;

                case ".docx":
                    type = "data:application/vnd.openxmlformats-officeCountry.wordprocessingml.Country;base64,";
                    break;

                case ".pptx":
                    type = "data:application/vnd.openxmlformats-officeCountry.presentationml.presentation;base64,";
                    break;

                default:
                    type = "";
                    break;
            }
            return type;
        }
    }
}
using System.Net;
using System.Net.Mail;

namespace CBS.UserServiceManagement.Helper
{
    public class EmailHelper
    {
        private static List<MemoryStream> attachments = new List<MemoryStream>();

        public static void SendEmail(SendEmailSpecification sendEmailSpecification)
        {
            MailMessage message = new MailMessage()
            {
                Sender = new MailAddress(sendEmailSpecification.FromAddress),
                From = new MailAddress(sendEmailSpecification.FromAddress),
                Subject = sendEmailSpecification.Subject,
                IsBodyHtml = true,
                Body = sendEmailSpecification.Body,
            };

            if (sendEmailSpecification.Attechments.Count > 0)
            {
                Attachment attach;
                foreach (var file in sendEmailSpecification.Attechments)
                {
                    string fileData = file.Src.Split(',').LastOrDefault();
                    byte[] bytes = Convert.FromBase64String(fileData);
                    var ms = new MemoryStream(bytes);
                    attach = new Attachment(ms, file.Name, file.FileType);
                    attachments.Add(ms);
                    message.Attachments.Add(attach);
                }
            }
            sendEmailSpecification.ToAddress.Split(",").ToList().ForEach(toAddress =>
            {
                message.To.Add(new MailAddress(toAddress));
            });
            if (!string.IsNullOrEmpty(sendEmailSpecification.CCAddress))
            {
                sendEmailSpecification.CCAddress.Split(",").ToList().ForEach(ccAddress =>
                {
                    message.CC.Add(new MailAddress(ccAddress));
                });
            }

            SmtpClient smtp = new SmtpClient()
            {
                Port = sendEmailSpecification.Port,
                Host = sendEmailSpecification.Host,
                EnableSsl = sendEmailSpecification.IsEnableSSL,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(sendEmailSpecification.UserName, sendEmailSpecification.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtp.Send(message);
            if (attachments.Count() > 0)
            {
                foreach (var attachment in attachments)
                {
                    try
                    {
                        attachment.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static Attachment ConvertStringToStream(FileInfo fileInfo)
        {
            string fileData = fileInfo.Src.Split(',').LastOrDefault();
            byte[] bytes = Convert.FromBase64String(fileData);
            System.Net.Mail.Attachment attach;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                attach = new System.Net.Mail.Attachment(ms, fileInfo.Name, fileInfo.FileType);
                // I guess you know how to send email with an attachment
                // after sending email
                //ms.Close();
                attachments.Add(ms);
            }
            return attach;
        }
    }
}
namespace CBS.UserServiceManagement.Helper
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
namespace CBS.UserServiceManagement.Helper
{
    public class FileInfo
    {
        public string Src { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string FileType { get; set; }
    }
}

using System.Linq.Expressions;

namespace CBS.UserServiceManagement.Helper
{
    public class GenericSpecification<T>
    {
        private readonly Expression<Func<T, bool>> expression;

        public GenericSpecification(Expression<Func<T, bool>> _expression)
        {
            this.expression = _expression;
        }

        public bool IsSatifiedBy(T entity)
        {
            return this.expression.Compile().Invoke(entity);
        }
    }

    internal sealed class IdentificationSpecification<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> ExpressionTo()
        {
            return x => true;
        }
    }

    public abstract class Specification<T>
    {
        public static readonly Specification<T> All = new IdentificationSpecification<T>();

        public bool IsSatified(T entity)
        {
            Func<T, Boolean> predicate = this.ExpressionTo().Compile();
            return predicate.Invoke(entity);
        }

        public abstract Expression<Func<T, bool>> ExpressionTo();

        public Specification<T> And(Specification<T> specification)
        {
            if (this == All)
                return specification;
            if (specification == All)
                return this;
            return new AndSpecification<T>(this, specification);
        }

        public Specification<T> Or(Specification<T> specification)
        {
            if (this == All || specification == All)
                return All;
            return new OrSpecification<T>(this, specification);
        }

        public Specification<T> Not(Specification<T> specification)
        {
            return new NotSpecification<T>(this);
        }
    }

    internal sealed class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> left;
        private readonly Specification<T> right;

        public AndSpecification(Specification<T> _left, Specification<T> _right)
        {
            left = _left;
            right = _right;
        }

        public override Expression<Func<T, bool>> ExpressionTo()
        {
            Expression<Func<T, bool>> leftPredicate = left.ExpressionTo();
            Expression<Func<T, bool>> rightPredicate = right.ExpressionTo();

            BinaryExpression andExpression = Expression.AndAlso(leftPredicate.Body, rightPredicate.Body);
            return Expression.Lambda<Func<T, bool>>(andExpression, leftPredicate.Parameters.Single());
        }
    }

    internal sealed class OrSpecification<T> : Specification<T>
    {
        private readonly Specification<T> left;
        private readonly Specification<T> right;

        public OrSpecification(Specification<T> _left, Specification<T> _right)
        {
            left = _left;
            right = _right;
        }

        public override Expression<Func<T, bool>> ExpressionTo()
        {
            Expression<Func<T, bool>> leftPredicate = left.ExpressionTo();
            Expression<Func<T, bool>> rightPredicate = right.ExpressionTo();

            BinaryExpression expression = Expression.OrElse(leftPredicate.Body, rightPredicate.Body);

            return Expression.Lambda<Func<T, bool>>(expression, leftPredicate.Parameters.Single());
        }
    }

    internal sealed class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> specification;

        public NotSpecification(Specification<T> _specification)
        {
            specification = _specification;
        }

        public override Expression<Func<T, bool>> ExpressionTo()
        {
            Expression<Func<T, bool>> predicate = specification.ExpressionTo();

            UnaryExpression expression = Expression.Not(predicate.Body);
            return Expression.Lambda<Func<T, bool>>(expression, predicate.Parameters.Single());
        }
    }
}

using System.Dynamic;
using System.Reflection;

namespace CBS.UserServiceManagement.Helper
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(
            this IEnumerable<TSource> source,
            string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // create a list to hold our ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();

            // create a list with PropertyInfo objects on TSource.  Reflection is
            // expensive, so rather than doing it for each object in the list, we do
            // it once and reuse the results.  After all, part of the reflection is on the
            // type of the object (TSource), not on the instance
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                // all public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                // only the public properties that match the fields should be
                // in the ExpandoObject

                // the field are separated by ",", so we split it.
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    // trim each field, as it might contain leading
                    // or trailing spaces. Can't trim the var in foreach,
                    // so use another var.
                    var propertyName = field.Trim();

                    // use reflection to get the property on the source object
                    // we need to include public and instance, b/c specifying a binding flag overwrites the
                    // already-existing binding flags.
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                    }

                    // add propertyInfo to list
                    propertyInfoList.Add(propertyInfo);
                }
            }

            // run through the source objects
            foreach (TSource sourceObject in source)
            {
                // create an ExpandoObject that will hold the
                // selected properties & values
                var dataShapedObject = new ExpandoObject();

                // Get the value of each property we have to return.  For that,
                // we run through the list
                foreach (var propertyInfo in propertyInfoList)
                {
                    // GetValue returns the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    // add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                // add the ExpandoObject to the list
                expandoObjectList.Add(dataShapedObject);
            }

            // return the list

            return expandoObjectList;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}

using System.Dynamic;
using System.Reflection;

namespace CBS.UserServiceManagement.Helper
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source,
          string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var dataShapedObject = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields))
            {
                // all public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource)
                        .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                foreach (var propertyInfo in propertyInfos)
                {
                    // get the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(source);

                    // add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                return dataShapedObject;
            }

            // the field are separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                // trim each field, as it might contain leading
                // or trailing spaces. Can't trim the var in foreach,
                // so use another var.
                var propertyName = field.Trim();

                // use reflection to get the property on the source object
                // we need to include public and instance, b/c specifying a binding flag overwrites the
                // already-existing binding flags.
                var propertyInfo = typeof(TSource)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                }

                // get the value of the property on the source object
                var propertyValue = propertyInfo.GetValue(source);

                // add the field to the ExpandoObject
                ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }

            // return
            return dataShapedObject;
        }
    }
}

namespace CBS.UserServiceManagement.Helper
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int Skip { get; set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious
        {
            get
            {
                return (CurrentPage > 1);
            }
        }

        public bool HasNext
        {
            get
            {
                return (CurrentPage < TotalPages);
            }
        }

        public PagedList(List<T> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static PagedList<T> Create(IQueryable<T> source, int skip, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip(skip).Take(pageSize).ToList();
            return new PagedList<T>(items, count, skip, pageSize);
        }
    }
}

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

namespace CBS.UserServiceManagement.Helper
{
    public abstract class ResourceParameters
    {
        public ResourceParameters(string orderBy)
        {
            this.OrderBy = orderBy;
        }

        private const int maxPageSize = 100;
        public int Skip { get; set; } = 0;

        private int _pageSize = 10;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string SearchQuery { get; set; }

        public string OrderBy { get; set; }

        public string Fields { get; set; }
    }
}

namespace CBS.UserServiceManagement.Helper
{
    public class SendEmailSpecification
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string CCAddress { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public bool IsEnableSSL { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<FileInfo> Attechments { get; set; }
    }
}

// Source: Votre modèle standard et intouchable.
using System;
using System.Collections.Generic;

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

        // Constructeurs privés pour forcer l'utilisation des méthodes factory
        private ServiceResponse(T data) { Data = data; }
        private ServiceResponse() { }
        private ServiceResponse(int statusCode, List<string> errors) { StatusCode = statusCode; Errors = errors; }
        private ServiceResponse(int statusCode, T data) { StatusCode = statusCode; Data = data; }
        private ServiceResponse(int statusCode, string errorMessage) { StatusCode = statusCode; Errors = new List<string> { errorMessage }; }
        private ServiceResponse(int statusCode, T data, string message) { StatusCode = statusCode; Data = data; Message = message; }
        private ServiceResponse(int statusCode, T data, string message, string description, string status) { StatusCode = statusCode; Data = data; Message = message; Description = description; Status = status; }
        private ServiceResponse(int statusCode, string message, string description, string status) { StatusCode = statusCode; Message = message; Description = description; Status = status; }

        public bool Success => Errors == null || Errors.Count == 0;

        public static ServiceResponse<T> ReturnException(Exception ex)
        {
            return new ServiceResponse<T>(500, $"An unexpected fault happened. Error: {ex.Message}", "Failed with Internal Server Error", "FAILED");
        }
        public static ServiceResponse<T> ReturnFailed(int statusCode, List<string> errors)
        {
            return new ServiceResponse<T>(statusCode, errors);
        }
        public static ServiceResponse<T> ReturnFailed(int statusCode, string errorMessage)
        {
            return new ServiceResponse<T>(statusCode, errorMessage);
        }
        public static ServiceResponse<T> ReturnSuccess()
        {
            return new ServiceResponse<T>(default(T));
        }
        public static ServiceResponse<T> ReturnSuccess(string message)
        {
            return new ServiceResponse<T>(200, message, "Transaction was successful", "SUCCESS");
        }
        public static ServiceResponse<T> ReturnResultWith200(T data)
        {
            return new ServiceResponse<T>(200, data, string.Empty, "Transaction was successful", "SUCCESS");
        }
        public static ServiceResponse<T> ReturnResultWith200(T data, string message)
        {
            return new ServiceResponse<T>(200, data, message, "Transaction was successful", "SUCCESS");
        }
        public static ServiceResponse<T> ReturnResultWith201(T data, string message)
        {
            return new ServiceResponse<T>(201, data, message, "Transaction was successful", "SUCCESS");
        }
        public static ServiceResponse<T> ReturnResultWith204(T data)
        {
            return new ServiceResponse<T>(204, data, string.Empty, "Transaction was successful", "SUCCESS");
        }
        public static ServiceResponse<T> Return500()
        {
            return new ServiceResponse<T>(500, "An unexpected fault happened. Try again later.");
        }
        public static ServiceResponse<T> Return500(Exception ex, string message = null)
        {
            return new ServiceResponse<T>(500, $"An unexpected fault happened. {message} Error: {ex.Message}", "Failed with Internal Server Error", "FAILED");
        }
        public static ServiceResponse<T> Return500(string message)
        {
            return new ServiceResponse<T>(500, $"{message}", "Failed with Internal Server Error", "FAILED");
        }
        public static ServiceResponse<T> Return409(string message)
        {
            return new ServiceResponse<T>(409, message, "Failed, Record already exists. Operation was cancelled", "FAILED");
        }
        public static ServiceResponse<T> Return409()
        {
            return new ServiceResponse<T>(409, "Record already exists", "Failed, Record already exists. Operation was cancelled", "FAILED");
        }
        public static ServiceResponse<T> Return422(string message)
        {
            return new ServiceResponse<T>(422, new List<string> { "Unprocessable Entity", message, "Failed. Process terminated.", "FAILED" });
        }
        public static ServiceResponse<T> Return404()
        {
            return new ServiceResponse<T>(404, "Record not found", "Failed. Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return404(string message)
        {
            return new ServiceResponse<T>(404, message, "Failed. Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return401(string message)
        {
            return new ServiceResponse<T>(401, message, "Failed.User not authourized Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return403(string message)
        {
            return new ServiceResponse<T>(403, message, "Operation is forbidden. Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return403(T data)
        {
            return new ServiceResponse<T>(403, "Operation is forbidden.", "Failed. Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return403()
        {
            return new ServiceResponse<T>(403, "Operation is forbidden.", "Failed. Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return403(T data, string message)
        {
            return new ServiceResponse<T>(403, data, message, "Operation is forbidden.", "FAILED");
        }
    }

    public class ResponseObject
    {
        public object Data { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; } = 200;
        public string StatusDescription { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }

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

// Source: Votre code, ajusté pour retourner le type correct.
using CBS.UserServiceManagement.Data;

using CBS.UserServiceManagement.Helper;
using MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    public class AddUserCommand : IRequest<ServiceResponse<UserDto>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

using CBS.UserServiceManagement.Helper;
using MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    public class LoginUserCommand : IRequest<ServiceResponse<LoginResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}

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

using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.MediatR;
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.MediatR
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ServiceResponse<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoginUserCommandHandler> _logger;
        private readonly IConfiguration _configuration;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            ILogger<LoginUserCommandHandler> logger,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindBy(u => u.Email == request.Email)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login validation failed for email: {Email}", request.Email);
                return ServiceResponse<LoginResponse>.Return401("Invalid credentials provided to UserServiceManagement.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"]);

            var claims = new System.Collections.Generic.List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:minutesToExpiration"])),
                Issuer = _configuration["JwtSettings:issuer"],
                Audience = _configuration["JwtSettings:audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var response = new LoginResponse
            {
                Token = tokenString,
                RefreshToken = string.Empty
            };

            return ServiceResponse<LoginResponse>.ReturnResultWith200(response, "Credentials are valid. Token obtained from UserServiceManagement.");
        }
    }
}

// Source: Votre modèle AddUserCommandValidator.cs
using FluentValidation;
using CBS.UserServiceManagement.MediatR; // using vers la commande à valider

namespace CBS.UserServiceManagement.MediatR
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            // Règle pour le prénom
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Le prénom est requis.")
                .MaximumLength(100).WithMessage("Le prénom ne peut pas dépasser 100 caractères.");

            // Règle pour le nom de famille
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Le nom est requis.")
                .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères.");

            // Règle pour l'email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est requis.")
                .EmailAddress().WithMessage("Un format d'email valide est requis.")
                .MaximumLength(150).WithMessage("L'email ne peut pas dépasser 150 caractères.");

            // Règle pour le mot de passe
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est requis.")
                .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères.")
                .Matches(@"[A-Z]").WithMessage("Le mot de passe doit contenir au moins une lettre majuscule.")
                .Matches(@"[a-z]").WithMessage("Le mot de passe doit contenir au moins une lettre minuscule.")
                .Matches(@"[0-9]").WithMessage("Le mot de passe doit contenir au moins un chiffre.");
            // On pourrait ajouter une règle pour les caractères spéciaux si nécessaire
            // .Matches(@"[\W_]").WithMessage("Le mot de passe doit contenir au moins un caractère spécial.");
        }
    }
}

using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
       
    }
}
// Source: Votre modèle CountryRepository appliqué à notre contexte.
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;


namespace CBS.UserServiceManagement.Repository
{
    public class UserRepository : GenericRepository<User, UserContext>, IUserRepository
    {
        public UserRepository(IUnitOfWork<UserContext> unitOfWork) : base(unitOfWork)
        {
            // LE CONSTRUCTEUR EST VIDE, COMME DANS LE MODÈLE.
            // Toute la logique de base est gérée par la classe GenericRepository.
        }
    }
}

<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
	
	  <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
	
	  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.13" />
	
	  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.7" />
	  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.7">
	    <PrivateAssets>all</PrivateAssets>
	    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
	  </PackageReference>
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
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.7" />
  </ItemGroup>

</Project>
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.7" />
    <PackageReference Include="Microsoft.Web.Administration" Version="11.1.0" />
  </ItemGroup>

  <ItemGroup>
    <Folder Include="Enum\" />
  </ItemGroup>

</Project>
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.7" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.7">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.7" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.7">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
  </ItemGroup>

</Project>
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\..\Common\APICallerHelper\CBS.APICaller.Helper\CBS.APICaller.Helper.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
  </ItemGroup>

</Project>
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
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
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
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
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
	  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.7" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CBS.UserServiceManagement.Common\CBS.UserServiceManagement.Common.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Domain\CBS.UserServiceManagement.Domain.csproj" />
  </ItemGroup>

</Project>
