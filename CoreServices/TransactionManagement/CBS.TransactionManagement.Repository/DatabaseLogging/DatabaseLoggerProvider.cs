using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.DatabaseLogging
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly TransactionContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseLoggerProvider(TransactionContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
       
        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(categoryName, _context, _httpContextAccessor);
        }
        public void Dispose() { }
    }
}
