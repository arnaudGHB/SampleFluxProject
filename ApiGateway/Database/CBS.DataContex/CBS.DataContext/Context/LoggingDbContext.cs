using Microsoft.EntityFrameworkCore;

namespace CBS.Gateway.DataContext
{
    public class LoggingDbContext : DbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options)
        {
        }
      
        public virtual DbSet<RequestResponseLog> RequestResponseLog { get; set; }
    }
}
