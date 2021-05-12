using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Controllers.SignIn;
using WeebReader.Web.Models.Controllers.UsersManager;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Extensions;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Portal.Others.Extensions;
using WeebReader.Web.Services;
using Utilities = WeebReader.Web.Localization.Others.Utilities;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize]
    [Route("Admin/Users/")]
    public class UsersManagerController : Controller
    {
        private readonly BaseContext _context;
        private readonly EmailSender _emailSender;
        private readonly ParametersManager _parameterManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public UsersManagerController(BaseContext context, EmailSender emailSender, ParametersManager parameterManager, UserManager<IdentityUser<Guid>> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _parameterManager = parameterManager;
            _userManager = userManager;
        }

        [HttpGet("{page:int?}")]
        [Authorize(Roles = Utilities.Roles.Administrator)]
        public async Task<IActionResult> Index(ushort page = 1)
        {
            var totalPages = Math.Ceiling(await _userManager.Users.LongCountAsync() / (decimal) Constants.ItemsPerPageUserAdmin);
            page = page >= 1 && page <= totalPages ? page : (ushort) 1;

            var users = _userManager.Users.OrderBy(user => user.UserName).Skip(Constants.ItemsPerPageUserAdmin * (page - 1)).Take(Constants.ItemsPerPageUserAdmin).AsEnumerable()
                .Select(Mapper.MapToModel).ToArray();
            
            foreach (var user in users)
                user.Role = Utilities.FromRole((await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.UserId.ToString()))).FirstOrDefault());

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["DeletionRoute"] = Url.Action("Delete", new {userId = Guid.Empty}).Replace(Guid.Empty.ToString(), string.Empty);
            
            return View(users);
        }

        [HttpGet("{action}")]
        [Authorize(Roles = Utilities.Roles.Administrator)]
        public IActionResult Add()
        {
            ViewData["Title"] = Labels.AddUser;
            ViewData["ActionRoute"] = Url.Action("Add");
            ViewData["Method"] = "POST";
            
            return View("UserEditor");
        }

        [HttpPost("{action}")]
        [Authorize(Roles = Utilities.Roles.Administrator)]
        public async Task<IActionResult> Add(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(userModel.Email) != null)
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.EmailAlreadyInUse}
                    });
                
                if (await _userManager.FindByNameAsync(userModel.Username) != null)
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.UsernameAlreadyInUse}
                    });

                var user = Mapper.MapToEntity(userModel);

                await using var transaction = await _context.Database.BeginTransactionAsync();
                
                if (await _parameterManager.GetValue<bool>(ParameterTypes.EmailSenderEnabled))
                {
                    var userResult = string.IsNullOrWhiteSpace(userModel.Password) ? await _userManager.CreateAsync(user) : await _userManager.CreateAsync(user, userModel.Password);

                    if (userResult.Succeeded)
                    {
                        if (Utilities.ToRole(userModel.Role) is var resultingRole && resultingRole != null)
                        {
                            var roleResult = await _userManager.AddToRoleAsync(user, resultingRole);

                            if (roleResult.Succeeded)
                            {
                                TempData["SuccessMessage"] = new[] {OtherMessages.UserCreatedEmailSent};

                                await transaction.CommitAsync();
                                await SendAccountCreationEmail(user);

                                return Json(new
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
                            
                            return Json(new
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
                            if (Utilities.ToRole(userModel.Role) is var resultingRole && resultingRole != null)
                            {
                                var roleResult = await _userManager.AddToRoleAsync(user, resultingRole);

                                if (roleResult.Succeeded)
                                {
                                    TempData["SuccessMessage"] = new[] {OtherMessages.UserCreatedSuccessfully};

                                    await transaction.CommitAsync();

                                    return Json(new
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

                                return Json(new
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
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpGet("{userId:guid}")]
        [Authorize(Roles = Utilities.Roles.Administrator)]
        public async Task<IActionResult> Edit(Guid userId)
        {
            if (await _userManager.FindByIdAsync(userId.ToString()) is var user && user == null)
            {
                TempData["ErrorMessage"] = new[] {ValidationMessages.UserNotFound};
                
                return RedirectToAction("Index");
            }
            
            ViewData["Title"] = Labels.EditUser;
            ViewData["ActionRoute"] = Url.Action("Edit", new {userId});
            ViewData["Method"] = "PATCH";

            var model = Mapper.MapToModel(user);
            model.Role = Utilities.FromRole((await _userManager.GetRolesAsync(user)).FirstOrDefault());

            return View("UserEditor", model);
        }

        [HttpPatch("{userId:guid}")]
        [Authorize(Roles = Utilities.Roles.Administrator)]
        public async Task<IActionResult> Edit(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                if (userModel.UserId == null || await _userManager.FindByIdAsync(userModel.UserId.ToString()) is var user && user == null)
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.UserNotFound}
                    });
                
                if (await _userManager.FindByEmailAsync(userModel.Email) is var emailUser && emailUser != null && emailUser != user)
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.EmailAlreadyInUse}
                    });

                if ((await _userManager.GetRolesAsync(user)).SingleOrDefault() is var role && role == Utilities.Roles.Administrator && (await _userManager.GetUsersInRoleAsync(Utilities.Roles.Administrator)).Count == 1 && Utilities.ToRole(userModel.Role) != role)
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.UserUpdateIsLastAdministrator}
                    });

                Mapper.MapEditModelToEntity(userModel, ref user);
                user.EmailConfirmed = true;
                
                if (!string.IsNullOrWhiteSpace(userModel.Password))
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userModel.Password);

                await using var transaction = await _context.Database.BeginTransactionAsync();

                if ((await _userManager.UpdateAsync(user)).Succeeded)
                {
                    var roleResult = true;

                    if (Utilities.ToRole(userModel.Role) is var targetRole && targetRole != role && targetRole != null)
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
                        
                        return Json(new
                        {
                            success = true,
                            destination = Url.Action("Index")
                        });
                    }
                }
                
                ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
            }
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            if (await _userManager.FindByIdAsync(userId.ToString()) is var user && user == null)
                return Json(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.UserNotFound}
                });

            if ((await _userManager.GetRolesAsync(user)).SingleOrDefault() == Utilities.Roles.Administrator && (await _userManager.GetUsersInRoleAsync(Utilities.Roles.Administrator)).Count == 1)
                return Json(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.UserDeleteIsLastAdministrator}
                });

            if ((await _userManager.DeleteAsync(user)).Succeeded)
            {
                TempData["SuccessMessage"] = new[] {OtherMessages.UserDeletedSuccessfully};

                return Json(new
                {
                    success = true
                });
            }

            return Json(new
            {
                success = false,
                messages = new[] {OtherMessages.SomethingWrong}
            });
        }
        
        [HttpGet("{action:slugify}")]
        public async Task<IActionResult> YourProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = Mapper.MapToModel(user);
            model.Role = Utilities.FromRole((await _userManager.GetRolesAsync(user)).FirstOrDefault());

            return View(model);
        }

        [HttpPatch("{action:slugify}")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.ChangePasswordAsync(await _userManager.GetUserAsync(User), changePasswordModel.Password, changePasswordModel.NewPassword);
                
                if(result.Succeeded)
                    return Json(new
                    {
                        success = true,
                        messages = new[] {OtherMessages.PasswordChangedSuccessfully}
                    });

                ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotChangePassword);
            }
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpPatch("{action:slugify}")]
        public async Task<IActionResult> ChangeEmail(EmailModel emailModel)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(emailModel.Email) is var candidate && candidate != null)
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.EmailAlreadyInUse}
                    });

                var user = await _userManager.GetUserAsync(User);

                if (await _parameterManager.GetValue<bool>(ParameterTypes.EmailSenderEnabled))
                {
                    var token = (await _userManager.GenerateChangeEmailTokenAsync(user, emailModel.Email)).EncodeToBase64();
                    var siteName = await _parameterManager.GetValue<string>(ParameterTypes.SiteName);
                    var siteEmail = await _parameterManager.GetValue<string>(ParameterTypes.SiteEmail);

                    var message = string.Format(Emails.ChangeEmailBody, user.UserName, siteName, Url.Action("ChangeEmail", "UsersManager", new {userId = user.Id, email = emailModel.Email, token = "replace"}, Request.Scheme).Replace("replace", token));

                    _emailSender.SendEmail(siteEmail, emailModel.Email, string.Format(Emails.ChangeEmailSubject, siteName), message);

                    return Json(new
                    {
                        success = true,
                        messages = new[] {OtherMessages.ChangeEmailSent}
                    });
                }
                else
                {
                    user.Email = emailModel.Email;
                    user.EmailConfirmed = true;
                    
                    var result = await _userManager.UpdateAsync(user);
                    
                    if(result.Succeeded)
                        return Json(new
                        {
                            success = true,
                            messages = new[] {OtherMessages.EmailChangedSuccessfully}
                        });   
                    
                    ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotChangeEmail);
                }
            }
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        private async Task SendAccountCreationEmail(IdentityUser<Guid> user)
        {
            var token = (await _userManager.GeneratePasswordResetTokenAsync(user)).EncodeToBase64();
            var siteName = await _parameterManager.GetValue<string>(ParameterTypes.SiteName);
            var siteEmail = await _parameterManager.GetValue<string>(ParameterTypes.SiteEmail);

            var message = string.Format(Emails.AccountCreationEmailBody, user.UserName, siteName, Url.Action("ResetPassword", "SignIn", new {userId = user.Id, token = "replace" }, Request.Scheme).Replace("replace", token)); 
            
            _emailSender.SendEmail(siteEmail, user.Email, string.Format(Emails.AccountCreationEmailSubject, siteName), message);
        }
    }
}