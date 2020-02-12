using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
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
        private readonly BaseContext _context;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly SettingManager _settingManager;
        private readonly EmailSender _emailSender;

        public UserManagerController(BaseContext context, UserManager<IdentityUser<Guid>> userManager, SettingManager settingManager, EmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _settingManager = settingManager;
            _emailSender = emailSender;
        }

        [HttpGet("{page:int?}")]
        [Authorize(Roles = RoleMapper.Administrator)]
        public async Task<IActionResult> Index(ushort page = 1)
        {
            var totalPages = Math.Ceiling(await _userManager.Users.LongCountAsync() / (decimal) Constants.ItemsPerPage);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1); 

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

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            
            return View(users);
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

                await using var transaction = await _context.Database.BeginTransactionAsync();
                
                if (await _settingManager.GetValue<bool>(Setting.Keys.EmailEnabled))
                {
                    var userResult = string.IsNullOrWhiteSpace(userModel.Password) ? await _userManager.CreateAsync(user) : await _userManager.CreateAsync(user, userModel.Password);

                    if (userResult.Succeeded)
                    {
                        if (RoleMapper.UnMap(userModel.Role) is var resultingRole && resultingRole != null)
                        {
                            var roleResult = await _userManager.AddToRoleAsync(user, resultingRole);

                            if (roleResult.Succeeded)
                            {
                                TempData["SuccessMessage"] = new[] {PortalMessages.MSG068};

                                await transaction.CommitAsync();
                                await SendAccountCreationEmail(user);

                                return new JsonResult(new
                                {
                                    success = true,
                                    destination = Url.Action("Index")
                                });
                            }
                        }
                        else
                        {
                            TempData["SuccessMessage"] = new[] {PortalMessages.MSG068};

                            await transaction.CommitAsync();
                            await SendAccountCreationEmail(user);
                            
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

                                    await transaction.CommitAsync();

                                    return new JsonResult(new
                                    {
                                        success = true,
                                        destination = Url.Action("Index")
                                    });
                                }
                            }
                            else
                            {
                                TempData["SuccessMessage"] = new[] {PortalMessages.MSG068};

                                await transaction.CommitAsync();

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

        [HttpGet("{userId:guid?}")]
        [Authorize(Roles = RoleMapper.Administrator)]
        public async Task<IActionResult> Edit(Guid userId)
        {
            if (await _userManager.FindByIdAsync(userId.ToString()) is var user && user == null)
            {
                ViewData["ErrorMessage"] = new[] {PortalMessages.MSG065};
                
                return RedirectToAction("Index");
            }

            return View(new UserModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = RoleMapper.Map((await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? PortalMessages.MSG054)
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
                
                if (await _userManager.FindByEmailAsync(userModel.Email) != user)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {PortalMessages.MSG013}
                    });

                if ((await _userManager.GetRolesAsync(user)).SingleOrDefault() is var role && role == RoleMapper.Administrator && (await _userManager.GetUsersInRoleAsync(RoleMapper.Administrator)).Count == 1 && RoleMapper.UnMap(userModel.Role) != role)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {PortalMessages.MSG066}
                    });

                user.UserName = userModel.Username;
                user.Email = userModel.Email;
                user.EmailConfirmed = true;
                
                if (!string.IsNullOrWhiteSpace(userModel.Password))
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userModel.Password);

                await using var transaction = await _context.Database.BeginTransactionAsync();

                if ((await _userManager.UpdateAsync(user)).Succeeded)
                {
                    var roleResult = true;

                    if (role != RoleMapper.UnMap(userModel.Role))
                        roleResult = (await _userManager.RemoveFromRoleAsync(user, role)).Succeeded && (await _userManager.AddToRoleAsync(user, RoleMapper.UnMap(userModel.Role))).Succeeded;

                    if (roleResult)
                    {
                        await transaction.CommitAsync();
                        
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

                if (await _settingManager.GetValue<bool>(Setting.Keys.EmailEnabled))
                {
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
                else
                {
                    user.Email = emailModel.Email;
                    user.EmailConfirmed = true;
                    
                    var result = await _userManager.UpdateAsync(user);
                    
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

        private async Task<bool> SendAccountCreationEmail(IdentityUser<Guid> user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var siteName = await _settingManager.GetValue(Setting.Keys.SiteName);
            var siteAddress = await _settingManager.GetValue(Setting.Keys.SiteAddress);
            var siteEmail = await _settingManager.GetValue(Setting.Keys.SiteEmail);

            var message = string.Format(PortalMessages.MSG071, user.UserName, siteName, $"{siteAddress}{Url.Action("ResetPassword", "SignIn", new {userId = user.Id, token})}");

            return await _emailSender.SendEmail(siteEmail, user.Email, string.Format(PortalMessages.MSG070, siteName), message);
        }
    }
}