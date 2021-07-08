using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WeebReader.Web.API.Data.DAOs.Identity;
using WeebReader.Web.API.Models.Request.Users;
using WeebReader.Web.API.Settings;
using WeebReader.Web.API.Utilities;

namespace WeebReader.Web.API.Controllers
{
    [Route("[controller]/")]
    public class UsersController : ApiController
    {
        private readonly Configuration _configuration;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly UserDAO _userDAO;
        
        public UsersController(IOptions<Configuration> configuration, UserManager<IdentityUser<Guid>> userManager, UserDAO userDAO)
        {
            _configuration = configuration.Value;
            _userManager = userManager;
            _userDAO = userDAO;
        }

        [HttpGet("{page:int?}")]
        [Authorize(Roles = Identity.Roles.Administrator)]
        public async Task<IActionResult> Index(ushort page = 1)
        {
            var totalPages = Convert.ToUInt16(Math.Ceiling(await _userDAO.CountUsers() / (decimal) Constants.ItemsPerPage));
            var skip = Convert.ToUInt16(Constants.ItemsPerPage * (page - 1));
            var users = await _userDAO.GetUsersWithRole(skip, Constants.ItemsPerPage);

            return Ok(ModelMapper.MapToUsersListResponse(page, totalPages, users));
        }

        [HttpPost]
        [Authorize(Roles = Identity.Roles.Administrator)]
        public IActionResult Add(AddUserRequest model)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet("{userId:guid}")]
        [Authorize(Roles = Identity.Roles.Administrator)]
        public IActionResult Get(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{userId:guid}")]
        [Authorize(Roles = Identity.Roles.Administrator)]
        public IActionResult Edit(Guid userId, EditUserRequest model)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = Identity.Roles.Administrator)]
        public IActionResult Delete(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("[action]/{userId:guid}")]
        [Authorize(Roles = Identity.Roles.Administrator)]
        public IActionResult Block(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("[action]/{userId:guid}")]
        [Authorize(Roles = Identity.Roles.Administrator)]
        public IActionResult Unblock(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("[action]")]
        public IActionResult ResetPassword(RequestPasswordResetRequest model)
        {
            throw new NotImplementedException();
        }
        
        [HttpPatch("[action]")]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            throw new NotImplementedException();
        }

        [HttpGet("[action]")]
        [Authorize]
        public IActionResult Self()
        {
            throw new NotImplementedException();
        }
        
        [HttpPatch("[action]")]
        [Authorize]
        public IActionResult ChangePassword(ChangePasswordRequest model)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("[action]")]
        [Authorize]
        public IActionResult ChangeEmail(ChangeEmailRequest model)
        {
            throw new NotImplementedException();
        }
    }
}