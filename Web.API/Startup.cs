using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WeebReader.Web.API.Data.Contexts;
using WeebReader.Web.API.Data.Contexts.Abstract;
using WeebReader.Web.API.Extensions;
using WeebReader.Web.API.Filters;
using WeebReader.Web.API.Others;
using WeebReader.Web.API.Services;
using WeebReader.Web.API.Settings;
using WeebReader.Web.API.Utilities;

namespace WeebReader.Web.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly Configuration _bindConfiguration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _bindConfiguration = new();
            
            _configuration.Bind(_bindConfiguration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Configuration>(_configuration);
            
            services.AddDbContext<BaseContext, MariaDbContext>(builder =>
                builder.UseMySql(_bindConfiguration.Database.ConnectionString, new MariaDbServerVersion(new Version(10, 3))));

            services.AddIdentityCore<IdentityUser<Guid>>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddRoles<IdentityRole<Guid>>().AddEntityFrameworkStores<BaseContext>().AddDefaultTokenProviders().AddApiSignInManager().AddSignInManager();
            
            services.AddDataProtection().PersistKeysToFileSystem(Security.KeysDirectory).ProtectKeysWithCertificate(Security.Certificate);

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Security.Issuer,
                    ValidAudience = Security.Audience,
                    IssuerSigningKey = new X509SecurityKey(Security.Certificate),
                    AuthenticationType = TokenValidationParameters.DefaultAuthenticationType
                };
            });

            if (_bindConfiguration.Proxy.Enabled)
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    foreach (var ip in _bindConfiguration.Proxy.TrustedIps)
                        options.KnownProxies.Add(IPAddress.Parse(ip));
                });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddCors(options => options.AddDefaultPolicy(builder => builder.WithOrigins(_bindConfiguration.AllowedOrigins).AllowAnyMethod().AllowAnyHeader()));

            services.AddControllers(options =>
            {
                options.Filters.Add<ModelValidatorFilter>();
                options.Filters.Add<ExceptionHandlerFilter>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(new SnakeCaseNamingPolicy(), false));
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddHttpClient();
            services.AddSingleton<EmailDispatcher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            RunMigrations(app);

            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new(CultureInfo.InvariantCulture);
                options.SupportedCultures = new List<CultureInfo>(new[] {CultureInfo.InvariantCulture});
                options.SupportedUICultures = new List<CultureInfo>(new[] {CultureInfo.InvariantCulture});
            });

            if (_bindConfiguration.Proxy.Enabled)
                app.UseForwardedHeaders(new()
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            
            if (_bindConfiguration.UseHttps)
                app.UseHttpsRedirection();

            if (_bindConfiguration.ServeStaticFiles)
                app.UseStaticFiles();
            
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(builder => builder.MapControllers());
        }
        
        private static void RunMigrations(IApplicationBuilder application)
        {
            using var scope = application.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<BaseContext>();
            
            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();
        }

    }
}