using CBS.UserSkillManagement.Data;
using CBS.UserSkillManagement.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CBS.UserSkillManagement.API
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;

        // Le constructeur est simple et n'injecte PAS de services Scoped.
        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Le DbContext (qui est un service Scoped) est injecté EN PARAMÈTRE de la méthode InvokeAsync.
        // Le conteneur de DI le fournira à chaque requête.
        public async Task InvokeAsync(HttpContext context, UserSkillContext dbContext)
        {
            // Skip logging for health checks and swagger
            if (context.Request.Path.StartsWithSegments("/health") || 
                context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            context.Request.EnableBuffering();
            
            await _next(context);

            var request = context.Request;
            
            // On audite uniquement les opérations de modification.
            if (request.Method == HttpMethods.Post || request.Method == HttpMethods.Put || request.Method == HttpMethods.Delete)
            {
                // On vérifie que le middleware JWT a bien identifié un utilisateur.
                if (context.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is string userId)
                {
                    var auditLog = new AuditLog
                    {
                        Id = new Guid(),
                        UserId = userId,
                        UserEmail = context.Items["Email"]?.ToString(),
                        EntityName = request.RouteValues["controller"]?.ToString(),
                        Action = request.Method,
                        Timestamp = DateTime.UtcNow,
                        Changes = await GetChangesAsync(request),
                        IPAddress = context.Connection.RemoteIpAddress?.ToString(),
                        Url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}"
                    };

                    // On utilise le DbContext injecté dans la méthode pour sauvegarder.
                    dbContext.AuditLogs.Add(auditLog);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task<string> GetChangesAsync(HttpRequest request)
        {
            if (request.Method == HttpMethods.Delete)
            {
                return $"Deleted resource with ID: {request.RouteValues["id"]?.ToString()}";
            }

            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var bodyAsText = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            
            return bodyAsText;
        }
    }
}
