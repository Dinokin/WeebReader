using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WeebReader.Web.API.Data.Contexts.Abstract;
using WeebReader.Web.API.Data.DAOs.Identity;
using WeebReader.Web.API.Models.Request.Users;
using WeebReader.Web.API.Services;
using WeebReader.Web.API.Settings;
using WeebReader.Web.API.Utilities;

namespace WeebReader.Web.API.Controllers
{
    [Route("[controller]/")]
    public class UsersController : ApiController
    {
        private readonly Configuration _configuration;
        private readonly BaseContext _baseContext;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly UserDAO _userDAO;
        private readonly EmailDispatcher _emailDispatcher;

        public UsersController(IOptions<Configuration> configuration, BaseContext baseContext, UserManager<IdentityUser<Guid>> userManager, UserDAO userDAO, EmailDispatcher emailDispatcher)
        {
            _configuration = configuration.Value;
            _baseContext = baseContext;
            _userManager = userManager;
            _userDAO = userDAO;
            _emailDispatcher = emailDispatcher;
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
        public async Task<IActionResult> Add(AddUserRequest model)
        {
            var user = EntityMapper.MapToUser(model);
            user.EmailConfirmed = !_configuration.Email.Enabled;

            await using var transaction = await _baseContext.Database.BeginTransactionAsync();

            if (!_configuration.Email.Enabled && string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(ModelMapper.MapToDefaultResponse(Messages.PasswordRequired));

            var userResult = string.IsNullOrWhiteSpace(model.Password) ? await _userManager.CreateAsync(user) : await _userManager.CreateAsync(user, model.Password);

            if (!userResult.Succeeded)
                return StatusCode(500, ModelMapper.MapToDefaultResponse(userResult.Errors.Select(error => error.Description)));

            if (model.Role != Identity.Roles.None)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, model.Role);

                if (!roleResult.Succeeded)
                    return StatusCode(500, ModelMapper.MapToDefaultResponse(roleResult.Errors.Select(error => error.Description)));
            }

            await transaction.CommitAsync();

            return Ok(ModelMapper.MapToDefaultResponse(Messages.UserCreatedSuccessfully));
        }
        
        [HttpGet("{userId:guid}")]
        [Authorize(Roles = Identity.Roles.Administrator)]
        public async Task<IActionResult> Get(Guid userId)
        {
            var (user, role) = await _userDAO.GetUserByIdWithRole(userId);

            if (user == null)
                return NotFound(ModelMapper.MapToDefaultResponse(Messages.UserNotFound));

            return Ok(ModelMapper.MapToUserResponse(user, role));
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

        private async Task SendAccountCreationEmail(IdentityUser<Guid> user)
        {
            throw new NotImplementedException();
        }
    }
}