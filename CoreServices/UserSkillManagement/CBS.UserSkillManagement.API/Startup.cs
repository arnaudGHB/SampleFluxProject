using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;

using CBS.UserSkillManagement.API.Helpers.MapperConfiguration;
using CBS.UserSkillManagement.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CBS.UserSkillManagement.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            // Add JWT Authentication
            services.AddJwtAuthentication(Configuration);

            // Configure CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                        .WithOrigins("http://localhost:3000", "http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "User Skill Management API", 
                    Version = "v1",
                    Description = "API for managing user skills and skill assignments",
                    Contact = new OpenApiContact
                    {
                        Name = "Support",
                        Email = "support@cbs.com",
                        Url = new Uri("https://cbs.com/support")
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // Add JWT Authentication to Swagger
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
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new string[] {}
                    }
                });
            });

            // Add application services
            services.AddDependencyInjection(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Skill Management API V1");
                c.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Global Exception Handling
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Security Headers
            app.UseSecurityHeaders();

            // Request/Response Logging
            app.UseRequestResponseLogging();

            // Audit Logging
            app.UseMiddleware<AuditLogMiddleware>();

            // Enable CORS
            app.UseCors("AllowSpecificOrigin");

            // Enable routing
            app.UseRouting();

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                // Health check endpoint
                endpoints.MapHealthChecks("/health");
                
                // Fallback to controller actions for SPA
                endpoints.MapFallbackToController("Index", "Home");
            });
        }
    }
}
