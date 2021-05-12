using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Controllers.SignIn;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Extensions;
using WeebReader.Web.Portal.Others.Extensions;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    public class SignInController : Controller
    {
        private readonly EmailSender _emailSender;
        private readonly ParametersManager _parameterManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;

        public SignInController(EmailSender emailSender, ParametersManager parameterManager, UserManager<IdentityUser<Guid>> userManager, SignInManager<IdentityUser<Guid>> signInManager)
        {
            _emailSender = emailSender;
            _parameterManager = parameterManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("Admin/{returnUrl?}")]
        public IActionResult SignIn(string returnUrl)
        {
            TempData["ReturnUrl"] = !string.IsNullOrWhiteSpace(returnUrl) ? returnUrl : null;
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UsersManager") : View();
        }
        
        [HttpPost("Admin/{action:slugify}")]
        public async Task<IActionResult> SignIn(SignInModel signInModel)
        {
            if (_signInManager.IsSignedIn(User))
                return Json(new
                {
                    success = false,
                    messages = new[] {OtherMessages.AlreadySignedIn}
                });
            
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(signInModel.Username, signInModel.Password, false, true);

                if (result.Succeeded)
                    return Json(new
                    {
                        success = true,
                        destination = string.IsNullOrWhiteSpace(TempData["ReturnUrl"]?.ToString()) ? Url.Action("YourProfile", "UsersManager") : TempData["ReturnUrl"]
                    });
                
                if (result.IsLockedOut)
                    ModelState.AddModelError("LockedOut", OtherMessages.TooManyFailedSignIn);
                else
                    ModelState.AddModelError("InvalidCredentials", ValidationMessages.InvalidCredentials);
            }
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [Authorize]
        [HttpGet("Admin/{action:slugify}")]
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Content");
        }

        [HttpGet("Admin/{action:slugify}")]
        public async Task<IActionResult> ForgotPassword()
        {
            if (!await _parameterManager.GetValue<bool>(ParameterTypes.EmailSenderEnabled))
                return RedirectToAction("Index","Content");
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UsersManager") : View();
        }

        [HttpPost("Admin/{action:slugify}")]
        public async Task<IActionResult> ForgotPassword(EmailModel forgotPasswordModel)
        {
            if (!await _parameterManager.GetValue<bool>(ParameterTypes.EmailSenderEnabled))
            {
                ModelState.AddModelError("FunctionalityDisabled", OtherMessages.DisableFunctionality);

                return Json(new
                {
                    success = false,
                    messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
                });
            }
            
            if (_signInManager.IsSignedIn(User))
                return Json(new
                {
                    success = false,
                    messages = new[] {OtherMessages.AlreadySignedInChangePassword}
                });

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);

                if (user != null)
                {
                    var token = (await _userManager.GeneratePasswordResetTokenAsync(user)).EncodeToBase64();
                    var siteName = await _parameterManager.GetValue<string>(ParameterTypes.SiteName);
                    var siteEmail = await _parameterManager.GetValue<string>(ParameterTypes.SiteEmail);
                    var message = string.Format(Emails.PasswordResetEmailBody, user.UserName, siteName, Url.Action("ResetPassword", "SignIn", new {userId = user.Id, token = "replace" }, Request.Scheme).Replace("replace", token));
                    
                    _emailSender.SendEmail(siteEmail, user.Email, string.Format(Emails.PasswordResetEmailSubject, siteName), message);
                }

                TempData["SuccessMessage"] = new[] {OtherMessages.EmailAlreadyInDatabaseWarning};
                
                return Json(new
                {
                    success = true,
                    destination = Url.Action("SignIn")
                });
            }
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        } 
        
        [HttpGet("Admin/{action:slugify}")]
        public async Task<IActionResult> ResetPassword(Guid userId, string token)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("YourProfile", "UsersManager");

            if (await _userManager.FindByIdAsync(userId.ToString()) != null && !string.IsNullOrWhiteSpace(token))
                return View(new ResetPasswordModel
                {
                    UserId = userId,
                    Token = token
                });

            TempData["ErrorMessage"] = new[] {OtherMessages.PasswordResetInvalidData};

            return RedirectToAction("Index", "Content");
        } 
        
        [HttpPatch("Admin/{action:slugify}")]
        public async Task<IActionResult> ChangePassword(ResetPasswordModel resetPasswordModel)
        {
            if (_signInManager.IsSignedIn(User))
                return Json(new
                {
                    success = false,
                    messages = new[] {OtherMessages.AlreadySignedInChangePassword}
                });

            if (ModelState.IsValid && await _userManager.FindByIdAsync(resetPasswordModel.UserId.ToString()) is var user && user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token.DecodeFromBase64(), resetPasswordModel.NewPassword);

                if (result.Succeeded)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    
                    TempData["SuccessMessage"] = new[] {OtherMessages.PasswordChangedSuccessfully};
                    
                    return Json(new
                    {
                        success = true,
                        destination = Url.Action("SignIn")
                    });
                }
                
                ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotChangePassword);
            }
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpGet("Admin/{action:slugify}")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailModel changeEmailModel)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(changeEmailModel.Email) is var candidate && candidate != null)
                    TempData["ErrorMessage"] = new[] {ValidationMessages.EmailAlreadyInUse};
                else
                {
                    var user = await _userManager.FindByIdAsync(changeEmailModel.UserId.ToString());

                    if (user != null)
                    {
                        var result = await _userManager.ChangeEmailAsync(user, changeEmailModel.Email, changeEmailModel.Token.DecodeFromBase64());

                        if (result.Succeeded)
                            TempData["SuccessMessage"] = new[] {OtherMessages.EmailChangedSuccessfully};
                    }
                    else
                        ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotChangeEmail);
                }
            }
            else
                TempData["ErrorMessage"] = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage);
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UsersManager") : RedirectToAction("Index", "Content");
        }
    }
}