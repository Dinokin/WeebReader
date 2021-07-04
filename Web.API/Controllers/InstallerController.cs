using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Web.API.Data.Contexts.Abstract;
using WeebReader.Web.API.Models.Request.Installer;
using WeebReader.Web.API.Models.Response;
using WeebReader.Web.API.Utilities;

namespace WeebReader.Web.API.Controllers
{
    [Route("[controller]/{action=Index}")]
    public class InstallerController : ApiController
    {
        private readonly BaseContext _context;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        
        public InstallerController(BaseContext context, UserManager<IdentityUser<Guid>> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index() => Ok(new DefaultResponseMessage
        {
            Message = new[] {await IsInstalled() ? Messages.Installed : Messages.NotInstalled}
        });
        
        [HttpPost]
        public async Task<IActionResult> Install(InstallRequestModel model)
        {
            if (await IsInstalled())
                return StatusCode(403, new DefaultResponseMessage
                {
                    Message = new[] {Messages.CannotProceedAlreadyInstalled}
                });

            var user = Mapper.MapToEntity(model);
            user.EmailConfirmed = true;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var userResult = await _userManager.CreateAsync(user, model.Password);

            if (userResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, Security.Roles.Administrator);

                if (roleResult.Succeeded)
                {
                    await transaction.CommitAsync();

                    return Ok();
                }

                await transaction.RollbackAsync();
                    
                return StatusCode(500, new DefaultResponseMessage
                {
                    Message = roleResult.Errors.Select(error => error.Description)
                });
            }

            return StatusCode(500, new DefaultResponseMessage
            {
                Message = userResult.Errors.Select(error => error.Description)
            });
        }

        private Task<bool> IsInstalled() => _userManager.Users.AnyAsync();
    }
}