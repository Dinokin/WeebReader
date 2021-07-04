using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WeebReader.Web.API.Services
{
    public class ApiSignInManager<TUser> where TUser : class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<TUser> _claimsFactory;
        private readonly IdentityOptions _options;
        private readonly IUserConfirmation<TUser> _confirmation;
        private readonly HttpContext _httpContext;

        public ApiSignInManager(UserManager<TUser> userManager, IUserClaimsPrincipalFactory<TUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, IUserConfirmation<TUser> confirmation, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _options = optionsAccessor.Value;
            _confirmation = confirmation;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<SignInResult> PasswordSignIn(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            
            if (user == null)
                return SignInResult.Failed;

            if (await PreSignInCheck(user) is { } error)
                return error;

            if (await _userManager.CheckPasswordAsync(user, password))
            {
                await ResetLockout(user);

                _httpContext.User = await _claimsFactory.CreateAsync(user);

                return SignInResult.Success;
            }

            if (_userManager.SupportsUserLockout)
            {
                await _userManager.AccessFailedAsync(user);
                
                if (await _userManager.IsLockedOutAsync(user))
                    return SignInResult.LockedOut;
            }
            
            return SignInResult.Failed;
        }

        public async Task<SignInResult> RefreshSignInAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
                return SignInResult.NotAllowed;

            if (await PreSignInCheck(user) is { } error)
                return error;
            
            _httpContext.User = await _claimsFactory.CreateAsync(user);

            return SignInResult.Success;
        }
        
        private async Task<bool> CanSignIn(TUser user)
        {
            if (_options.SignIn.RequireConfirmedEmail && !await _userManager.IsEmailConfirmedAsync(user))
                return false;

            if (_options.SignIn.RequireConfirmedPhoneNumber && !await _userManager.IsPhoneNumberConfirmedAsync(user))
                return false;
            
            if (_options.SignIn.RequireConfirmedAccount && !await _confirmation.IsConfirmedAsync(_userManager, user))
                return false;
            
            return true;
        } 
        
        private async Task<bool> IsLockedOut(TUser user) => _userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user);
        
        private async Task<SignInResult?> PreSignInCheck(TUser user)
        {
            if (!await CanSignIn(user))
                return SignInResult.NotAllowed;
            
            if (await IsLockedOut(user))
                return SignInResult.LockedOut;

            return null;
        }
        
        private Task ResetLockout(TUser user) => _userManager.SupportsUserLockout ? _userManager.ResetAccessFailedCountAsync(user) : Task.CompletedTask;
    }
}