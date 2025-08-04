using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.DatabaseLogging
{
    public class DatabaseLoggerOptions
    {
        public string ConnectionString { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}
