using CBS.Communication.Data;
using CBS.Communication.Data.Entity;
using CBS.Communication.Data.Entity.AuditLog;
using Microsoft.EntityFrameworkCore;

namespace CBS.Communication.Domain
{
    public class POSContext : DbContext
    {
        public POSContext(DbContextOptions<POSContext> options) : base(options)
        { }
        public DbSet<AuditLog> AuditLogs { get; set; }

     
    }
}
