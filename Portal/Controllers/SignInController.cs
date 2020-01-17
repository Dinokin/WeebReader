using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WeebReader.Data.Services;
using WeebReader.Web.Portal.Models.Shared;
using WeebReader.Web.Portal.Models.SignIn;
using WeebReader.Web.Portal.Services;

namespace WeebReader.Web.Portal.Controllers
{
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

        public IActionResult Index(string returnUrl)
        {
            TempData["ReturnUrl"] = !string.IsNullOrWhiteSpace(returnUrl) ? returnUrl : null;
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UserManager") : (IActionResult) View();
        }
        
        public async Task<IActionResult> SignIn(SignInModel signInModel)
        {
            if (_signInManager.IsSignedIn(User))
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {"You're already signed in."}
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

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            if (await _settingManager.GetValue<bool>("EmailEnabled") || _signInManager.IsSignedIn(User))
                return RedirectToAction("Index");
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UserManager") : (IActionResult) View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(EmailModel forgotPasswordModel)
        {
            if (await _settingManager.GetValue<bool>("EmailEnabled"))
            {
                ModelState.AddModelError("FunctionalityDisabled", "This functionality is disabled, please contact an administrator.");

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
                    messages = new[] {"You're already signed in. Please sign out or change your password in the administrator panel."}
                });

            if (TryValidateModel(forgotPasswordModel))
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var siteName = await _settingManager.GetValue("SiteName");
                    var siteAddress = await _settingManager.GetValue("SiteAddress");
                    var siteEmail = await _settingManager.GetValue("SiteEmail");
                
                    var stringBuilder = new StringBuilder();

                    stringBuilder.AppendLine($"Hello {user.UserName},");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine($"A password reset was requested at {siteName}.");
                    stringBuilder.AppendLine("Please go to the following URL to proceed with the reset. You can safely ignore this email if you didn't request this.");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine($"{siteAddress}{Url.Action("ResetPassword", new {userId = user.Id, token})}");

                    await _emailSender.SendEmail(siteEmail, user.Email, $"Password Reset - {siteName}", stringBuilder.ToString());
                }

                TempData["SuccessMessage"] = new[] {"If that address exists in our database, an e-mail will be send with the details on how to proceed with the password reset."};
                
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
        
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("YourProfile", "UserManager");
            
            TryValidateModel(resetPasswordModel);

            if (ModelState["Token"].ValidationState == ModelValidationState.Valid && await _userManager.FindByIdAsync(resetPasswordModel.UserId.ToString()) != null)
                return View(resetPasswordModel);

            TempData["ErrorMessage"] = new[] {"We cannot proceed with the password reset because invalid data was supplied."};

            return RedirectToAction("Index");
        } 
        
        public async Task<IActionResult> ProcessResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (_signInManager.IsSignedIn(User))
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {"You're already signed in. Please sign out or change your password in the administrator panel."}
                });

            if (TryValidateModel(resetPasswordModel) && await _userManager.FindByIdAsync(resetPasswordModel.UserId.ToString()) is var user && user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.NewPassword);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = new[] {"Your password was changed successfully. You can now sign in using your new password."};
                    
                    return new JsonResult(new
                    {
                        success = true,
                        destination = Url.Action("Index")
                    });
                }
                
                ModelState.AddModelError("NotSucceeded", "We could not complete your password reset. Please try again or restart the process.");
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        public async Task<IActionResult> ChangeEmail(ChangeEmailModel changeEmailModel)
        {
            if (TryValidateModel(changeEmailModel))
            {
                if (await _userManager.FindByEmailAsync(changeEmailModel.Email) is var candidate && candidate != null)
                    TempData["ErrorMessage"] = new[] {"The requested e-mail is already in use."};
                else
                {
                    var user = await _userManager.FindByIdAsync(changeEmailModel.UserId.ToString());

                    if (user != null)
                    {
                        var result = await _userManager.ChangeEmailAsync(user, changeEmailModel.Email, changeEmailModel.Token);

                        if (result.Succeeded)
                            TempData["SuccessMessage"] = new[] {"Your e-mail was changed successfully."};
                    }
                    else
                        ModelState.AddModelError("NotSucceeded", "We could not change your e-mail. Please try again or contact an administrator.");
                }
            }
            else
                TempData["ErrorMessage"] = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage);
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UserManager") : RedirectToAction("Index");
        }
    }
}