using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using CBS.APICaller.Helper;
using MediatR;
using FluentValidation;
using CBS.BankMGT.MediatR.PipeLineBehavior;
using CBS.BankMGT.Domain;
using CBS.BankMGT.API.Helpers;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Data.Dto;
using Microsoft.AspNetCore.Hosting;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.CustomLog.Logger.CustomLogger;
using Microsoft.Extensions.Hosting;
using CBS.ServicesDelivery.Service.ServiceDiscovery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CBS.BankMGT.Domain.Context;
using CBS.BankMGT.Common.Repository.Uow;
using CBS.BankMGT.Common.Repository.Generic;
using CBS.BankMGT.Common.DBConnection;

namespace CBS.BankMGT.API
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
            var assembly = AppDomain.CurrentDomain.Load("CBS.BankMGT.MediatR");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblies(Enumerable.Repeat(assembly, 1));
            services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
            services.Configure<FileLoggerOptions>(Configuration.GetSection("BiosLogger:Options"));
            services.AddConsulConfig(Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            JwtSettings settings = GetJwtSettings();
            services.AddSingleton(settings);
            services.AddSingleton(new PathHelper(Configuration));
            services.AddScoped<UserInfoToken>(c => new UserInfoToken() { Id = string.Empty });

            services.AddDbContext<POSContext>(options =>
            {
                options.UseSqlServer(Configuration["Conn:POSDbConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });


            // MongoDB Connection String and Database Name
            var mongoConnectionString = Configuration.GetSection("MongoDB").GetSection("DatabaseConnectionString").Value;
            var mongoDatabaseName = Configuration.GetSection("MongoDB").GetSection("DatabaseName").Value;
            services.AddSingleton<IMongoDbConnection>(sp => new MongoDbConnection(mongoConnectionString, mongoDatabaseName));


            services.AddSingleton(MapperConfig.GetMapperConfigs());
            services.AddDependencyInjection();

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
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
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
                    Title = "Bank Configuration Micro Service API"
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

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(c =>
            {
                c.DefaultModelsExpandDepth(-1);
                c.SwaggerEndpoint($"v1/swagger.json", "Template");
                c.RoutePrefix = "swagger";
            });

            try
            {
                // Apply database migrations on startup
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<POSContext>();
                    //dbContext.Database.Migrate();
                    //DbInitializer.Initialize(dbContext);
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
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<AuditLogMiddleware>();
            app.UseMiddleware<JWTMiddleware>();
            //app.UseAuthorization();
            app.UseResponseCompression();
            app.UseConsul(Configuration);
          

            //app.UseCors("ExposeResponseHeaders");


            //app.UseRouting();
            //app.UseConsul(Configuration);
            //app.UseMiddleware<RequestResponseLoggingMiddleware>();
            //app.UseMiddleware<JWTMiddleware>();
            ////app.UseMiddleware<AuditLogMiddleware>();
            //app.UseAuthorization();
            //app.UseResponseCompression();

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
