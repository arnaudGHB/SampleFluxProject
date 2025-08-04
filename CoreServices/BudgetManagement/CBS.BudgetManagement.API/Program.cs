using CBS.BudgetManagement.API;
using CBS.BudgetManagement.Data;
using CBS.CustomLog.Logger.CustomLogger;
using Newtonsoft.Json;
using NLog.Web;
using Serilog;
using Serilog.Events;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

        var host = CreateHostBuilder(args).Build();
        // Initialize the database with seed data
        InitializeDatabase(host);
        host.Run();
        try
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("EntityFrameworkCore", LogLevel.Information)
                    .AddConsole();
            });
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

    public static IHostBuilder CreateHostBuilder(string[] args) =>
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
        }).
    ConfigureLogging((hostBuilderContext, logging) =>
        {
        logging.AddCodeFileLogger(options =>
        {
            hostBuilderContext.Configuration.GetSection("BiosLogger").GetSection("Options").Bind(options);
        });
    });

    private static void InitializeDatabase(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                //var context = services.GetRequiredService<POSContext>();

                //// Apply any pending migrations
                ////context.Database.Migrate();
                ////var listOfOperationEvent = ReadDataFromOperationEventFile("OperationEvent.json");
                ////var listOfOperationEventAttributes = ReadDataFromOperationEventAttributesFiles("OperationEventAttributes.json");
                ////                 //// Seed the database with provided data
                ////DbInitializer.Initialize(context, listOfOperationEvent, listOfOperationEventAttributes);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }


}