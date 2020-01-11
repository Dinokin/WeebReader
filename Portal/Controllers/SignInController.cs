using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Web.Portal.Models.SignIn;
using WeebReader.Web.Portal.Services;

namespace WeebReader.Web.Portal.Controllers
{
    public class SignInController : Controller
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly BaseContext _context;
        private readonly EmailSender _emailSender;

        public SignInController(SignInManager<IdentityUser<Guid>> signInManager, UserManager<IdentityUser<Guid>> userManager, BaseContext context, EmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
        }

        public IActionResult Index(string returnUrl)
        {
            TempData["ReturnUrl"] = !string.IsNullOrWhiteSpace(returnUrl) ? returnUrl : null;
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UserManager") : (IActionResult) View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInModel signInModel)
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
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [Authorize]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ForgotPassword()
        {
            if (!bool.Parse((await _context.Settings.SingleAsync(setting => setting.Key == "EmailEnabled")).Value) || _signInManager.IsSignedIn(User))
                return RedirectToAction("Index");
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UserManager") : (IActionResult) View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordSendMail(ForgotPasswordModel forgotPasswordModel)
        {
            if (!bool.Parse((await _context.Settings.SingleAsync(setting => setting.Key == "EmailEnabled")).Value))
            {
                ModelState.AddModelError("FunctionalityDisabled", "This functionality is disabled, please contact an administrator.");

                return new JsonResult(new
                {
                    success = false,
                    messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
                });
            }

            if (TryValidateModel(forgotPasswordModel))
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var siteName = await _context.Settings.SingleAsync(setting => setting.Key == "SiteName");
                    var siteAddress = await _context.Settings.SingleAsync(setting => setting.Key == "SiteAddress");
                    var siteEmail = await _context.Settings.SingleAsync(setting => setting.Key == "SiteEmail");
                
                    var stringBuilder = new StringBuilder();

                    stringBuilder.AppendLine($"Hello {user.UserName},");
                    stringBuilder.AppendLine($"A password reset was request at {siteName.Value}.");
                    stringBuilder.AppendLine("Please go to the following URL to proceed with the reset.No action is need if you didn't request this.");
                    stringBuilder.AppendLine($"{siteAddress.Value}{Url.Action("ResetPassword", new {id = user.Id, token})}");

                    await _emailSender.SendEmail(siteEmail.Value, user.Email, $"Password Reset - {siteName.Value}", stringBuilder.ToString());
                }

                TempData["SuccessMessage"] = "If that address exists in our database, a message will be send with the details on how to proceed.";
                
                return new JsonResult(new
                {
                    success = true,
                    destination = Url.Action("Index")
                });
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        public async Task<IActionResult> ResetPassword()
        {
            throw new NotImplementedException();
        }
    }
}