using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Localization.Utilities;
using WeebReader.Web.Models;
using WeebReader.Web.Models.Home;
using WeebReader.Web.Models.UsersManager;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize]
    [Route("Admin/Users/")]
    public class UsersManagerController : Controller
    {
        private readonly BaseContext _context;
        private readonly EmailSender _emailSender;
        private readonly ParameterManager _parameterManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public UsersManagerController(BaseContext context, EmailSender emailSender, ParameterManager parameterManager, UserManager<IdentityUser<Guid>> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _parameterManager = parameterManager;
            _userManager = userManager;
        }

        [HttpGet("{page:int?}")]
        [Authorize(Roles = RoleTranslator.Administrator)]
        public async Task<IActionResult> Index(ushort page = 1)
        {
            var totalPages = Math.Ceiling(await _userManager.Users.LongCountAsync() / (decimal) Constants.ItemsPerPage);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1);

            var users = _userManager.Users.Skip(Constants.ItemsPerPage * (page - 1)).Take(Constants.ItemsPerPage)
                .Select(Mapper.Map).ToArray();
            
            foreach (var user in users)
                user.Role = RoleTranslator.FromRole((await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.UserId.ToString()))).FirstOrDefault());

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["DeletionRoute"] = Url.Action("Delete", new {userId = Guid.Empty}).Replace(Guid.Empty.ToString(), string.Empty);
            
            return View(users);
        }

        [HttpGet("{action}")]
        [Authorize(Roles = RoleTranslator.Administrator)]
        public IActionResult Add()
        {
            ViewData["Title"] = Labels.AddUser;
            ViewData["ActionRoute"] = Url.Action("Add");
            ViewData["Method"] = "POST";
            
            return View("UserEditor");
        }

        [HttpPost("{action}")]
        [Authorize(Roles = RoleTranslator.Administrator)]
        public async Task<IActionResult> Add(UserModel userModel)
        {
            if (TryValidateModel(userModel))
            {
                if (await _userManager.FindByEmailAsync(userModel.Email) != null)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.EmailAlreadyInUse}
                    });
                
                if (await _userManager.FindByNameAsync(userModel.Username) != null)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.UsernameAlreadyInUse}
                    });

                var user = Mapper.Map(userModel);

                await using var transaction = await _context.Database.BeginTransactionAsync();
                
                if (await _parameterManager.GetValue<bool>(Parameter.Types.EmailEnabled))
                {
                    var userResult = string.IsNullOrWhiteSpace(userModel.Password) ? await _userManager.CreateAsync(user) : await _userManager.CreateAsync(user, userModel.Password);

                    if (userResult.Succeeded)
                    {
                        if (RoleTranslator.ToRole(userModel.Role) is var resultingRole && resultingRole != null)
                        {
                            var roleResult = await _userManager.AddToRoleAsync(user, resultingRole);

                            if (roleResult.Succeeded)
                            {
                                TempData["SuccessMessage"] = new[] {OtherMessages.UserCreatedEmailSent};

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
                            TempData["SuccessMessage"] = new[] {OtherMessages.UserCreatedEmailSent};

                            await transaction.CommitAsync();
                            await SendAccountCreationEmail(user);
                            
                            return new JsonResult(new
                            {
                                success = true,
                                destination = Url.Action("Index")
                            });
                        }
                    }
                    
                    ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
                }
                else
                {
                    user.EmailConfirmed = true;

                    if (!string.IsNullOrWhiteSpace(userModel.Password))
                    {
                        var userResult = await _userManager.CreateAsync(user, userModel.Password);
                        
                        if (userResult.Succeeded)
                        {
                            if (RoleTranslator.ToRole(userModel.Role) is var resultingRole && resultingRole != null)
                            {
                                var roleResult = await _userManager.AddToRoleAsync(user, resultingRole);

                                if (roleResult.Succeeded)
                                {
                                    TempData["SuccessMessage"] = new[] {OtherMessages.UserCreatedSuccessfully};

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
                                TempData["SuccessMessage"] = new[] {OtherMessages.UserCreatedSuccessfully};

                                await transaction.CommitAsync();

                                return new JsonResult(new
                                {
                                    success = true,
                                    destination = Url.Action("Index")
                                });
                            }
                        }
                        
                        ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
                    }
                    else
                        ModelState.AddModelError("InvalidPassword", ValidationMessages.PasswordRequired);
                }
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpGet("{userId:guid}")]
        [Authorize(Roles = RoleTranslator.Administrator)]
        public async Task<IActionResult> Edit(Guid userId)
        {
            if (await _userManager.FindByIdAsync(userId.ToString()) is var user && user == null)
            {
                ViewData["ErrorMessage"] = new[] {ValidationMessages.UserNotFound};
                
                return RedirectToAction("Index");
            }
            
            ViewData["Title"] = Labels.EditUser;
            ViewData["ActionRoute"] = Url.Action("Edit", new {userId});
            ViewData["Method"] = "PATCH";

            var model = Mapper.Map(user);
            model.Role = RoleTranslator.FromRole((await _userManager.GetRolesAsync(user)).FirstOrDefault());

            return View("UserEditor", model);
        }

        [HttpPatch("{userId:guid}")]
        [Authorize(Roles = RoleTranslator.Administrator)]
        public async Task<IActionResult> Edit(UserModel userModel)
        {
            if (TryValidateModel(userModel))
            {
                if (userModel.UserId == null || await _userManager.FindByIdAsync(userModel.UserId.ToString()) is var user && user == null)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.UserNotFound}
                    });
                
                if (await _userManager.FindByEmailAsync(userModel.Email) is var emailUser && emailUser != null && emailUser != user)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.EmailAlreadyInUse}
                    });

                if ((await _userManager.GetRolesAsync(user)).SingleOrDefault() is var role && role == RoleTranslator.Administrator && (await _userManager.GetUsersInRoleAsync(RoleTranslator.Administrator)).Count == 1 && RoleTranslator.ToRole(userModel.Role) != role)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.UserUpdateIsLastAdministrator}
                    });

                Mapper.Map(userModel, ref user);
                user.EmailConfirmed = true;
                
                if (!string.IsNullOrWhiteSpace(userModel.Password))
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userModel.Password);

                await using var transaction = await _context.Database.BeginTransactionAsync();

                if ((await _userManager.UpdateAsync(user)).Succeeded)
                {
                    var roleResult = true;

                    if (RoleTranslator.ToRole(userModel.Role) is var targetRole && targetRole != role && targetRole != null)
                    {
                        roleResult = (role == null || (await _userManager.RemoveFromRoleAsync(user, role)).Succeeded) && (await _userManager.AddToRoleAsync(user, targetRole)).Succeeded;
                    }
                    else if (role != null && targetRole == null)
                    {
                        roleResult = (await _userManager.RemoveFromRoleAsync(user, role)).Succeeded;
                    }

                    if (roleResult)
                    {
                        await transaction.CommitAsync();
                        
                        TempData["SuccessMessage"] = new[] {OtherMessages.UserUpdatedSuccessfully};
                        
                        return new JsonResult(new
                        {
                            success = true,
                            destination = Url.Action("Index")
                        });
                    }
                }
                
                ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            if (await _userManager.FindByIdAsync(userId.ToString()) is var user && user == null)
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.UserNotFound}
                });

            if ((await _userManager.GetRolesAsync(user)).SingleOrDefault() == RoleTranslator.Administrator && (await _userManager.GetUsersInRoleAsync(RoleTranslator.Administrator)).Count == 1)
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.UserDeleteIsLastAdministrator}
                });

            if ((await _userManager.DeleteAsync(user)).Succeeded)
            {
                TempData["SuccessMessage"] = new[] {OtherMessages.UserDeletedSuccessfully};

                return new JsonResult(new
                {
                    success = true
                });
            }

            return new JsonResult(new
            {
                success = false,
                messages = new[] {OtherMessages.SomethingWrong}
            });
        }
        
        [HttpGet("{action:slugify}")]
        public async Task<IActionResult> YourProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = Mapper.Map(await _userManager.GetUserAsync(User));
            model.Role = RoleTranslator.FromRole((await _userManager.GetRolesAsync(user)).FirstOrDefault());

            return View(model);
        }

        [HttpPatch("{action:slugify}")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (TryValidateModel(changePasswordModel))
            {
                var result = await _userManager.ChangePasswordAsync(await _userManager.GetUserAsync(User), changePasswordModel.Password, changePasswordModel.NewPassword);
                
                if(result.Succeeded)
                    return new JsonResult(new
                    {
                        success = true,
                        messages = new[] {OtherMessages.PasswordChangedSuccessfully}
                    });

                ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotChangePassword);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpPatch("{action:slugify}")]
        public async Task<IActionResult> ChangeEmail(EmailModel emailModel)
        {
            if (TryValidateModel(emailModel))
            {
                if (await _userManager.FindByEmailAsync(emailModel.Email) is var candidate && candidate != null)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.EmailAlreadyInUse}
                    });

                var user = await _userManager.GetUserAsync(User);

                if (await _parameterManager.GetValue<bool>(Parameter.Types.EmailEnabled))
                {
                    var token = await _userManager.GenerateChangeEmailTokenAsync(user, emailModel.Email);
                    var siteName = await _parameterManager.GetValue<string>(Parameter.Types.SiteName);
                    var siteAddress = await _parameterManager.GetValue<string>(Parameter.Types.SiteAddress);
                    var siteEmail = await _parameterManager.GetValue<string>(Parameter.Types.SiteEmail);

                    var message = string.Format(Emails.ChangeEmailBody, user.UserName, siteName, $"{siteAddress}{Url.Action("ChangeEmail", "Home", new {userId = user.Id, email = emailModel.Email, token})}");

                    var result =  await _emailSender.SendEmail(siteEmail, emailModel.Email, string.Format(Emails.ChangeEmailSubject, siteName), message);

                    if(result)
                        return new JsonResult(new
                        {
                            success = true,
                            messages = new[] {OtherMessages.ChangeEmailSent}
                        });
                    
                    ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotSendConfirmationEmail);
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
                            messages = new[] {OtherMessages.EmailChangedSuccessfully}
                        });   
                    
                    ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotChangeEmail);
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
            var siteName = await _parameterManager.GetValue<string>(Parameter.Types.SiteName);
            var siteAddress = await _parameterManager.GetValue<string>(Parameter.Types.SiteAddress);
            var siteEmail = await _parameterManager.GetValue<string>(Parameter.Types.SiteEmail);

            var message = string.Format(Emails.AccountCreationEmailBody, user.UserName, siteName, $"{siteAddress}{Url.Action("ResetPassword", "Home", new {userId = user.Id, token})}");

            return await _emailSender.SendEmail(siteEmail, user.Email, string.Format(Emails.AccountCreationEmailSubject, siteName), message);
        }
    }
}