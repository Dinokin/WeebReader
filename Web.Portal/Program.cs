using System.IO;
using System.Net;
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

            builder.ConfigureKestrel((context, options) =>
            {
                options.Listen(IPAddress.Parse(context.Configuration.GetValue<string>("Application:IpAddress")), context.Configuration.GetValue<int>("Application:HttpPort"));

                if (context.Configuration.GetValue<bool>("Application:UseHttps"))
                {
                    options.Listen(IPAddress.Parse(context.Configuration.GetValue<string>("Application:IpAddress")), context.Configuration.GetValue<int>("Application:HttpsPort"), listenOptions =>
                    {
                        listenOptions.UseHttps(Security.GetCertificate());
                    });
                }
            });

            builder.UseContentRoot($"{Location.CurrentDirectory}");
            builder.UseWebRoot($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}Static");
            builder.UseStartup<Startup>();
        }).Build().RunAsync();
    }
}