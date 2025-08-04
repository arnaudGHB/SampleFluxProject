using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;

public class MockWebHostEnvironment : IWebHostEnvironment
{
    public string EnvironmentName { get; set; } = "Development";
    public string ApplicationName { get; set; } = "MyApp";
    public string WebRootPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    public IFileProvider WebRootFileProvider { get; set; }
    public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
    public IFileProvider ContentRootFileProvider { get; set; }

    public MockWebHostEnvironment()
    {
        WebRootFileProvider = new PhysicalFileProvider(WebRootPath);
        ContentRootFileProvider = new PhysicalFileProvider(ContentRootPath);
    }
}

