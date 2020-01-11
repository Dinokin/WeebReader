using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Web.Portal.Models.SignIn;

namespace WeebReader.Web.Portal.Controllers
{
    public class SignInController : Controller
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        public SignInController(SignInManager<IdentityUser<Guid>> signInManager) => _signInManager = signInManager;

        public IActionResult Index(string returnUrl)
        {
            TempData["ReturnUrl"] = !string.IsNullOrWhiteSpace(returnUrl) ? returnUrl : null;
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UserManager") : (IActionResult) View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInFormModel signInModel)
        {
            if (TryValidateModel(signInModel))
            {
                var result = await _signInManager.PasswordSignInAsync(signInModel.Username, signInModel.Password, false, true);

                if (result.Succeeded)
                    return new JsonResult(new
                    {
                        success = true,
                        destination = string.IsNullOrWhiteSpace(TempData["ReturnUrl"]?.ToString()) ? Url.Action("YourProfile", "UserManager") : TempData["ReturnUrl"]
                    });
                
                if (result.IsLockedOut)
                    ModelState.AddModelError("LockedOut", "This account cannot sign in due to too many failed sign in attempts.");
                else
                    ModelState.AddModelError("InvalidCredentials", "Invalid credentials, please try again.");
            }
            
            return new JsonResult(new
            {
                success = false,
                problems = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [Authorize]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ResetPassword()
        {
            throw new NotImplementedException();
        }
    }
}