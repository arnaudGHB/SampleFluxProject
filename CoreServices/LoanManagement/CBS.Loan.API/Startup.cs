using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Helpers.DependencyResolver;
using CBS.NLoan.API.Helpers.MapperConfiguation;
using CBS.NLoan.API.JwtTokenValidation;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.MediatR.PipeLineBehavior;
using FluentValidation;
using Hangfire;
using CBS.NLoan.Helper.Helper;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using CBS.ServicesDelivery.Service.ServiceDiscovery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using Microsoft.AspNetCore.Authorization;
using CBS.NLoan.MediatR.LoanCalculatorHelper.InterestCalculationService;
using CBS.NLoan.MediatR.LoanCalculatorHelper.DeliquencyCalculations;
using CBS.NLoan.MediatR.AuthHelper;
using CBS.NLoan.Domain.MongoDBContext.DBConnection;

namespace CBS.NLoan.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("CBS.NLoan.MediatR");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblies(Enumerable.Repeat(assembly, 1));
            services.AddHangfireServer();
            var conn = Configuration.GetSection("Conn").GetSection("POSDbConnectionString").Value;
            services.AddHangfire(configuration => configuration.UseSqlServerStorage(conn));
            services.AddSingleton<TokenStorageService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            string mongoConnectionString = Configuration["MongoDbSettings:MongoDb"];
            string databaseName = Configuration["MongoDbSettings:DatabaseName"];

            services.AddSingleton<IMongoDbConnection>(provider =>
                new MongoDbConnection(mongoConnectionString, databaseName));

            var jwtConfig = GetJwtSettings();
            services.AddSingleton(jwtConfig);

            services.AddSingleton<PathHelper>(new PathHelper(Configuration));
           
            services.AddDbContext<LoanContext>(options => options.UseSqlServer(conn));
            services.AddScoped<UserInfoToken>(c => new UserInfoToken() { Id = string.Empty });

            services.AddConsulConfig(Configuration);
            services.AddSingleton(MapperConfig.GetMapperConfigs());
            services.AddDependencyInjection();

            services.AddSingleton<BackgroundServiceState>();
            services.AddHostedService<DailyInterestBackgroundService>();
            services.AddHostedService<LoanDelinquencyBackgroundService>();
            services.AddHealthChecks().AddCheck<BackgroundServiceHealthCheck>("background_service_health_check", tags: new[] { "background_service" });
            services.AddHttpContextAccessor(); // Register IHttpContextAccessor
                                               // Add distributed memory cache
            services.AddDistributedMemoryCache();
            // Add session services
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only
                options.Cookie.IsEssential = true; // Make the session cookie essential
            });
            // Other service registrations
            // JWT Authentication Configuration
            var key = Encoding.UTF8.GetBytes(jwtConfig.Key);
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
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtConfig.Audience,
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
                    Title = "Loan Micro Service API"
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

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
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

            app.UseSwagger(c => { c.SerializeAsV2 = true; });
            app.UseSwaggerUI(c =>
            {
                c.DefaultModelsExpandDepth(-1);
                c.SwaggerEndpoint($"v1/swagger.json", "Loan");
                c.RoutePrefix = "swagger";
            });

            try
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LoanContext>();
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
            app.UseHangfireDashboard("/hangfire");
            app.UseHangfireServer();
            app.UseSession();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<JWTMiddleware>();
            app.UseMiddleware<AuditLogMiddleware>();
            app.UseAuthorization();
            app.UseResponseCompression();
            app.UseConsul(Configuration);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health").WithMetadata(new AllowAnonymousAttribute());
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
