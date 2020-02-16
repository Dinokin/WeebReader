using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Web.Localization;
using WeebReader.Web.Localization.Utilities;
using WeebReader.Web.Models.Models.Home;

namespace WeebReader.Web.Portal.Controllers
{
    [Route("{action=Index}")]
    public class HomeController : Controller
    {
        private readonly BaseContext _context;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public HomeController(BaseContext context, UserManager<IdentityUser<Guid>> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> Install() => await AllowInstaller() ? View() : (IActionResult) RedirectToAction("Index");

        [HttpPost]
        public async Task<IActionResult> Install(CreateAdminUserModel userModel)
        {
            if (!await AllowInstaller())
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.CannotProceedAlreadyInstalled}
                });

            if (TryValidateModel(userModel))
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                
                var user = new IdentityUser<Guid>
                {
                    UserName = userModel.Username,
                    Email = userModel.Email
                };
                
                var userResult = await _userManager.CreateAsync(user, userModel.Password);

                if (userResult.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, RoleTranslator.Administrator);

                    if (roleResult.Succeeded)
                    {
                        await transaction.CommitAsync();
                        
                        TempData["SuccessMessage"] = new[] {OtherMessages.InstalledSuccessfully};

                        return new JsonResult(new
                        {
                            success = true,
                            destination = Url.Action("Index", "SignIn")
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

        private async Task<bool> AllowInstaller() => !await _userManager.Users.AnyAsync();
    }
}