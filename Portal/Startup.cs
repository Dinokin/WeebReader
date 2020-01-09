using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WeebReader.Web.Portal
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }
        
        public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
            }

            application.UseRouting();

            application.UseEndpoints(endpoints => { endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); }); });
        }
    }
}