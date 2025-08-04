using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CBS.CustomLog.Logger.CustomLogger
{
    [ProviderAlias("BiosLogger")]
    public class FileLoggerProvider : ILoggerProvider
    {
        public readonly FileLoggerOptions Options;
        public FileLoggerProvider(IOptions<FileLoggerOptions> _options)
        {
            Options = _options.Value;
            if (Options.FilePath != null && Options.FilePath != null)
            {
                if (!Directory.Exists(Options.FolderPath))
                {
                    Directory.CreateDirectory(Options.FolderPath);
                }
            }

        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(this);
        }

        public void Dispose()
        {
        }
    }
}