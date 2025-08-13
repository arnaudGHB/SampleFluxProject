using System;
using CBS.UserSkillManagement.Common;
using CBS.UserSkillManagement.Data;
using CBS.UserSkillManagement.Domain;
using CBS.UserSkillManagement.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using AutoMapper;
using CBS.UserSkillManagement.API.Helpers.MapperConfiguration;

namespace CBS.UserSkillManagement.API
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<UserSkillContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(180))); // 3 minutes timeout

            // Register Unit of Work
            services.AddScoped<IUnitOfWork<UserSkillContext>, UnitOfWork<UserSkillContext>>();

            // Register Repositories
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IUserSkillRepository, UserSkillRepository>();
            services.AddScoped<IGenericRepository<AuditLog>>(provider =>
                provider.GetRequiredService<IUnitOfWork<UserSkillContext>>().GetRepository<AuditLog>());

            // Add AutoMapper
            services.AddMapperConfiguration();

            // Add MediatR
            services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));

            // Add HttpContextAccessor if not already added
            services.AddHttpContextAccessor();

            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins(
                                "http://localhost:3000", // React
                                "http://localhost:4200")  // Angular
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

            // Add Health Checks
            services.AddHealthChecks()
                .AddDbContextCheck<UserSkillContext>("Database Health Check");

            return services;
        }
    }
}
