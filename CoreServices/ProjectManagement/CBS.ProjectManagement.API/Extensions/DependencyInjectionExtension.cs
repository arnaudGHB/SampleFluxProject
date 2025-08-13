using System.Reflection;
using AutoMapper;
using CBS.ProjectManagement.Common;
using CBS.ProjectManagement.Common.Interfaces;
using CBS.ProjectManagement.Domain.Context;
using CBS.ProjectManagement.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CBS.ProjectManagement.API.Extensions
{
    /// <summary>
    /// Extension methods for configuring dependency injection.
    /// </summary>
    public static class DependencyInjectionExtension
    {
        /// <summary>
        /// Adds project management services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection with project management services added.</returns>
        public static IServiceCollection AddProjectManagementServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register database context
            services.AddDbContext<ProjectContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("ProjectManagementConnection"),
                    b => b.MigrationsAssembly(typeof(ProjectContext).Assembly.FullName)));

            // Register repositories
            services.AddScoped<IUnitOfWork<ProjectContext>, UnitOfWork<ProjectContext>>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectAssignmentRepository, ProjectAssignmentRepository>();

            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Register health checks
            services.AddHealthChecks()
                .AddDbContextCheck<ProjectContext>("ProjectManagement Database Health Check");

            return services;
        }
    }
}
