using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Others;
using WeebReader.Data.Entities;
using WeebReader.Data.Services;
using WeebReader.Web.Models;
using WeebReader.Web.Models.Models.SignIn;
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
            var users = _userManager.Users.Skip(Constants.ItemsPerPage * (page - 1)).Take(Constants.ItemsPerPage)
                .Select(user => new UserModel
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = null
                }).ToArray();

            foreach (var user in users)
                user.Role = RoleMapper.Map((await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.UserId.ToString()))).FirstOrDefault() ?? PortalMessages.MSG054);
            
            return new JsonResult(new
            {
                success = true,
                page,
                totalPages = Math.Ceiling(await _userManager.Users.LongCountAsync() / (decimal) Constants.ItemsPerPage),
                users
            });
        }

        [HttpGet]
        [Authorize(Roles = RoleMapper.Administrator)]
        public IActionResult Add() => View();
        
        [HttpPost]
        [Authorize(Roles = RoleMapper.Administrator)]
        public async Task<IActionResult> Add(UserModel userModel)
        {
            if (TryValidateModel(userModel))
            {
                if (await _userManager.FindByEmailAsync(userModel.Email) != null)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {PortalMessages.MSG013}
                    });
                
                if (await _userManager.FindByNameAsync(userModel.Username) != null)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {PortalMessages.MSG072}
                    });

                var user = new IdentityUser<Guid>
                {
                    UserName = userModel.Username,
                    Email = userModel.Email
                };
                
                if (await _settingManager.GetValue<bool>(Setting.Keys.EmailEnabled))
                {
                    var userResult = await _userManager.CreateAsync(user, userModel.Password);

                    if (userResult.Succeeded)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var siteName = await _settingManager.GetValue(Setting.Keys.SiteName);
                        var siteAddress = await _settingManager.GetValue(Setting.Keys.SiteAddress);
                        var siteEmail = await _settingManager.GetValue(Setting.Keys.SiteEmail);

                        var message = string.Format(PortalMessages.MSG071, user.UserName, siteName, $"{siteAddress}{Url.Action("ResetPassword", "SignIn", new {userId = user.Id, token})}");

                        await _emailSender.SendEmail(siteEmail, user.Email, string.Format(PortalMessages.MSG070, siteName), message);
                        
                        TempData["SuccessMessage"] = new[] {PortalMessages.MSG069};

                        return new JsonResult(new
                        {
                            success = true,
                            destination = Url.Action("Index")
                        });
                    }
                    
                    ModelState.AddModelError("SomethingWrong", PortalMessages.MSG055);
                }
                else
                {
                    user.EmailConfirmed = true;

                    if (!string.IsNullOrWhiteSpace(userModel.Password))
                    {
                        var userResult = await _userManager.CreateAsync(user, userModel.Password);
                        
                        if (userResult.Succeeded)
                        {
                            if (RoleMapper.UnMap(userModel.Role) is var resultingRole && resultingRole != null)
                            {
                                var roleResult = await _userManager.AddToRoleAsync(user, resultingRole);

                                if (roleResult.Succeeded)
                                {
                                    TempData["SuccessMessage"] = new[] {PortalMessages.MSG068};

                                    return new JsonResult(new
                                    {
                                        success = true,
                                        destination = Url.Action("Index")
                                    });
                                }
                                
                                await _userManager.DeleteAsync(user);
                            }
                            else
                            {
                                TempData["SuccessMessage"] = new[] {PortalMessages.MSG068};

                                return new JsonResult(new
                                {
                                    success = true,
                                    destination = Url.Action("Index")
                                });
                            }
                        }
                        
                        ModelState.AddModelError("SomethingWrong", PortalMessages.MSG055);
                    }
                    else
                        ModelState.AddModelError("InvalidPassword", ModelMessages.MSG006);
                }
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = new[] {ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)}
            });
        }

        [HttpPatch("{userId:guid?}")]
        [Authorize(Roles = RoleMapper.Administrator)]
        public async Task<IActionResult> Edit(Guid userId, UserModel userModel)
        {
            if (TryValidateModel(userModel))
            {
                if (await _userManager.FindByIdAsync(userId.ToString()) is var user && user == null)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {PortalMessages.MSG065}
                    });

                if ((await _userManager.GetRolesAsync(user)).SingleOrDefault() == RoleMapper.Administrator && (await _userManager.GetUsersInRoleAsync(RoleMapper.Administrator)).Count == 1)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {PortalMessages.MSG066}
                    });

                var removeResult = await _userManager.RemoveFromRolesAsync(user, new[] {RoleMapper.Administrator, RoleMapper.Moderator, RoleMapper.Uploader});

                if (removeResult.Succeeded)
                {
                    if (RoleMapper.UnMap(userModel.Role) is var resultingRole && resultingRole != null)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, resultingRole);

                        if (roleResult.Succeeded)
                        {
                            TempData["SuccessMessage"] = new[] {PortalMessages.MSG067};

                            return new JsonResult(new
                            {
                                success = true,
                                destination = Url.Action("Index")
                            });
                        }
                    }
                    else
                    {
                        TempData["SuccessMessage"] = new[] {PortalMessages.MSG067};

                        return new JsonResult(new
                        {
                            success = true,
                            destination = Url.Action("Index")
                        });
                    }
                }
            }

            ModelState.AddModelError("SomethingWrong", PortalMessages.MSG055);

            return new JsonResult(new
            {
                success = false,
                messages = new[] {ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)}
            });
        }
        
        [HttpGet]
        public async Task<IActionResult> YourProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(new UserModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = RoleMapper.Map((await _userManager.GetRolesAsync(user)).FirstOrDefault())
            });
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