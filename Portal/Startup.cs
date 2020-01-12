using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeebReader.Data.Contexts;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Web.Portal.Services;

namespace WeebReader.Web.Portal
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BaseContext, MariaDbContext>(builder => builder.UseMySql(_configuration.GetConnectionString("MariaDb")));

            services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
                {
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(3);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddDefaultTokenProviders().AddEntityFrameworkStores<BaseContext>();

            services.AddRouting(options => options.LowercaseUrls = true);
            
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/admin/";
                options.LogoutPath = "/admin/signout";
                options.AccessDeniedPath = "/denied"; 
            });
            
            services.AddDataProtection().PersistKeysToFileSystem(Directory.CreateDirectory($"{Utilities.CurrentDirectory}{Path.DirectorySeparatorChar}Keys"))
                .ProtectKeysWithCertificate(Utilities.GetCertificate());

            services.AddControllersWithViews()
                .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false);

            services.AddTransient<EmailSender>();
        }
        
        public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
                application.UseDatabaseErrorPage();
            }
            else
                application.UseExceptionHandler("/error");
            
            application.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture);
                options.SupportedCultures = new List<CultureInfo>(new[] {CultureInfo.InvariantCulture});
                options.SupportedUICultures = new List<CultureInfo>(new[] {CultureInfo.InvariantCulture});
            });
            
            application.UseStaticFiles();
            application.UseRouting();
            application.UseAuthentication();
            application.UseAuthorization();

            application.UseEndpoints(builder => builder.MapDefaultControllerRoute());

            using var scope = application.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<BaseContext>();
            
            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();
        }
    }
}