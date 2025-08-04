using CBS.UserServiceManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace CBS.UserServiceManagement.Domain
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
            // L'injection de la chaîne de connexion se fera dans Program.cs
            // pour plus de flexibilité, mais le principe reste le même.
        }

        // Déclarez un DbSet pour chaque entité gérée par ce microservice.
        // Ici, nous n'avons que l'entité User.
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des contraintes et relations pour l'entité User.
            modelBuilder.Entity<User>(entity =>
            {
                // S'assurer que l'ID est bien la clé primaire.
                // Votre modèle de BaseEntity n'a pas d'Id, donc nous le précisons ici.
                // Si l'ID était dans BaseEntity, cette ligne serait inutile.
                entity.HasKey(e => e.Id);

                // Définir l'index unique pour l'email.
                entity.HasIndex(u => u.Email).IsUnique();

                // Configurer les propriétés pour éviter les longueurs infinies en base de données.
                entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
                entity.Property(u => u.Role).HasMaxLength(50).IsRequired();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Activation du logging des données sensibles (très utile en développement).
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
