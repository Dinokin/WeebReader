using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Directory = WeebReader.Utilities.Common.Directory;

namespace WeebReader.Web.Portal
{
    public static class Program
    {
        public static async Task Main(string[] args) => await Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder =>
        {
            builder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                configurationBuilder.AddEnvironmentVariables();
                configurationBuilder.AddJsonFile($"{Directory.CurrentDirectory}{Path.DirectorySeparatorChar}appsettings.json");

                if (!context.HostingEnvironment.IsProduction())
                    configurationBuilder.AddJsonFile($"{Directory.CurrentDirectory}{Path.DirectorySeparatorChar}appsettings.{context.HostingEnvironment.EnvironmentName}.json");
            });
            
            builder.UseContentRoot(Directory.CurrentDirectory.FullName);
            builder.UseWebRoot($"{Directory.CurrentDirectory}{Path.DirectorySeparatorChar}Static");
            builder.UseStartup<Startup>();
        }).Build().RunAsync();
    }
}