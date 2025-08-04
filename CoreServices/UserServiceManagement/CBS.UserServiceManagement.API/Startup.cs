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