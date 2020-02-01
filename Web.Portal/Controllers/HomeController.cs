using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts;
using WeebReader.Web.Models.Models.UserManager;

namespace WeebReader.Web.Portal.Controllers
{
    [Route("{action=Index}")]
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public HomeController(UserManager<IdentityUser<Guid>> userManager) => _userManager = userManager;

        [HttpGet]
        public IActionResult Index()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> Install() => await AllowInstaller() ? (IActionResult) RedirectToAction("Index") : View();

        [HttpPost]
        public async Task<IActionResult> Install(CreateUserModel createUserModel)
        {
            if (!await AllowInstaller())
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {PortalMessages.MSG059}
                });

            if (TryValidateModel(createUserModel))
            {
                var user = new IdentityUser<Guid>
                {
                    UserName = createUserModel.Username,
                    Email = createUserModel.Email
                };
                
                var userResult = await _userManager.CreateAsync(user, createUserModel.Password);

                if (userResult.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, ContextMessages.MSG001);

                    if (roleResult.Succeeded)
                    {
                        TempData["SuccessMessage"] = new[] {PortalMessages.MSG056};

                        return new JsonResult(new
                        {
                            success = true,
                            destination = Url.Action("Index", "SignIn")
                        });
                    }
                    
                    await _userManager.DeleteAsync(user);
                }

                ModelState.AddModelError("SomethingWrong", PortalMessages.MSG055);
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