using CBS.TransactionManagement.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.DatabaseLogging
{
    public static class DatabaseLoggerExtensions
    {
        public static ILoggingBuilder AddDbLogger(this ILoggingBuilder builder)
        {
            builder.AddProvider(new DatabaseLoggerProvider(builder.Services.BuildServiceProvider().GetRequiredService<TransactionContext>(),builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()));
            builder.Services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>();

            return builder;
        }

      
    }
}
