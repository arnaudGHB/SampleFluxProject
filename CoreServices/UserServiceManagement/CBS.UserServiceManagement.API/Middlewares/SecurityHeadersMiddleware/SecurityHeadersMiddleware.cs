// Fichier : CoreServices/UserServiceManagement/CBS.UserServiceManagement.API/Middlewares/SecurityHeadersMiddleware/SecurityHeadersMiddleware.cs

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.API.Middlewares.SecurityHeadersMiddleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Ajoute l'en-tête X-Content-Type-Options pour empêcher le "MIME type sniffing".
            // Cela réduit le risque d'attaques où un fichier est interprété avec un type de contenu incorrect.
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // Ajoute l'en-tête X-Frame-Options pour se protéger contre le "clickjacking".
            // 'DENY' empêche la page d'être affichée dans une frame ou iframe.
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // Ajoute l'en-tête Content-Security-Policy (CSP) pour contrôler les ressources
            // que le navigateur est autorisé à charger pour la page.
            // "default-src 'self'" est une politique restrictive qui n'autorise que les ressources provenant de la même origine.
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; frame-ancestors 'none';");

            // Ajoute l'en-tête Referrer-Policy pour contrôler la quantité d'informations de "referrer"
            // envoyées avec les requêtes. "no-referrer" empêche l'envoi de cet en-tête.
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");

            // Ajoute l'en-tête Permissions-Policy pour contrôler l'accès aux fonctionnalités du navigateur.
            // Ici, nous désactivons explicitement la géolocalisation.
            context.Response.Headers.Append("Permissions-Policy", "geolocation=()");

            // Ajoute l'en-tête X-XSS-Protection pour activer le filtre anti-XSS des navigateurs.
            // "1; mode=block" active la protection et empêche le rendu de la page si une attaque est détectée.
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Continue le traitement de la requête dans le pipeline
            await _next(context);
        }
    }
}