using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Services;
using WeebReader.Web.Models.Models.Shared;
using WeebReader.Web.Models.Models.UserManager;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize]
    public class UserManagerController : Controller
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly SettingManager _settingManager;
        private readonly EmailSender _emailSender;

        public UserManagerController(UserManager<IdentityUser<Guid>> userManager, SettingManager settingManager, EmailSender emailSender)
        {
            _userManager = userManager;
            _settingManager = settingManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> YourProfile()
        {
            ViewData["User"] = await _userManager.GetUserAsync(User);
            ViewData["Role"] = (await _userManager.GetRolesAsync((IdentityUser<Guid>) ViewData["User"])).FirstOrDefault();

            return View();
        }

        [HttpPatch]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (TryValidateModel(changePasswordModel))
            {
                var result = await _userManager.ChangePasswordAsync(await _userManager.GetUserAsync(User), changePasswordModel.Password, changePasswordModel.NewPassword);
                
                if(result.Succeeded)
                    return new JsonResult(new
                    {
                        success = true,
                        messages = new[] {"Password changed successfully."}
                    });

                ModelState.AddModelError("NotSucceeded", "We could not change your password, please verify your password and try again.");
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpPatch]
        public async Task<IActionResult> ChangeEmail(EmailModel emailModel)
        {
            if (TryValidateModel(emailModel))
            {
                if (await _userManager.FindByEmailAsync(emailModel.Email) is var candidate && candidate != null)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {"The requested e-mail is already in use."}
                    });

                var user = await _userManager.GetUserAsync(User);
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, emailModel.Email);
                var siteName = await _settingManager.GetValue("SiteName");
                var siteAddress = await _settingManager.GetValue("SiteAddress");
                var siteEmail = await _settingManager.GetValue("SiteEmail");
                
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"Hello {user.UserName},");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"An e-mail change was requested at {siteName}.");
                stringBuilder.AppendLine("Please go to the following URL to proceed with the change. If you changed your mind, you can safely ignore this email.");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"{siteAddress}{Url.Action("ChangeEmail", "SignIn", new {userId = user.Id, email = emailModel.Email, token})}");
                
                var result =  await _emailSender.SendEmail(siteEmail, emailModel.Email, $"Change E-mail - {siteName}", stringBuilder.ToString());

                if(result)
                    return new JsonResult(new
                    {
                        success = true,
                        messages = new[] {"An e-mail was sent to the new e-mail with the details on how to proceed with the change."}
                    });
                
                ModelState.AddModelError("NotSucceeded", "We could not send a confirmation email, please try again or contact an administrator.");
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
    }
}