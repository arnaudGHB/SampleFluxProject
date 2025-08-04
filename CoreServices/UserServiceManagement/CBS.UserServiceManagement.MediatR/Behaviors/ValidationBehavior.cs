using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// --- NAMESPACE CONFORME À LA STRUCTURE ---
namespace CBS.UserServiceManagement.MediatR
{
    // Ce behavior s'exécutera pour chaque requête MediatR (TRequest) qui retourne une réponse (TResponse).
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        // On injecte tous les validateurs que FluentValidation connaît pour la requête TRequest.
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // S'il n'y a pas de validateur pour cette requête, on passe directement au handler.
            if (!_validators.Any())
            {
                return await next();
            }

            // Créer un contexte de validation pour FluentValidation.
            var context = new ValidationContext<TRequest>(request);

            // Exécuter tous les validateurs en parallèle et récupérer les résultats.
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Agréger toutes les erreurs de validation en une seule liste.
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            // S'il y a au moins une erreur de validation...
            if (failures.Any())
            {
                // ... on lève une ValidationException.
                // Cela interrompt le pipeline et empêche l'exécution du handler.
                throw new ValidationException(failures);
            }

            // Si aucune erreur n'a été trouvée, on exécute le prochain maillon du pipeline (le handler).
            return await next();
        }
    }
}