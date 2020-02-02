using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Others;
using WeebReader.Data.Entities;
using WeebReader.Data.Services;
using WeebReader.Web.Models.Models.Shared;
using WeebReader.Web.Models.Models.UserManager;
using WeebReader.Web.Portal.Others;
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

        [HttpGet("{page:int?}")]
        [Authorize(Roles = RoleMapper.Administrator)]
        public async Task<IActionResult> Index(ushort page = 1)
        {
            ViewData["Page"] = page > 1 ? page : 1;
            ViewData["TotalPages"] = Math.Ceiling(await _userManager.Users.LongCountAsync() / (decimal) Constants.ItemsPerPage);

            return View();
        }

        [HttpGet("{page:int?}")]
        [Authorize(Roles = RoleMapper.Administrator)]
        public async Task<IActionResult> List(ushort page = 1)
        {
            page = (ushort) (page > 1 ? page : 1);
            var users = _userManager.Users.Skip(Constants.ItemsPerPage * (page - 1)).Take(Constants.ItemsPerPage).AsEnumerable()
                .Select(user =>
                {
                    dynamic result = new ExpandoObject();

                    result.id = user.Id;
                    result.userName = user.UserName;
                    result.email = user.Email;
                    result.role = null;

                    return result;
                }).ToArray();

            foreach (var user in users)
                user.role = RoleMapper.Map((await _userManager.GetRolesAsync(await _userManager.FindByIdAsync((string) user.id.ToString()))).FirstOrDefault()) ?? PortalMessages.MSG054;
            
            return new JsonResult(new
            {
                success = true,
                page,
                totalPages = Math.Ceiling(await _userManager.Users.LongCountAsync() / (decimal) Constants.ItemsPerPage),
                users
            });
        }

        [Authorize(Roles = RoleMapper.Administrator)]
        public IActionResult Add()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{userId:guid?}")]
        [Authorize(Roles = RoleMapper.Administrator)]
        public IActionResult Edit(Guid userId)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet]
        public async Task<IActionResult> YourProfile()
        {
            ViewData["User"] = await _userManager.GetUserAsync(User);
            ViewData["Role"] = RoleMapper.Map((await _userManager.GetRolesAsync((IdentityUser<Guid>) ViewData["User"])).FirstOrDefault());

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

                if (await _settingManager.GetValue<bool>(Setting.Keys.EmailEnabled))
                {
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
                else
                {
                    var result = await _userManager.ChangeEmailAsync(user, emailModel.Email, token);
                    
                    if(result.Succeeded)
                        return new JsonResult(new
                        {
                            success = true,
                            messages = new[] {PortalMessages.MSG014}
                        });   
                    
                    ModelState.AddModelError("NotSucceeded", PortalMessages.MSG015);
                }
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
    }
}