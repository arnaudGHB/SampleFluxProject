using NLog.Web;
using CBS.CustomLog.Logger;
using CBS.TransactionManagement.API;
using Serilog;
using Serilog.Events;
using CBS.CustomLog.Logger.CustomLogger;
using NLog.Extensions.Logging;
using CBS.TransactionManagement.Domain;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>()

             //.ConfigureLogging((hostingContext, logging) =>
             //{
             //    logging.AddNLog(hostingContext.Configuration.GetSection("Logging"));
             //})
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
        logging.ClearProviders(); // Clear default logging providers
        //logging.AddCodeFileLogger(options =>
        //{
        //    hostBuilderContext.Configuration.GetSection("BiosLogger").GetSection("Options").Bind(options);
        //});
        
        logging.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "\n[dd-MMM-yyyy hh:mm:ss]\n";
        });
    });
}



//var builder = WebApplication.CreateBuilder(args);

//var startup = new Startup(builder.Configuration);

//startup.ConfigureServices(builder.Services);

//builder.Logging.ClearProviders();
//builder.Host.UseNLog();

//var app = builder.Build();

//try
//{
//    using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
//    {
//        var context = serviceScope.ServiceProvider.GetRequiredService<POSContext>();
//        context.Database.Migrate();
//    }
//}
//catch (System.Exception)
//{
//    throw;
//}

//ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
//startup.Configure(app, loggerFactory, app.Environment);

//app.Run();
