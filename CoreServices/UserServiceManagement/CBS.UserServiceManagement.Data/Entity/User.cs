// Source: Modèle implicite, avec l'ID défini directement dans l'entité.
using System;
using System.ComponentModel.DataAnnotations; // Ajout pour l'attribut [Key]

namespace CBS.UserServiceManagement.Data
{
    public class User : BaseEntity
    {
        [Key] // Déclare cette propriété comme étant la clé primaire de l'entité User.
        public string Id { get; set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }

        // Constructeur privé pour EF Core
        private User() { }

        // Constructeur public pour la création, incluant l'ID.
        public User(string id, string firstName, string lastName, string email, string passwordHash, string role = "User")
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }

        // Méthodes pour mettre à jour l'entité de manière contrôlée
        public void UpdateProfile(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }
    }
}