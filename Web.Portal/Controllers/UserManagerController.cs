using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Data.Services;
using WeebReader.Web.Models.Models.Shared;
using WeebReader.Web.Models.Models.UserManager;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize]
    [Route("Admin/Users/{action:slugify=Index}")]
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
                        messages = new[] {PortalMessages.MSG011}
                    });

                ModelState.AddModelError("NotSucceeded", PortalMessages.MSG016);
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
                        messages = new[] {PortalMessages.MSG013}
                    });

                var user = await _userManager.GetUserAsync(User);
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, emailModel.Email);
                var siteName = await _settingManager.GetValue(Setting.Keys.SiteName);
                var siteAddress = await _settingManager.GetValue(Setting.Keys.SiteAddress);
                var siteEmail = await _settingManager.GetValue(Setting.Keys.SiteEmail);

                var message = string.Format(PortalMessages.MSG017, user.UserName, siteName, $"{siteAddress}{Url.Action("ChangeEmail", "SignIn", new {userId = user.Id, email = emailModel.Email, token})}");

                var result =  await _emailSender.SendEmail(siteEmail, emailModel.Email, string.Format(PortalMessages.MSG018, siteName), message);

                if(result)
                    return new JsonResult(new
                    {
                        success = true,
                        messages = new[] {PortalMessages.MSG019}
                    });
                
                ModelState.AddModelError("NotSucceeded", PortalMessages.MSG020);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
    }
}