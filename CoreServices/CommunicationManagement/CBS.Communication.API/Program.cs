using CBS.Communication.API;
using NLog.Web;
using Serilog;
using Serilog.Events;
using System.Reflection.Metadata;

using CBS.CustomLog.Logger.CustomLogger;

public class Program
{
    static void Main(string[] args)
    {
        var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

        try
        {
            logger.Info("System Init");
            CreateHostBuilder(args).Build().Run();
            logger.Info("System Init Completed.");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Application stopped because of an exception");
            throw;
        }
        finally
        {
            NLog.LogManager.Shutdown();
        }
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
   Host.CreateDefaultBuilder(args)
       .ConfigureWebHostDefaults(webBuilder =>
       {
           webBuilder.UseStartup<Startup>()

            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            });
           webBuilder.ConfigureServices((hostContext, services) =>
           {
               var startup = new Startup(hostContext.Configuration);
               services.AddControllers();
           });
       }).UseSerilog((context, config) =>
       {
           config
               .MinimumLevel.Information()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .Enrich.FromLogContext()
               .WriteTo.File($"{context.Configuration["LoogerSetting:LoggerPath"]}\\Log_.log", rollingInterval: RollingInterval.Day);
       })
       .ConfigureLogging((hostBuilderContext, logging) =>
       {
           logging.AddCodeFileLogger(options =>
           {
               hostBuilderContext.Configuration.GetSection("BiosLogger").GetSection("Options").Bind(options);
           });
       });
}
