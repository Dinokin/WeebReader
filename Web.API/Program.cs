using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WeebReader.Web.API.Others.Settings;
using WeebReader.Web.API.Others.Utilities;

namespace WeebReader.Web.API
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder =>
            {
                builder.ConfigureKestrel((context, options) =>
                {
                    var configuration = new Configuration();
                    context.Configuration.Bind(configuration);
                    
                    options.Listen(IPAddress.Parse(configuration.IpAddress), configuration.HttpPort);

                    if (configuration.UseHttps)
                        options.Listen(IPAddress.Parse(configuration.IpAddress), configuration.HttpsPort, listenOptions => listenOptions.UseHttps(Security.Certificate));
                });

                builder.UseContentRoot(Location.CurrentDirectory.ToString());
                builder.UseWebRoot(Location.StaticDirectory.ToString());
                builder.UseStartup<Startup>();
            }).Build().RunAsync();
        }
    }
}