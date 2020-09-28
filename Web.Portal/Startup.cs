using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using WeebReader.Common.Utilities;
using WeebReader.Data.Contexts;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Extensions;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Portal.Others.RateLimit;
using WeebReader.Web.Services;
using Utilities = WeebReader.Web.Models.Others.Utilities;

namespace WeebReader.Web.Portal
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BaseContext, MariaDbContext>(builder => builder.UseMySql(_configuration.GetConnectionString("MariaDb"), optionsBuilder => optionsBuilder.ServerVersion(ServerVersion.Default.Version, ServerType.MariaDb)));

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

            if (_configuration.GetValue<bool>("Application:Nginx:Enabled"))
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    foreach (var ip in _configuration.GetValue<IEnumerable<string>>("Application:Nginx:TrustedIps"))
                        options.KnownProxies.Add(IPAddress.Parse(ip));
                });

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
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                });

            services.AddHttpClient();
            services.AddTransient<ReCaptchaValidator>();
            services.AddTransient<EmailSender>();
            services.AddTransient<ParametersManager>();
            services.AddTransient<PostsManager>();
            services.AddTransient<TitlesManager<Title>>();
            services.AddTransient<ChapterManager<Chapter>>();
            services.AddTransient<PagesManager<Page>>();
            services.AddTransient<NovelChapterContentManager>();
            services.AddTransient<TitleArchiver<Title>>();
            services.AddTransient<ChapterArchiver<Chapter>>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, AlternativeRateLimitConfiguration>();
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

            if (_configuration.GetValue<bool>("Application:Nginx:Enabled"))
            {
                application.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }
            
            RunMigrations(application);
            SetUpRateLimiting(application);
            
            application.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture);
                options.SupportedCultures = new List<CultureInfo>(new[] {CultureInfo.InvariantCulture});
                options.SupportedUICultures = new List<CultureInfo>(new[] {CultureInfo.InvariantCulture});
            });

            if (_configuration.GetValue<bool>("Application:UseHttps"))
                application.UseHttpsRedirection();

            application.UseIpRateLimiting();
            application.UseStaticFiles();
            application.UseRouting();
            application.UseAuthentication();
            application.UseAuthorization();

            application.UseEndpoints(builder => builder.MapControllers());
        }

        private static void RunMigrations(IApplicationBuilder application)
        {
            using var scope = application.ApplicationServices.CreateScope();
            
            using var context = scope.ServiceProvider.GetRequiredService<BaseContext>();
            
            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();
        }

        private static async void SetUpRateLimiting(IApplicationBuilder application)
        {
            using var scope = application.ApplicationServices.CreateScope();
            
            var parametersManager = scope.ServiceProvider.GetRequiredService<ParametersManager>();
            var rateLimitOptions = scope.ServiceProvider.GetRequiredService<IOptions<IpRateLimitOptions>>().Value;
            var policyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

            rateLimitOptions.EnableEndpointRateLimiting = true;
            rateLimitOptions.RealIpHeader = await parametersManager.GetValue<string?>(ParameterTypes.RateLimitRealIpHeader) ?? rateLimitOptions.RealIpHeader;
            rateLimitOptions.QuotaExceededMessage = OtherMessages.RequestLimitExceeded;
            rateLimitOptions.GeneralRules = new List<RateLimitRule>();

            if (await parametersManager.GetValue<bool>(ParameterTypes.ContactDiscordEnabled))
                rateLimitOptions.GeneralRules.AddRange(new [] {
                    new RateLimitRule 
                    {
                        Endpoint = Constants.RateLimitEndpointContent, 
                        Limit = await parametersManager.GetValue<double?>(ParameterTypes.RateLimitMaxContentRequests) ?? Constants.RateLimitDefaultRequestLimit, 
                        Period = $"1{Utilities.GetRateLimitTimePeriod(await parametersManager.GetValue<byte?>(ParameterTypes.RateLimitPeriodContent)) ?? Constants.RateLimitDefaultTimeInterval}"
                    },
                    new RateLimitRule
                    {
                        Endpoint = Constants.RateLimitEndpointContentJson,
                        Limit = await parametersManager.GetValue<double?>(ParameterTypes.RateLimitMaxContentRequests) ?? Constants.RateLimitDefaultRequestLimit, 
                        Period = $"1{Utilities.GetRateLimitTimePeriod(await parametersManager.GetValue<byte?>(ParameterTypes.RateLimitPeriodContent)) ?? Constants.RateLimitDefaultTimeInterval}"
                    }
                });
            
            if (await parametersManager.GetValue<bool>(ParameterTypes.RateLimitApiEnabled))
                rateLimitOptions.GeneralRules.Add(new RateLimitRule
                {
                    Endpoint = "*:/api/*",
                    Limit = await parametersManager.GetValue<double?>(ParameterTypes.RateLimitMaxApiRequests) ?? Constants.RateLimitDefaultRequestLimit,
                    Period = $"1{Utilities.GetRateLimitTimePeriod(await parametersManager.GetValue<byte?>(ParameterTypes.RateLimitPeriodApi)) ?? Constants.RateLimitDefaultTimeInterval}"
                });

            await policyStore.SeedAsync();
        }
    }
}