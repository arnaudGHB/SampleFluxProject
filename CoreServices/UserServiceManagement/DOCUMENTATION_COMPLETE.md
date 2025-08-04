# Documentation complète du projet UserServiceManagement

## Structure du projet

### Couche API

### Contrôleurs

#### BaseController.cs
```csharp
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
    }
}
```

#### UsersController.cs
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

### Contrôleurs API

#### UsersController.cs
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

### Middlewares

#### JwtAuthenticationConfigurationExtension.cs
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
```

#### ExceptionHandlingMiddleware.cs
```csharp
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
```

#### RequestResponseLoggingMiddleware.cs
```csharp
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

### Configuration et démarrage

#### Program.cs
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
```

#### Startup.cs
```csharp
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

            // Configuration de la base de données
            services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Injection des dépendances
            services.AddInfrastructureServices();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddUserCommand).Assembly));
            services.AddSingleton(provider => MapperConfig.GetMapperConfigs());
            services.AddValidatorsFromAssembly(typeof(AddUserCommandValidator).Assembly);

            // Configuration de la sécurité
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

            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddEndpointsApiExplorer();

            // Configuration Swagger
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

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
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<AuditLogMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```

#### JwtSettings.cs
```csharp
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

### Couche MediatR

#### AddUserCommandHandler.cs
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

#### AddUserCommandValidator.cs
```csharp
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

#### LoginUserCommandHandler.cs
```csharp
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
