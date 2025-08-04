 

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Tracing;
 
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;
using CBS.DailyCollectionManagement.Data.Entity;

namespace CBS.DailyCollectionManagement.Domain
{
    public class DailyCollectionContext : DbContext
    {
        private string connectionString { get; set; }
        public DailyCollectionContext(DbContextOptions<DailyCollectionContext> options, IConfiguration configuration) : base(options)
        {

            //connectionString = configuration.GetRequiredSection("ConnectionStrings")?["CBSDailyCollectionManagementDB"] ?? "default_connection_string";
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region sequences
  
            #endregion


       

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Other configuration code...AuditLogs

            optionsBuilder.EnableSensitiveDataLogging();

        }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<CommissionSetting> CommissionSettings { get; set; }
    }
}