using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeebReader.Common.Utilities;
using WeebReader.Data.Contexts;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Services;

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
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddDefaultTokenProviders().AddEntityFrameworkStores<BaseContext>();

            services.AddRouting(options =>
            {
                options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });
            
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/admin/";
                options.LogoutPath = "/admin/signout";
                options.AccessDeniedPath = "/denied";
                options.ReturnUrlParameter = "returnUrl";
            });
            
            services.AddDataProtection().PersistKeysToFileSystem(Directory.CreateDirectory($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}Keys"))
                .ProtectKeysWithCertificate(Security.GetCertificate());

            services.AddControllersWithViews()
                .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
                });

            services.AddHttpClient();
            services.AddTransient<ReCaptchaValidator>();
            services.AddTransient<EmailSender>();
            services.AddTransient<ParametersManager>();
            services.AddTransient<PostsManager>();
            services.AddTransient<TitlesManager<Title>>();
            services.AddTransient<ChapterManager<Chapter>>();
            services.AddTransient<PagesManager<Page>>();
            services.AddTransient<TitleArchiver<Title>>();
            services.AddTransient<ChapterArchiver<Chapter>>();
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

            application.UseEndpoints(builder => builder.MapControllers());
            
            using var scope = application.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<BaseContext>();
            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();
        }
    }
}