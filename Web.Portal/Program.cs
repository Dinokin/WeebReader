using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WeebReader.Web.Portal.Others;

namespace WeebReader.Web.Portal
{
    public static class Program
    {
        public static async Task Main(string[] args) => await Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder =>
        {
            builder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                configurationBuilder.AddEnvironmentVariables();
                configurationBuilder.AddJsonFile($"{Utilities.CurrentDirectory}{Path.DirectorySeparatorChar}appsettings.json");

                if (!context.HostingEnvironment.IsProduction())
                    configurationBuilder.AddJsonFile($"{Utilities.CurrentDirectory}{Path.DirectorySeparatorChar}appsettings.{context.HostingEnvironment.EnvironmentName}.json");
            });
            
            builder.ConfigureKestrel((context, options) => options.Listen(IPAddress.Parse(context.Configuration.GetValue("Application:IP", IPAddress.Any.ToString())), context.Configuration.GetValue("Application:Port", 5000)));
            
            builder.UseContentRoot(Utilities.CurrentDirectory.FullName);
            builder.UseWebRoot($"{Utilities.CurrentDirectory}{Path.DirectorySeparatorChar}Content");
            builder.UseStartup<Startup>();
        }).Build().RunAsync();
    }
}