using CBS.ProjectManagement.Data.Entity;
using CBS.ProjectManagement.Common;
using Microsoft.EntityFrameworkCore;

namespace CBS.ProjectManagement.Domain.Context
{
    /// <summary>
    /// Represents the database context for the Project Management domain.
    /// </summary>
    public class ProjectContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Projects DbSet.
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// Gets or sets the ProjectAssignments DbSet.
        /// </summary>
        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }

        /// <summary>
        /// Configures the schema needed for the project management context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Project entity
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(p => p.ProjectId);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.HasIndex(p => p.Name).IsUnique();
                entity.Property(p => p.Description).HasMaxLength(1000);
                entity.Property(p => p.Status).HasDefaultValue(ProjectStatus.Active);
                
                // Configure the one-to-many relationship with ProjectAssignment
                entity.HasMany(p => p.ProjectAssignments)
                      .WithOne(pa => pa.Project)
                      .HasForeignKey(pa => pa.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ProjectAssignment entity
            modelBuilder.Entity<ProjectAssignment>(entity =>
            {
                entity.HasKey(pa => pa.AssignmentId);
                
                // Create a unique index on the combination of ProjectId and UserId
                entity.HasIndex(pa => new { pa.ProjectId, pa.UserId })
                      .IsUnique();
                
                // Configure the relationship with Project
                entity.HasOne(pa => pa.Project)
                      .WithMany(p => p.ProjectAssignments)
                      .HasForeignKey(pa => pa.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure BaseEntity properties
            modelBuilder.Entity<BaseEntity>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedDate)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAddOrUpdate();
            });
        }
    }
}
