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
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.API.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Add")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 409)]
        public async Task<IActionResult> AddUser([FromBody] AddUserCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}

using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Repository;
using Microsoft.Extensions.DependencyInjection;
using CBS.UserServiceManagement.API.Helpers;
using CBS.UserServiceManagement.MediatR;
using FluentValidation;
using CBS.UserServiceManagement.MediatR;
using CBS.UserServiceManagement.Data;

namespace CBS.UserServiceManagement.API.Helpers
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<UserInfoToken>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddUserCommand).Assembly));
            services.AddSingleton(provider => MapperConfig.GetMapperConfigs());
            services.AddValidatorsFromAssembly(typeof(AddUserCommandValidator).Assembly);
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

namespace CBS.UserServiceManagement.API.Helpers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<AddUserCommand, User>();
        }
    }
}
namespace CBS.UserServiceManagement.API.Helpers
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
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
using CBS.UserServiceManagement.API.Helpers;
using CBS.UserServiceManagement.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
using CBS.UserServiceManagement.API.Helpers;
using CBS.UserServiceManagement.API.Middlewares;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CBS.UserServiceManagement.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Cette méthode est appelée par le runtime. Utilisez-la pour ajouter des services au conteneur.
        public void ConfigureServices(IServiceCollection services)
        {
            // 1. Configuration du DbContext
            services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // 2. Appel à notre extension centralisée pour la DI
            services.AddDependencyInjection();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddUserCommand).Assembly));
            services.AddSingleton(provider => MapperConfig.GetMapperConfigs());
            services.AddValidatorsFromAssembly(typeof(AddUserCommandValidator).Assembly);

            // 3. Ajout des services de base de l'API
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        // Cette méthode est appelée par le runtime. Utilisez-la pour configurer le pipeline de requêtes HTTP.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            
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
    public class Claim
    {
        public string claimType { get; set; }
        public string claimValue { get; set; }
    }
}
namespace CBS.UserServiceManagement.Helper
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
// Source: Votre code, ajusté pour retourner le type correct.
using CBS.UserServiceManagement.Data;
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
// Source: Basé sur votre modèle AddCountryCommandHandler.
using AutoMapper;
using BCrypt.Net;
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;

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

namespace CBS.UserServiceManagement.MediatR
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, ServiceResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork<UserContext> _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<AddUserCommandHandler> _logger;
        // TODO: Injecter un service de hachage de mot de passe (IPasswordHasher)

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
                // 1. Vérifier si l'utilisateur existe déjà
                var emailExists = await _userRepository.FindBy(u => u.Email.ToLower() == request.Email.ToLower()).AnyAsync(cancellationToken);
                if (emailExists)
                {
                    var errorMessage = $"A user with the email '{request.Email}' already exists.";
                    _logger.LogWarning(errorMessage);
                    return ServiceResponse<UserDto>.Return409(errorMessage);
                }

                // 2. Hacher le mot de passe (logique de sécurité critique)
                // REMPLACER par un vrai service de hachage comme BCrypt.Net
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                // Pour que cela compile, installez le package: dotnet add package BCrypt.Net-Next

                // 3. Créer la nouvelle entité User
                var newUser = new User(
                    id: BaseUtilities.GenerateUniqueNumber(36),
                    firstName: request.FirstName,
                    lastName: request.LastName,
                    email: request.Email,
                    passwordHash: passwordHash
                );

                // 4. Ajouter l'entité et sauvegarder la transaction
                _userRepository.Add(newUser);
                await _uow.SaveAsync();

                // 5. Mapper l'entité créée vers le DTO de réponse
                var userDto = _mapper.Map<UserDto>(newUser);

                _logger.LogInformation($"Successfully created user with ID {newUser.Id}.");
                return ServiceResponse<UserDto>.ReturnResultWith200(userDto, "User created successfully.");
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
## Architecture Technique

### Couche API
- **Authentification** :
  - JWT avec clé secrète configurable
  - Durée de validité : 60 minutes
  - Claims supportés : Id, Email, Role

- **Endpoints** :
  - `POST /api/auth/login` (AllowAnonymous)
  - `POST /api/users/add` (Authorize Admin)

- **Middlewares** :
  1. Exception Handling
  2. JWT Validation
  3. Authorization
  4. Audit Logging

### Couche Domain
- **UserContext** :
  - DbSets : Users, AuditLogs
  - Configuration EF Core
  - Migrations à jour

### Couche Data
- **Entités** :
  - User (FirstName, LastName, Email, Role)
  - AuditLog (tracking complet)