using CBS.DailyCollectionManagement.API.Helpers;
using CBS.DailyCollectionManagement.Common;
using CBS.DailyCollectionManagement.Common.DBConnection;
using CBS.DailyCollectionManagement.Data.Dto;
using CBS.DailyCollectionManagement.Domain;
using CBS.DailyCollectionManagement.Helper;
using CBS.DailyCollectionManagement.MediatR.PipeLineBehavior;
using CBS.DailyCollectionManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.ServicesDelivery.Service.ServiceDiscovery;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;


namespace CBS.DailyCollectionManagement.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("CBS.DailyCollectionManagement.MediatR");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblies(Enumerable.Repeat(assembly, 1));
            services.AddConsulConfig(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            JwtSettings settings = GetJwtSettings();
            services.AddSingleton(settings);
            services.AddSingleton(new PathHelper(Configuration));

            services.AddDbContext<DailyCollectionContext>(options =>
            {
                options.UseSqlServer(Configuration["Conn:POSDbConnectionString"], sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });

            //services.AddScoped<UserInfoToken>(c => new UserInfoToken() { Id = string.Empty });
            services.AddSingleton<UserInfoToken>();
            // MongoDB Connection String and Database Name
            var mongoConnectionString = Configuration.GetSection("MongoDB").GetSection("DatabaseConnectionString").Value;
            var mongoDatabaseName = Configuration.GetSection("MongoDB").GetSection("DatabaseName").Value;

            services.AddSingleton<IMongoDbConnection>(sp => new MongoDbConnection(mongoConnectionString, mongoDatabaseName));
   
            services.AddHttpClient();
            services.AddSingleton(MapperConfig.GetMapperConfigs());
            services.AddDependencyInjection();

            //services.AddScoped<BackgroundServiceState>();
            //services.AddScoped<ReconciliationWorkerService>();
            //services.AddHostedService<ReconciliationWorkerService>();
            //services.AddHealthChecks().AddCheck<ReconciliationBackgroundServiceHealthCheck>("background_service_health_check", tags: new[] { "background_service" });

            // Add distributed memory cache
            services.AddDistributedMemoryCache();

            // Add session services
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only
                options.Cookie.IsEssential = true; // Make the session cookie essential
            });
            // JWT Authentication Configuration
            var key = Encoding.UTF8.GetBytes(settings.Key);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("ExposeResponseHeaders",
                    builder =>
                    {
                        builder.WithOrigins()
                               .WithExposedHeaders("X-Pagination")
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .WithMethods("POST", "PUT", "PATCH", "GET", "DELETE")
                               .SetIsOriginAllowed(host => true);
                    });
            });

            services.AddSignalR();

            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DailyCollectionManagement Micro Service API"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
               {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  new string[] { }
                }
            });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                string message = "";
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
                            message = exceptionHandlerFeature.Error.Message;
                        }
                        context.Response.StatusCode = 500;
                      
                        await context.Response.WriteAsync("An unexpected fault happened."+message+" Try again later.");
                    });
                });
            }

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(c =>
            {
                c.DefaultModelsExpandDepth(-1);
                c.SwaggerEndpoint($"v1/swagger.json", "DailyCollection Management Services");
                c.RoutePrefix = "swagger";
            });

            try
            {
                // Apply database migrations on startup
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DailyCollectionContext>();
                    dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger($"Failed creating database with errors: {ex.Message}");
                throw ex;
            }

            app.UseStaticFiles();
            app.UseCors("ExposeResponseHeaders");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();
            app.UseSession();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<AuditLogMiddleware>();
            app.UseMiddleware<JWTMiddleware>();
            app.UseAuthorization();
            app.UseResponseCompression();
            app.UseConsul(Configuration);
       
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

          
        }

        public JwtSettings GetJwtSettings()
        {
            JwtSettings settings = new JwtSettings
            {
                Key = Configuration["JwtSettings:key"],
                Audience = Configuration["JwtSettings:audience"],
                Issuer = Configuration["JwtSettings:issuer"],
                MinutesToExpiration = Convert.ToInt32(Configuration["JwtSettings:minutesToExpiration"])
            };
            return settings;
        }
    }
}