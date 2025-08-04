// Source: Modèle original, corrigé pour être cohérent avec l'entité User.
using System;

namespace CBS.UserServiceManagement.Data // J'ajuste le namespace pour la cohérence
{
    public class UserDto
    {
        // Correction : Le type de l'ID doit correspondre à celui de l'entité (string).
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
