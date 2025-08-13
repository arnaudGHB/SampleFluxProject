using CBS.UserSkillManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.UserSkillManagement.Domain
{
    public class UserSkillContext : DbContext
    {
        public UserSkillContext(DbContextOptions<UserSkillContext> options) : base(options)
        {
        }

        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Skill entity
            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).IsRequired();
            });

            // Configure UserSkill entity
            modelBuilder.Entity<UserSkill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Level).IsRequired();
                
                // Configure relationship
                entity.HasOne(us => us.Skill)
                    .WithMany(s => s.UserSkills)
                    .HasForeignKey(us => us.SkillId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure AuditLog entity
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.TableName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IpAddress).HasMaxLength(200);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var utcNow = DateTime.UtcNow;
            var currentUser = "system"; // This should be replaced with the actual user from the current context

            foreach (var entry in entries)
            {
                if (entry.Entity is BaseEntity trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.ModifiedDate = utcNow;
                            trackable.ModifiedBy = currentUser;
                            entry.Property("CreatedDate").IsModified = false;
                            entry.Property("CreatedBy").IsModified = false;
                            break;

                        case EntityState.Added:
                            trackable.CreatedDate = utcNow;
                            trackable.CreatedBy = currentUser;
                            trackable.ModifiedDate = utcNow;
                            trackable.ModifiedBy = currentUser;
                            break;
                    }
                }
            }
        }
    }
}
