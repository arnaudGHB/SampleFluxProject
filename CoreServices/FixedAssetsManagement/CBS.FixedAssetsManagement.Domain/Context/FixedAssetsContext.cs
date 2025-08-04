using CBS.FixedAssetsManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace CBS.FixedAssetsManagement.Domain
{
    public class FixedAssetsContext : DbContext
    {
        public FixedAssetsContext(DbContextOptions<FixedAssetsContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<DepreciationMethod> DepreciationMethods { get; set; }
        public DbSet<DepreciationEntry> DepreciationEntries { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }
        public DbSet<AssetTransfer> AssetTransfers { get; set; }
        public DbSet<AssetDisposal> AssetDisposals { get; set; }
        public DbSet<AssetRevaluation> AssetRevaluations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Asset
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.AssetType)
                .WithMany(at => at.Assets)
                .HasForeignKey(a => a.AssetTypeId);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Location)
                .WithMany(l => l.Assets)
                .HasForeignKey(a => a.LocationId);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Department)
                .WithMany(d => d.Assets)
                .HasForeignKey(a => a.DepartmentId);

            // AssetType
            modelBuilder.Entity<AssetType>()
                .HasOne(at => at.DepreciationMethod)
                .WithMany(dm => dm.AssetTypes)
                .HasForeignKey(at => at.DepreciationMethodId);

            // DepreciationEntry
            modelBuilder.Entity<DepreciationEntry>()
                .HasOne(de => de.Asset)
                .WithMany(a => a.DepreciationEntries)
                .HasForeignKey(de => de.AssetId);

            // MaintenanceLog
            modelBuilder.Entity<MaintenanceLog>()
                .HasOne(ml => ml.Asset)
                .WithMany(a => a.MaintenanceLogs)
                .HasForeignKey(ml => ml.AssetId);



            // AssetTransfer
            modelBuilder.Entity<AssetTransfer>()
                .HasOne(at => at.Asset)
                .WithMany(a => a.AssetTransfers)
                .HasForeignKey(at => at.AssetId);

            modelBuilder.Entity<AssetTransfer>()
                .HasOne(at => at.FromDepartment)
                .WithMany(d => d.FromTransfers)
                .HasForeignKey(at => at.FromDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetTransfer>()
                .HasOne(at => at.ToDepartment)
                .WithMany(d => d.ToTransfers)
                .HasForeignKey(at => at.ToDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // AssetDisposal
            modelBuilder.Entity<AssetDisposal>()
                .HasOne(ad => ad.Asset)
                .WithMany(a => a.AssetDisposals)
                .HasForeignKey(ad => ad.AssetId);

            // AssetRevaluation
            modelBuilder.Entity<AssetRevaluation>()
                .HasOne(ar => ar.Asset)
                .WithMany(a => a.AssetRevaluations)
                .HasForeignKey(ar => ar.AssetId);



 

            // Unique constraints
            modelBuilder.Entity<Asset>().HasIndex(a => a.SerialNumber).IsUnique();

        }
    }
}
