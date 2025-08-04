
using NLog.Web;
using CBS.NLoan.API;
using Serilog;
using Serilog.Events;
using CBS.CustomLog.Logger.CustomLogger;

public class Program
{
    public static void Main(string[] args)
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
    //This is to update the database
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
        })
        .ConfigureLogging((hostBuilderContext, logging) =>
        {
            logging.AddCodeFileLogger(options =>
            {
                hostBuilderContext.Configuration.GetSection("BiosLogger").GetSection("Options").Bind(options);
            });
        });
}


//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
