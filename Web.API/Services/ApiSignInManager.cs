using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WeebReader.Web.API.Services
{
    public class ApiSignInManager<TUser> where TUser : class
    {
        public UserManager<TUser> UserManager { get; }
        
        private readonly IUserClaimsPrincipalFactory<TUser> _claimsFactory;
        private readonly IdentityOptions _options;
        private readonly IUserConfirmation<TUser> _confirmation;
        private readonly HttpContext? _httpContext;

        public ApiSignInManager(UserManager<TUser> userManager, IUserClaimsPrincipalFactory<TUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, IUserConfirmation<TUser> confirmation, IHttpContextAccessor httpContextAccessor)
        {
            UserManager = userManager;
            
            _claimsFactory = claimsFactory;
            _options = optionsAccessor.Value;
            _confirmation = confirmation;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<SignInResult> PasswordSignIn(string username, string password, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(username);
            
            if (user == null)
                return SignInResult.Failed;

            if (await PreSignInCheck(user) is { } error)
                return error;

            if (await UserManager.CheckPasswordAsync(user, password))
            {
                await ResetLockout(user);

                if (_httpContext != null) 
                    _httpContext.User = await _claimsFactory.CreateAsync(user);

                return SignInResult.Success;
            }

            if (UserManager.SupportsUserLockout && lockoutOnFailure)
            {
                await UserManager.AccessFailedAsync(user);
                
                if (await UserManager.IsLockedOutAsync(user))
                    return SignInResult.LockedOut;
            }
            
            return SignInResult.Failed;
        }
        
        private async Task<bool> CanSignIn(TUser user)
        {
            if (_options.SignIn.RequireConfirmedEmail && !await UserManager.IsEmailConfirmedAsync(user))
                return false;

            if (_options.SignIn.RequireConfirmedPhoneNumber && !await UserManager.IsPhoneNumberConfirmedAsync(user))
                return false;
            
            if (_options.SignIn.RequireConfirmedAccount && !await _confirmation.IsConfirmedAsync(UserManager, user))
                return false;
            
            return true;
        } 
        
        private async Task<bool> IsLockedOut(TUser user) => UserManager.SupportsUserLockout && await UserManager.IsLockedOutAsync(user);
        
        private async Task<SignInResult?> PreSignInCheck(TUser user)
        {
            if (!await CanSignIn(user))
                return SignInResult.NotAllowed;
            
            if (await IsLockedOut(user))
                return SignInResult.LockedOut;

            return null;
        }
        
        private Task ResetLockout(TUser user) => UserManager.SupportsUserLockout ? UserManager.ResetAccessFailedCountAsync(user) : Task.CompletedTask;
    }
}