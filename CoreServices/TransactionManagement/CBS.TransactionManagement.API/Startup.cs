using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using CBS.TransactionManagement.Domain;
using CBS.APICaller.Helper;
using CBS.TransactionManagement.Helper;
using MediatR;
using CBS.TransactionManagement.MediatR.PipeLineBehavior;
using FluentValidation;
using CBS.TransactionManagement.API.Helpers;
using CBS.TransactionManagement.Dto;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using Hangfire;
using CBS.ServicesDelivery.Service.ServiceDiscovery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CBS.TransactionManagement.Repository.DatabaseLogging;
using CBS.TransactionManagement.Common.DBConnection;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace CBS.TransactionManagement.API
{
    //public class Startup
    //{
    //    public Startup(IConfiguration configuration)
    //    {
    //        Configuration = configuration;
    //    }

    //    public IConfiguration Configuration { get; }

    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //        var assembly = AppDomain.CurrentDomain.Load("UserManagement.MediatR");
    //        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

    //        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

    //        var jwtConfig = Configuration.GetSection("JwtSettings");
    //        var key = Encoding.UTF8.GetBytes(jwtConfig["key"]);

    //        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //            .AddJwtBearer(options =>
    //            {
    //                options.RequireHttpsMetadata = false;
    //                options.SaveToken = true;
    //                options.TokenValidationParameters = new TokenValidationParameters
    //                {
    //                    ValidateIssuer = true,
    //                    ValidIssuer = jwtConfig["issuer"],
    //                    ValidateAudience = true,
    //                    ValidAudience = jwtConfig["audience"],
    //                    ValidateLifetime = true,
    //                    ValidateIssuerSigningKey = true,
    //                    IssuerSigningKey = new SymmetricSecurityKey(key),
    //                    ClockSkew = TimeSpan.Zero
    //                };
    //            });

    //        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    //        services.AddValidatorsFromAssemblies(new[] { assembly });

    //        // Use Redis instead of in-memory cache for scalability
    //        //services.AddStackExchangeRedisCache(options =>
    //        //{
    //        //    options.Configuration = Configuration.GetConnectionString("RedisCache");
    //        //});

    //        services.AddHttpContextAccessor();
    //        services.AddDistributedMemoryCache();

    //        // ✅ Improved Session Configuration
    //        services.AddSession(options =>
    //        {
    //            options.IdleTimeout = TimeSpan.FromMinutes(30);
    //            options.Cookie.HttpOnly = true;
    //            options.Cookie.IsEssential = true;
    //        });

    //        // Optimize DbContext
    //        services.AddDbContextPool<UserContext>(options =>
    //        {
    //            options.UseSqlServer(Configuration.GetConnectionString("UserDbConnectionString"));
    //            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // Improve performance
    //        });

    //        // Configure Identity
    //        services.AddIdentity<User, Role>()
    //            .AddEntityFrameworkStores<UserContext>()
    //            .AddDefaultTokenProviders();

    //        services.Configure<IdentityOptions>(options =>
    //        {
    //            options.Password.RequireDigit = false;
    //            options.Password.RequiredLength = 5;
    //            options.Password.RequireNonAlphanumeric = false;
    //            options.Password.RequireUppercase = false;
    //            options.Password.RequireLowercase = false;
    //        });

    //        // Register Services
    //        services.AddSingleton(MapperConfig.GetMapperConfigs());
    //        services.AddSingleton<PathHelper>(new PathHelper(Configuration));
    //        services.AddSingleton<IConnectionMappingRepository, ConnectionMappingRepository>();
    //        services.AddScoped<UserInfoToken>();

    //        // CORS Policy Optimization ✅
    //        services.AddCors(options =>
    //        {
    //            options.AddPolicy("ExposeResponseHeaders", builder =>
    //            {
    //                builder.WithOrigins("https://your-allowed-domain.com") // Restrict origins
    //                       .WithExposedHeaders("X-Pagination")
    //                       .AllowAnyHeader()
    //                       .AllowCredentials()
    //                       .WithMethods("POST", "PUT", "PATCH", "GET", "DELETE");
    //            });
    //        });

    //        services.AddSignalR();

    //        services.Configure<IISServerOptions>(options =>
    //        {
    //            options.AutomaticAuthentication = false;
    //        });

    //        // Enable Response Compression ✅
    //        services.AddResponseCompression(options =>
    //        {
    //            options.Providers.Add<GzipCompressionProvider>();
    //        });

    //        // Configure Controllers
    //        services.AddControllers()
    //            .AddNewtonsoftJson(options =>
    //            {
    //                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    //                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
    //            });

    //        // Configure Swagger ✅ (Load XML Comments Conditionally)
    //        services.AddSwaggerGen(c =>
    //        {
    //            c.SwaggerDoc("v1", new OpenApiInfo
    //            {
    //                Version = "v1",
    //                Title = "User Management API"
    //            });

    //            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //            {
    //                Description = "JWT Authorization header using the Bearer scheme.\n\nExample: \"Bearer 12345abcdef\"",
    //                Name = "Authorization",
    //                In = ParameterLocation.Header,
    //                Type = SecuritySchemeType.ApiKey,
    //                Scheme = "Bearer"
    //            });

    //            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    //            {
    //                new OpenApiSecurityScheme
    //                {
    //                    Reference = new OpenApiReference
    //                    {
    //                        Type = ReferenceType.SecurityScheme,
    //                        Id = "Bearer"
    //                    }
    //                },
    //                new string[] { }
    //            }
    //        });

    //            // ✅ Load XML comments only if file exists
    //            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //            if (File.Exists(xmlPath))
    //            {
    //                c.IncludeXmlComments(xmlPath);
    //            }
    //        });

    //        SpaStartup.ConfigureServices(services);
    //    }

    //    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env)
    //    {
    //        if (env.IsDevelopment())
    //        {
    //            app.UseDeveloperExceptionPage();
    //        }
    //        else
    //        {
    //            app.UseExceptionHandler(appBuilder =>
    //            {
    //                appBuilder.Run(async context =>
    //                {
    //                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    //                    if (exceptionHandlerFeature != null)
    //                    {
    //                        var logger = loggerFactory.CreateLogger("Global exception logger");
    //                        logger.LogError(exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
    //                    }
    //                    context.Response.StatusCode = 500;
    //                    await context.Response.WriteAsync("An unexpected error occurred.");
    //                });
    //            });
    //        }

    //        app.UseSwagger();
    //        app.UseSwaggerUI(c =>
    //        {
    //            c.DefaultModelsExpandDepth(-1);
    //            c.SwaggerEndpoint($"/swagger/v1/swagger.json", "User Management API");
    //            c.RoutePrefix = "swagger";
    //        });

    //        // ✅ Middleware Execution Order Optimized
    //        app.UseSession(); // Moved higher for better performance 🚀
    //        app.UseCors("ExposeResponseHeaders");
    //        app.UseHttpsRedirection();
    //        app.UseAuthentication();
    //        app.UseRouting();
    //        //app.UseMiddleware<AuditLogMiddleware>();
    //        app.UseMiddleware<JWTMiddleware>();
    //        app.UseAuthorization();
    //        app.UseResponseCompression();

    //        // ✅ Log Request Processing Time
    //        app.Use(async (context, next) =>
    //        {
    //            var stopwatch = Stopwatch.StartNew();
    //            await next.Invoke();
    //            stopwatch.Stop();
    //            Console.WriteLine($"Request took: {stopwatch.ElapsedMilliseconds}ms");
    //        });

    //        app.UseEndpoints(endpoints =>
    //        {
    //            endpoints.MapControllers();
    //            endpoints.MapHub<UserHub>("/userHub");
    //        });

    //        SpaStartup.Configure(app);
    //    }

    //    public JwtSettings GetJwtSettings()
    //    {
    //        return new JwtSettings
    //        {
    //            Key = Configuration["JwtSettings:key"],
    //            Audience = Configuration["JwtSettings:audience"],
    //            Issuer = Configuration["JwtSettings:issuer"],
    //            MinutesToExpiration = Convert.ToInt32(Configuration["JwtSettings:minutesToExpiration"])
    //        };
    //    }
    //}



    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("CBS.TransactionManagement.MediatR");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            var jwtConfig = Configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtConfig["key"]);

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
                    ValidIssuer = jwtConfig["issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtConfig["audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblies(Enumerable.Repeat(assembly, 1));
            services.AddHttpContextAccessor(); // Register IHttpContextAccessor
            services.AddControllersWithViews();
            // Add distributed memory cache
            services.AddDistributedMemoryCache();
            // Add session services
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only
                options.Cookie.IsEssential = true; // Make the session cookie essential
            });
            services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            // Retrieve the connection string from the Logging section
            var connectionStringLogger = Configuration.GetSection("Logging:Database:ConnectionString").Value;

            // Use the retrieved connection string to configure the DbContext
            services.AddDbContext<TransactionContext>(options =>
                options.UseSqlServer(connectionStringLogger));

            // Add the custom Database Logger
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddDbLogger();
            });
            var connectionString = Configuration.GetSection("Conn:POSDbConnectionString").Value;
            services.AddDbContext<TransactionContext>(options => options.UseSqlServer(connectionString));
            // MongoDB Connection String and Database Name
            var mongoConnectionString = Configuration.GetSection("MongoDB").GetSection("DatabaseConnectionString").Value;
            var mongoDatabaseName = Configuration.GetSection("MongoDB").GetSection("DatabaseName").Value;

            services.AddSingleton<IMongoDbConnection>(sp => new MongoDbConnection(mongoConnectionString, mongoDatabaseName));
          


            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            services.AddHangfireServer();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<JwtSettings>(GetJwtSettings());
            services.AddSingleton<PathHelper>(new PathHelper(Configuration));
           
            services.AddScoped<UserInfoToken>(e => new UserInfoToken() { Id = string.Empty });
            // In ConfigureServices method
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ThirdPartyPolicy", policy =>
                {
                    policy.RequireRole("ThirdPartyProviders");
                    // or use other requirements as needed
                });
            });
            services.AddConsulConfig(Configuration);
            services.AddSingleton(MapperConfig.GetMapperConfigs());
            services.AddDependencyInjection();
            services.AddCors(options =>
            {
                options.AddPolicy("ExposeResponseHeaders",
                    builder =>
                    {

                        builder.WithOrigins()

                        //builder.WithOrigins("http://localhost:5239"
                        //                    //"http://localhost:4200",
                        //                    // "http://localhost:4201",
                        //                    // "https://usermgt.mlglobtech.com",
                        //                    // "https://www.usermgt.mlglobtech.com"
                        //                 )
                               .WithExposedHeaders("X-Pagination")
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .WithMethods("POST", "PUT", "GET", "DELETE")
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
                    Title = "Transaction Management Micro Service API"
                });
                c.AddServer(new OpenApiServer
                {
                    Description = "Development server",
                    Url = "http://localhost:7113"

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
                            logger.LogError(500, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
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
                c.SwaggerEndpoint($"v1/swagger.json", "Transaction Management");
                c.RoutePrefix = "swagger";
            });

            // Database migration and initialization within a scoped context
            using (var scope = app.ApplicationServices.CreateScope())
            {
                
            }

           
            var serviceProvider = app.ApplicationServices;
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TransactionContext>();
                context.Database.Migrate();
                DatabaseInitializerService.Initialize(context);
                var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                loggerFactory.AddProvider(new DatabaseLoggerProvider(context, httpContextAccessor));
            }
            // ✅ Middleware Execution Order Optimized
            app.UseSession(); // Moved higher for better performance 🚀
            app.UseCors("ExposeResponseHeaders");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();
            //app.UseMiddleware<AuditLogMiddleware>();
            app.UseMiddleware<JWTMiddleware>();
            app.UseAuthorization();
            app.UseResponseCompression();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseStaticFiles();
            //app.UseCors("ExposeResponseHeaders");
            ////app.UseHttpsRedirection();
            //app.UseAuthentication();
            //app.UseRouting();
            //// Ensure session is enabled before accessing or modifying session data
            //app.UseSession();
            //app.UseMiddleware<RequestResponseLoggingMiddleware>();
            //app.UseMiddleware<JWTMiddleware>();
            //app.UseAuthorization();
            //app.UseResponseCompression();

            //app.UseConsul(Configuration);
            //app.UseHangfireDashboard("/hangfire");
            //app.UseHangfireServer();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
        }


        public JwtSettings GetJwtSettings()
        {
            return new JwtSettings
            {
                Key = Configuration["JwtSettings:key"],
                Audience = Configuration["JwtSettings:audience"],
                Issuer = Configuration["JwtSettings:issuer"],
                MinutesToExpiration = Convert.ToInt32(Configuration["JwtSettings:minutesToExpiration"])
            };
        }
    }
}
