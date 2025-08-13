// Source: Mod�le original, corrig� pour �tre coh�rent avec l'entit� User.
using System;

namespace CBS.UserServiceMGT.Data // J'ajuste le namespace pour la coh�rence
{
    public class UserDto
    {
        // Correction : Le type de l'ID doit correspondre � celui de l'entit� (string).
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
