using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using CBS.Gateway.API.LogginMiddleWare;
using CBS.Gateway.API.Config;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Ocelot.Provider.Consul;
using CBS.Gateway.API.JTWMiddleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using CBS.Gateway.DataContext;
using CBS.Gateway.DataContext.DBConnection;
using CBS.Gateway.DataContext.Repository.Uow;
using CBS.Gateway.DataContext.Repository.Generic;

namespace CBS.Gateway.API
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
            // Enable PII logging for detailed error messages
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
            services.AddScoped(typeof(IMongoGenericRepository<>), typeof(MongoGenericRepository<>));
            // MongoDB Connection String and Database Name
            var mongoConnectionString = Configuration.GetSection("MongoDB").GetSection("DatabaseConnectionString").Value;
            var mongoDatabaseName = Configuration.GetSection("MongoDB").GetSection("DatabaseName").Value;
            services.AddSingleton<IMongoDbConnection>(sp =>new MongoDbConnection(mongoConnectionString, mongoDatabaseName));
            // Register UnitOfWork as Scoped
          
            // Configure JWT authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Jwt:ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudience = Configuration["Jwt:ValidAudience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // In ConfigureServices method
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ThirdPartyPolicy", policy =>
                {
                    policy.RequireRole("ThirdPartyProviders");
                    // or use other requirements as needed
                });
            });

            services.AddControllers();
            services.AddOcelot().AddConsul();
            services.AddSingleton(new PathHelper(Configuration));

            JwtSettings settings = GetJwtSettings();
            services.AddSingleton(settings);

            services.AddSwaggerForOcelot(Configuration);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = "FLUX TRUST BANK SOFT API Gateway",
                    Description = "This gateway provides a single point of consuming all FTBS microservices.",
                    Contact = new OpenApiContact
                    {
                        Name = "FLUXCM.COM-237650535634",
                        Url = new Uri("https://example.com/contact")
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
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
                        }
                    },
                    new string[] {}
                }
            });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddLogging(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                       .AddFilter("System", LogLevel.Warning)
                       .AddConsole();
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }
            var configuration = new OcelotPipelineConfiguration
            {
                AuthorizationMiddleware = async (context, next) =>
                {
                    await next.Invoke();
                }
            };
           
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c =>
            {
                c.DefaultModelsExpandDepth(-1);
                c.SwaggerEndpoint($"v1/swagger.json", "FLUX-API GATEWAY");
                c.RoutePrefix = "swagger";
            });
            //app.UseSession();
            app.UseHttpsRedirection();
            app.UseSwaggerForOcelotUI();
            app.UseRouting();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<CustomSecurityHeader>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOcelot(configuration);
        }

        public JwtSettings GetJwtSettings()
        {
            JwtSettings settings = new JwtSettings
            {
                Key = Configuration["Jwt:Secret"],
                Audience = Configuration["Jwt:ValidAudience"],
                Issuer = Configuration["Jwt:ValidIssuer"],
                MinutesToExpiration = Convert.ToInt32(Configuration["Jwt:TimeInHours"])
            };
            return settings;
        }
    }


}
