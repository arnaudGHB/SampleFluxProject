using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CBS.CustomLog.Logger.CustomLogger
{
    public static class FileLoggerExtensions
    {
        public static ILoggingBuilder AddCodeFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}