using CBS.ProjectManagement.API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
var configuration = builder.Configuration;

// Add Serilog for logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting web application");

    // Add services to the container.
    builder.Services.AddControllers();
    
    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Project Management API", Version = "v1" });
    });

    // Add project management services
    builder.Services.AddProjectManagementServices(configuration);

    // Add health checks
    builder.Services.AddHealthChecksUI()
        .AddInMemoryStorage();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management API V1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at the root
        });
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    
    // Add middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<RequestResponseLoggingMiddleware>();
    app.UseMiddleware<SecurityHeadersMiddleware>();
    app.UseMiddleware<AuditLogMiddleware>();
    
    app.UseAuthentication();
    app.UseAuthorization();

    // Map controllers
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHealthChecksUI();
    });

    // Health check endpoint
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
