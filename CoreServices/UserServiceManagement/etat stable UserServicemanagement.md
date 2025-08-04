<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
	
	  <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.1" />
	
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
    // --- CORRECTION : Ajout des attributs standards du modèle ---
    [ApiController]
    [Route("api/v1/[controller]")] // Route standardisée et versionnée
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new user in the system. (Admin role required)
        /// </summary>
        /// <param name="command">The user creation request data.</param>
        /// <returns>The newly created user's data.</returns>
        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 201)] // 201 Created est plus approprié pour un POST réussi
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 403)]
        [ProducesResponseType(typeof(ResponseObject), 409)]
        public async Task<IActionResult> AddUser([FromBody] AddUserCommand command)
        {
            // La validation automatique via [ApiController] rend ce bloc optionnel, mais c'est une sécurité supplémentaire
            // Validation automatique par FluentValidation
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var responseObject = new ResponseObject(
                    statusCode: 400,
                    message: "Invalid request",
                    status: "FAILED",
                    errors: errors
                );
                return BadRequest(responseObject);
            }

            _logger.LogInformation("Attempting to add a new user");
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to add user: {Message}", result.Message);
            }

            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token upon success.
        /// </summary>
        /// <param name="command">The user's login credentials.</param>
        /// <returns>A JWT token.</returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            // Vérification de la validité du modèle
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                var responseObject = new ResponseObject(
                    statusCode: 400,
                    message: "Requête invalide",
                    status: "FAILED",
                    errors: errors
                );
                return BadRequest(responseObject);
            }

            _logger.LogInformation("Login attempt for email {Email}", command.Email);
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a specific user by their unique ID. (Authenticated users only)
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The requested user's data.</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetUserById(string id)
        {
            _logger.LogInformation("Attempting to retrieve user with ID {UserId}", id);
            var query = new GetUserByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a list of all users. (Admin role required)
        /// </summary>
        /// <returns>A list of all users.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 403)]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Admin user retrieving all users.");
            var query = new GetAllUsersQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes a user from the system (soft delete). (Admin role required)
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>A confirmation of the deletion.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 403)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            _logger.LogInformation("Attempting to delete user with ID {UserId}", id);

            var command = new DeleteUserCommand(id);
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
using CBS.UserServiceManagement.MediatR;


namespace CBS.UserServiceManagement.API
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
                     CreateMap<User, UserDto>().ReverseMap();
           // CreateMap<UpdateUserCommand, User>().ReverseMap();
            CreateMap<AddUserCommand, User>().ReverseMap();

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
using FluentValidation;
using System.Text.Json;

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
            // --- AJOUT DE CE BLOC CATCH SPÉCIFIQUE ---
            catch (ValidationException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                // On récupère la liste des messages d'erreur de FluentValidation
                var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for request {Path}: {Errors}", context.Request.Path, string.Join(", ", errors));

                // On crée une réponse JSON structurée
                var response = new
                {
                    message = "One or more validation errors occurred.",
                    errors = errors
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new { message = "An internal server error has occurred." };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
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

// Fichier : CBS.UserServiceManagement.API/Middlewares/JwtValidator/JwtAuthenticationConfigurationExtension.cs

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
// Fichier : CoreServices/UserServiceManagement/CBS.UserServiceManagement.API/Middlewares/SecurityHeadersMiddleware/SecurityHeadersMiddleware.cs

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
            // Ajoute l'en-tête X-Content-Type-Options pour empêcher le "MIME type sniffing".
            // Cela réduit le risque d'attaques où un fichier est interprété avec un type de contenu incorrect.
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // Ajoute l'en-tête X-Frame-Options pour se protéger contre le "clickjacking".
            // 'DENY' empêche la page d'être affichée dans une frame ou iframe.
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // Ajoute l'en-tête Content-Security-Policy (CSP) pour contrôler les ressources
            // que le navigateur est autorisé à charger pour la page.
            // "default-src 'self'" est une politique restrictive qui n'autorise que les ressources provenant de la même origine.
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; frame-ancestors 'none';");

            // Ajoute l'en-tête Referrer-Policy pour contrôler la quantité d'informations de "referrer"
            // envoyées avec les requêtes. "no-referrer" empêche l'envoi de cet en-tête.
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");

            // Ajoute l'en-tête Permissions-Policy pour contrôler l'accès aux fonctionnalités du navigateur.
            // Ici, nous désactivons explicitement la géolocalisation.
            context.Response.Headers.Append("Permissions-Policy", "geolocation=()");

            // Ajoute l'en-tête X-XSS-Protection pour activer le filtre anti-XSS des navigateurs.
            // "1; mode=block" active la protection et empêche le rendu de la page si une attaque est détectée.
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Continue le traitement de la requête dans le pipeline
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
// Fichier : CoreServices/UserServiceManagement/CBS.UserServiceManagement.API/Startup.cs

// --- USINGS NÉCESSAIRES ---
using CBS.UserServiceManagement.API.Helpers;
using CBS.UserServiceManagement.API.Helpers.DependencyResolver;
using CBS.UserServiceManagement.API.Middlewares.ExceptionHandlingMiddleware;
using CBS.UserServiceManagement.API.Middlewares.JwtValidator;
using CBS.UserServiceManagement.API.Middlewares.SecurityHeadersMiddleware; // Vous devrez créer ce fichier à l'étape suivante
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            // Configuration CORS sécurisée (comme dans le modèle)
            services.AddCors(options =>
            {
                options.AddPolicy("SecurePolicy", builder =>
                {
                    // Adaptez les origines si nécessaire pour votre front-end
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // 1. CONFIGURATION DE LA BASE DE DONNÉES (en spécifiant l'assembly des migrations)
            services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("CBS.UserServiceManagement.Domain")));

            // 2. APPEL À L'EXTENSION DE DI POUR L'INFRASTRUCTURE (conforme au modèle)
            services.AddInfrastructureServices();

            // 3. ENREGISTREMENT DES SERVICES APPLICATIFS (directement ici, comme dans le modèle)

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddUserCommand).Assembly));
            services.AddValidatorsFromAssembly(typeof(AddUserCommandValidator).Assembly);
            services.AddSingleton(provider => MapperConfig.GetMapperConfigs());

            // 4. CONFIGURATION SÉCURITÉ JWT (copiée du modèle)
            var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null) throw new ArgumentNullException(nameof(jwtSettings), "JwtSettings configuration is missing in appsettings.json");
            services.AddSingleton(jwtSettings);
            services.AddJwtAuthenticationConfiguration(jwtSettings); // Appel à votre extension existante

            // 5. CONFIGURATION DES SERVICES API (copiée du modèle)
            services.AddControllers()
    .AddFluentValidation(config =>
    {
        config.RegisterValidatorsFromAssemblyContaining<AddUserCommandValidator>();
        config.DisableDataAnnotationsValidation = true;
    });
            services.AddHttpContextAccessor();
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
            // PIPELINE DE MIDDLEWARES COMPLET (copié du modèle, ordre critique)

            // 1. Gestion globale des exceptions (doit être le premier pour tout intercepter)
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // 2. Ajout des headers de sécurité
           // app.UseMiddleware<SecurityHeadersMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserServiceManagement API v1"));
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("SecurePolicy");

            // 3. Logging de chaque requête/réponse
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            // 4. Authentification : identifie l'utilisateur à partir du token JWT
            app.UseAuthentication();

            // 5. Autorisation : vérifie si l'utilisateur identifié a les droits nécessaires
            app.UseAuthorization();

            // 6. Audit : logue l'action de l'utilisateur authentifié
            app.UseMiddleware<AuditLogMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
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
// <auto-generated />
using System;
using CBS.UserServiceManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CBS.UserServiceManagement.Domain.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CBS.UserServiceManagement.Data.AuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Changes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IPAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AuditLogs");
                });

            modelBuilder.Entity("CBS.UserServiceManagement.Data.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TempData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
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
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageReference Include="FluentValidation" Version="12.0.0" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="12.0.0" />
    <PackageReference Include="FluentValidation.WebAPI" Version="8.6.1" />
    <PackageReference Include="MediatR" Version="12.2.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CBS.UserServiceManagement.Data\CBS.UserServiceManagement.Data.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Domain\CBS.UserServiceManagement.Domain.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Helper\CBS.UserServiceManagement.Helper.csproj" />
    <ProjectReference Include="..\CBS.UserServiceManagement.Repository\CBS.UserServiceManagement.Repository.csproj" />
  </ItemGroup>

</Project>
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// --- NAMESPACE CONFORME À LA STRUCTURE ---
namespace CBS.UserServiceManagement.MediatR
{
    // Ce behavior s'exécutera pour chaque requête MediatR (TRequest) qui retourne une réponse (TResponse).
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        // On injecte tous les validateurs que FluentValidation connaît pour la requête TRequest.
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // S'il n'y a pas de validateur pour cette requête, on passe directement au handler.
            if (!_validators.Any())
            {
                return await next();
            }

            // Créer un contexte de validation pour FluentValidation.
            var context = new ValidationContext<TRequest>(request);

            // Exécuter tous les validateurs en parallèle et récupérer les résultats.
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Agréger toutes les erreurs de validation en une seule liste.
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            // S'il y a au moins une erreur de validation...
            if (failures.Any())
            {
                // ... on lève une ValidationException.
                // Cela interrompt le pipeline et empêche l'exécution du handler.
                throw new ValidationException(failures);
            }

            // Si aucune erreur n'a été trouvée, on exécute le prochain maillon du pipeline (le handler).
            return await next();
        }
    }
}
// Fichier : CBS.UserServiceManagement.MediatR/AddUserCommand.cs

using CBS.UserServiceManagement.Data; // Assurez-vous d'avoir ce using pour UserDto
using CBS.UserServiceManagement.Helper;
using MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    // Le type de retour est maintenant ServiceResponse<UserDto> et non plus Guid.
    public class AddUserCommand : IRequest<ServiceResponse<UserDto>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public string Role { get; set; } // AJOUTER CETTE LIGNE
    }
}
using CBS.UserServiceManagement.Helper;
using MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    public class DeleteUserCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }

        public DeleteUserCommand(string id)
        {
            Id = id;
        }
    }
}
// Fichier : CBS.UserServiceManagement.MediatR/LoginUserCommand.cs

using CBS.UserServiceManagement.Helper;
using MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    // Le type de retour est standardisé
    public class LoginUserCommand : IRequest<ServiceResponse<LoginResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Le DTO de réponse est défini ici pour plus de clarté
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; } // Gardé pour une évolution future
    }
}
using AutoMapper;
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

// --- CORRECTION : Le namespace est conservé tel quel, conformément au modèle ---
using AutoMapper;
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

// --- CORRECTION : Le namespace est conservé tel quel, conformément au modèle ---
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
                    
                    // --- CORRECTION : Utilisation du template de message pour le logging ---
                    _logger.LogWarning("A user with the email {Email} already exists.", request.Email);
                    
                    return ServiceResponse<UserDto>.Return409(errorMessage);
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // --- CORRECTION : Utilisation du constructeur de l'entité ---
                var newUser = new User(
                    id: BaseUtilities.GenerateUniqueNumber(36),
                    firstName: request.FirstName,
                    lastName: request.LastName,
                    email: request.Email,
                    passwordHash: passwordHash,
                    role: "User" // Le rôle est assigné ici, de manière contrôlée.
                );

                _userRepository.Add(newUser);
                await _uow.SaveAsync();

                var userDto = _mapper.Map<UserDto>(newUser);

                _logger.LogInformation("Successfully created user with ID {UserId} and Role {Role}.", newUser.Id, newUser.Role);
                return ServiceResponse<UserDto>.ReturnResultWith201(userDto, "User created successfully.");
            }
            catch (Exception ex)
            {
                // --- CORRECTION : Utilisation du template de message pour le logging ---
                _logger.LogError(ex, "An unexpected error occurred while creating the user for email {Email}.", request.Email);
                
                return ServiceResponse<UserDto>.Return500(ex);
            }
        }
    }
}
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.MediatR;
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.MediatR
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ServiceResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork<UserContext> _uow;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork<UserContext> uow,
            ILogger<DeleteUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _uow = uow;
            _logger = logger;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userToDelete = await _userRepository.FindAsync(request.Id);

                if (userToDelete == null)
                {
                    _logger.LogWarning("Attempted to delete a non-existent user with ID {UserId}.", request.Id);
                    return ServiceResponse<bool>.Return404($"User with ID {request.Id} not found.");
                }
                
                _userRepository.Delete(userToDelete.Id);
                
                await _uow.SaveAsync();

                _logger.LogInformation("Successfully soft-deleted user with ID {UserId}.", request.Id);
                
                return ServiceResponse<bool>.ReturnResultWith200(true, "User successfully deleted.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with ID {UserId}.", request.Id);
                return ServiceResponse<bool>.Return500(ex);
            }
        }
    }
}
// --- USINGS NÉCESSAIRES ---
using AutoMapper;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.MediatR; // Namespace correct pour la Query
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore; // Nécessaire pour ToListAsync
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// --- NAMESPACE CONFORME À LA STRUCTURE ---
namespace CBS.UserServiceManagement.MediatR
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ServiceResponse<List<UserDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<GetAllUsersQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Appeler le repository pour récupérer tous les utilisateurs
                var users = await _userRepository.All.ToListAsync(cancellationToken);

                // 2. Mapper la liste d'entités User vers une liste de UserDto
                var userDtos = _mapper.Map<List<UserDto>>(users);

                _logger.LogInformation("Successfully retrieved {UserCount} users.", users.Count);

                // 3. Retourner la réponse de succès avec la liste
                return ServiceResponse<List<UserDto>>.ReturnResultWith200(userDtos);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                // Utiliser la méthode factory standard pour une erreur 500
                return ServiceResponse<List<UserDto>>.Return500(ex);
            }
        }
    }
}
// --- USINGS NÉCESSAIRES ---
using AutoMapper;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.MediatR; // Namespace correct pour la Query
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

// --- NAMESPACE CONFORME À LA STRUCTURE ---
namespace CBS.UserServiceManagement.MediatR
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ServiceResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<GetUserByIdQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Appeler le repository pour trouver l'utilisateur par son ID
                var user = await _userRepository.FindAsync(request.Id);

                // 2. Gérer le cas où l'utilisateur n'est pas trouvé
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", request.Id);
                    // Utiliser la méthode factory standard pour une erreur 404
                    return ServiceResponse<UserDto>.Return404($"User with ID {request.Id} not found.");
                }

                // 3. Mapper l'entité User vers un UserDto pour la réponse
                var userDto = _mapper.Map<UserDto>(user);

                _logger.LogInformation("Successfully retrieved user with ID {UserId}.", request.Id);

                // 4. Retourner une réponse de succès standard
                return ServiceResponse<UserDto>.ReturnResultWith200(userDto);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with ID {UserId}.", request.Id);
                // Utiliser la méthode factory standard pour une erreur 500
                return ServiceResponse<UserDto>.Return500(ex);
            }
        }
    }
}
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
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

// --- CORRECTION : Le namespace est conservé tel quel, conformément au modèle ---
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
            try
            {
                var user = await _userRepository.FindBy(u => u.Email == request.Email)
                    .FirstOrDefaultAsync(cancellationToken);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    // --- CORRECTION : Utilisation du template de message pour le logging ---
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
                
                // --- CORRECTION : Utilisation du template de message pour le logging ---
                _logger.LogInformation("User {Email} logged in successfully.", user.Email);

                return ServiceResponse<LoginResponse>.ReturnResultWith200(response, "Credentials are valid. Token obtained from UserServiceManagement.");
            }
            catch (Exception ex)
            {
                // --- CORRECTION : Utilisation du template de message pour le logging ---
                _logger.LogError(ex, "An unexpected error occurred during login for email: {Email}", request.Email);
                return ServiceResponse<LoginResponse>.Return500(ex);
            }
        }
    }
}
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using MediatR;
using System.Collections.Generic;

namespace CBS.UserServiceManagement.MediatR
{
    public class GetAllUsersQuery : IRequest<ServiceResponse<List<UserDto>>>
    {
    }
}
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    public class GetUserByIdQuery : IRequest<ServiceResponse<UserDto>>
    {
        public string Id { get; set; }
    }
}
// Source: Votre modèle AddUserCommandValidator.cs
using FluentValidation;
using CBS.UserServiceManagement.MediatR;



namespace CBS.UserServiceManagement.MediatR
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            // Règle pour le prénom
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Le prénom est requis.")
                .MaximumLength(100).WithMessage("Le prénom ne peut pas dépasser 100 caractères.")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Le prénom ne peut contenir que des lettres, des espaces, des apostrophes et des tirets.");
               

            // Règle pour le nom de famille
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Le nom est requis.")
                .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères.")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Le nom ne peut contenir que des lettres, des espaces, des apostrophes et des tirets.");
                

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
using FluentValidation;
using CBS.UserServiceManagement.MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required.")
                .NotNull().WithMessage("User ID cannot be null.");
        }
    }
}
using FluentValidation;
using CBS.UserServiceManagement.MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est requis.")
                .EmailAddress().WithMessage("Un format d'email valide est requis.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est requis.");
        }
    }
}
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