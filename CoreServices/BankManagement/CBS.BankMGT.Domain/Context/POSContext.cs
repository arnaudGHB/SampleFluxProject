

using CBS.BankMGT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Domain
{
    public class POSContext : DbContext
    {
        private readonly IConfiguration _configuration;

        // Constructor with DbContextOptions and IConfiguration injection
        public POSContext(DbContextOptions<POSContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        // Configures the DbContextOptionsBuilder if not already configured
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Retrieves the connection string from IConfiguration
                string connectionString = _configuration.GetConnectionString("POSDbConnectionString");
                // Configures DbContext to use SQL Server with the retrieved connection string
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        // Configures the model using modelBuilder (optional)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Custom model configurations can be added here
        }

        // DbSet properties representing database tables
        public DbSet<Country> Countries { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Subdivision> Subdivisions { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<ThirdPartyBranche> ThirdPartyBranches { get; set; }
        public DbSet<ThirdPartyInstitution> ThirdPartyInstitutions { get; set; }

          public DbSet<BankingZone> BankingZones { get; set; }
        public DbSet<BankZoneBranch> BankZoneBranches { get; set; }
        public DbSet<Branch> Branches { get; set; }
        
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<EconomicActivity> EconomicActivities { get; set; }
        public DbSet<FundingLine> FundingLines { get; set; }
        public DbSet<InstallmentPeriodicity> InstallmentPeriodicities { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<DocumentUploaded> DocumentUploadeds { get; set; }
 

    }
}
