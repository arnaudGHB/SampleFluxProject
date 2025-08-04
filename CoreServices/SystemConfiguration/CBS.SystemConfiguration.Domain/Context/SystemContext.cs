using CBS.SystemConfiguration.Data;

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Tracing;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IO;
using System;


namespace CBS.SystemConfiguration.Domain
{
    public class SystemContext : DbContext
    {
        private string connectionString { get; set; }
        public SystemContext(DbContextOptions<SystemContext> options, IConfiguration configuration) : base(options)
        {

            //connectionString = configuration.GetRequiredSection("ConnectionStrings")?["CBSAccountManagementDB"] ?? "default_connection_string";
        }

     
            public DbSet<Town> Towns { get; set; }
            public DbSet<Subdivision> Subdivisions { get; set; }
            public DbSet<Division> Divisions { get; set; }
            public DbSet<Region> Regions { get; set; }
            public DbSet<Country> Countries { get; set; }
            public DbSet<AuditLog> AuditLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                //base.OnModelCreating(modelBuilder);

                // Configure Town -> Subdivision
                modelBuilder.Entity<Town>()
                    .HasOne(t => t.Subdivision)
                    .WithMany(s => s.Towns)
                    .HasForeignKey(t => t.SubdivisionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure Town -> Division
                modelBuilder.Entity<Town>()
                    .HasOne(t => t.Division)
                    .WithMany(d => d.Towns)
                    .HasForeignKey(t => t.DivisionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure Town -> Region
                modelBuilder.Entity<Town>()
                    .HasOne(t => t.Region)
                    .WithMany(r => r.Towns)
                    .HasForeignKey(t => t.RegionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure Subdivision -> Division
                modelBuilder.Entity<Subdivision>()
                    .HasOne(s => s.Division)
                    .WithMany(d => d.Subdivisions)
                    .HasForeignKey(s => s.DivisionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure Division -> Region
                modelBuilder.Entity<Division>()
                    .HasOne(d => d.Region)
                    .WithMany(r => r.Divisions)
                    .HasForeignKey(d => d.RegionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure Region -> Country
                modelBuilder.Entity<Region>()
                    .HasOne(r => r.Country)
                    .WithMany(c => c.Regions)
                    .HasForeignKey(r => r.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Other configuration code...

            optionsBuilder.EnableSensitiveDataLogging();

        }
    
    }
}