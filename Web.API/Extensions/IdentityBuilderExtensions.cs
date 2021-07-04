using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WeebReader.Web.API.Services;

namespace WeebReader.Web.API.Extensions
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddApiSignInManager(this IdentityBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            
            var managerType = typeof(ApiSignInManager<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(managerType);
            
            return builder;
        }
    }
}