using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Controllers.Others;
using WeebReader.Web.Models.Others;
using Utilities = WeebReader.Web.Localization.Others.Utilities;

namespace WeebReader.Web.Portal.Controllers
{
    public class OthersController : Controller
    {
        private readonly BaseContext _context;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public OthersController(BaseContext context, UserManager<IdentityUser<Guid>> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Error() => StatusCode(500);

        public IActionResult Denied() => Forbid();

        [HttpGet("{action}")]
        public async Task<IActionResult> Install() => await AllowInstaller() ? View() : RedirectToAction("Index", "Content");

        [HttpPost("{action}")]
        public async Task<IActionResult> Install(InstallerModel installerModel)
        {
            if (!await AllowInstaller())
                return Json(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.CannotProceedAlreadyInstalled}
                });
            
            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                var user = Mapper.MapToEntity(installerModel);
                user.EmailConfirmed = true;
                
                var userResult = await _userManager.CreateAsync(user, installerModel.Password);

                if (userResult.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, Utilities.Roles.Administrator);

                    if (roleResult.Succeeded)
                    {
                        await transaction.CommitAsync();
                        
                        TempData["SuccessMessage"] = new[] {OtherMessages.InstalledSuccessfully};

                        return Json(new
                        {
                            success = true,
                            destination = Url.Action("SignIn", "SignIn")
                        });
                    }
                }

                await transaction.RollbackAsync();
                ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
            }
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
        
        private async Task<bool> AllowInstaller() => !await _userManager.Users.AnyAsync();
    }
}