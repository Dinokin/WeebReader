using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        
        public UsersController(Configuration configuration, UserManager<IdentityUser<Guid>> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpGet("{page:int?}")]
        [Authorize(Roles = Security.Roles.Administrator)]
        public IActionResult Index(ushort? page)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Authorize(Roles = Security.Roles.Administrator)]
        public IActionResult Add(AddUserRequest model)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet("{userId:guid}")]
        [Authorize(Roles = Security.Roles.Administrator)]
        public IActionResult Get(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{userId:guid}")]
        [Authorize(Roles = Security.Roles.Administrator)]
        public IActionResult Edit(Guid userId, EditUserRequest model)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = Security.Roles.Administrator)]
        public IActionResult Delete(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("[action]/{userId:guid}")]
        [Authorize(Roles = Security.Roles.Administrator)]
        public IActionResult Block(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("[action]/{userId:guid}")]
        [Authorize(Roles = Security.Roles.Administrator)]
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