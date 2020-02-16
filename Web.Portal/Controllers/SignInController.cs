using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WeebReader.Data.Entities;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Models.SignIn;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    [Route("Admin/{action:slugify=Index}")]
    public class SignInController : Controller
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly SettingManager _settingManager;
        private readonly EmailSender _emailSender;

        public SignInController(SignInManager<IdentityUser<Guid>> signInManager, UserManager<IdentityUser<Guid>> userManager, SettingManager settingManager, EmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _settingManager = settingManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl)
        {
            TempData["ReturnUrl"] = !string.IsNullOrWhiteSpace(returnUrl) ? returnUrl : null;
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UserManager") : (IActionResult) View();
        }
        
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInModel signInModel)
        {
            if (_signInManager.IsSignedIn(User))
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {OtherMessages.AlreadySignedIn}
                });
            
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
                    ModelState.AddModelError("LockedOut", OtherMessages.TooManyFailedSignIn);
                else
                    ModelState.AddModelError("InvalidCredentials", ValidationMessages.InvalidCredentials);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            if (await _settingManager.GetValue<bool>(Setting.Keys.EmailEnabled) || _signInManager.IsSignedIn(User))
                return RedirectToAction("Index");
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UserManager") : (IActionResult) View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(EmailModel forgotPasswordModel)
        {
            if (await _settingManager.GetValue<bool>(Setting.Keys.EmailEnabled))
            {
                ModelState.AddModelError("FunctionalityDisabled", OtherMessages.DisableFunctionality);

                return new JsonResult(new
                {
                    success = false,
                    messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
                });
            }
            
            if (_signInManager.IsSignedIn(User))
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {OtherMessages.AlreadySignedInChangePassword}
                });

            if (TryValidateModel(forgotPasswordModel))
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var siteName = await _settingManager.GetValue(Setting.Keys.SiteName);
                    var siteAddress = await _settingManager.GetValue(Setting.Keys.SiteAddress);
                    var siteEmail = await _settingManager.GetValue(Setting.Keys.SiteEmail);

                    var message = string.Format(Emails.PasswordResetEmailBody, user.UserName, siteName, $"{siteAddress}{Url.Action("ResetPassword", new {userId = user.Id, token})}");

                    await _emailSender.SendEmail(siteEmail, user.Email, string.Format(Emails.PasswordResetEmailSubject, siteName), message);
                }

                TempData["SuccessMessage"] = new[] {OtherMessages.EmailAlreadyInDatabaseWarning};
                
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
        
        [HttpGet]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("YourProfile", "UserManager");
            
            TryValidateModel(resetPasswordModel);

            if (ModelState["Token"].ValidationState == ModelValidationState.Valid && await _userManager.FindByIdAsync(resetPasswordModel.UserId.ToString()) != null)
                return View(resetPasswordModel);

            TempData["ErrorMessage"] = new[] {OtherMessages.PasswordResetInvalidData};

            return RedirectToAction("Index");
        } 
        
        [HttpPatch]
        public async Task<IActionResult> ChangePassword(ResetPasswordModel resetPasswordModel)
        {
            if (_signInManager.IsSignedIn(User))
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {OtherMessages.AlreadySignedInChangePassword}
                });

            if (TryValidateModel(resetPasswordModel) && await _userManager.FindByIdAsync(resetPasswordModel.UserId.ToString()) is var user && user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.NewPassword);

                if (result.Succeeded)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    
                    TempData["SuccessMessage"] = new[] {OtherMessages.PasswordChangedSuccessfully};
                    
                    return new JsonResult(new
                    {
                        success = true,
                        destination = Url.Action("Index")
                    });
                }
                
                ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotChangePassword);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpGet]
        public async Task<IActionResult> ChangeEmail(ChangeEmailModel changeEmailModel)
        {
            if (TryValidateModel(changeEmailModel))
            {
                if (await _userManager.FindByEmailAsync(changeEmailModel.Email) is var candidate && candidate != null)
                    TempData["ErrorMessage"] = new[] {ValidationMessages.EmailAlreadyInUse};
                else
                {
                    var user = await _userManager.FindByIdAsync(changeEmailModel.UserId.ToString());

                    if (user != null)
                    {
                        var result = await _userManager.ChangeEmailAsync(user, changeEmailModel.Email, changeEmailModel.Token);

                        if (result.Succeeded)
                            TempData["SuccessMessage"] = new[] {OtherMessages.EmailChangedSuccessfully};
                    }
                    else
                        ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotChangeEmail);
                }
            }
            else
                TempData["ErrorMessage"] = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage);
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UserManager") : RedirectToAction("Index");
        }
    }
}