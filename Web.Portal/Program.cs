using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WeebReader.Common.Utilities;

namespace WeebReader.Web.Portal
{
    public static class Program
    {
        public static async Task Main(string[] args) => await Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder =>
        {
            builder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                configurationBuilder.AddEnvironmentVariables();
                configurationBuilder.AddJsonFile($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}appsettings.json");

                if (!context.HostingEnvironment.IsProduction())
                    configurationBuilder.AddJsonFile($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}appsettings.{context.HostingEnvironment.EnvironmentName}.json");
            });
            
            builder.UseContentRoot(Location.CurrentDirectory.FullName);
            builder.UseWebRoot($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}Static");
            builder.UseStartup<Startup>();
        }).Build().RunAsync();
    }
}