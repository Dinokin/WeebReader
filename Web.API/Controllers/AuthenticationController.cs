using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WeebReader.Web.API.Data.Contexts.Abstract;
using WeebReader.Web.API.Models.Request.Authentication;
using WeebReader.Web.API.Models.Response;
using WeebReader.Web.API.Models.Response.Authentication;
using WeebReader.Web.API.Services;
using WeebReader.Web.API.Utilities;

namespace WeebReader.Web.API.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : ApiController
    {
        private readonly BaseContext _context;
        private readonly ApiSignInManager<IdentityUser<Guid>> _apiSignInManager;

        public AuthenticationController(BaseContext context, ApiSignInManager<IdentityUser<Guid>> apiSignInManager)
        {
            _context = context;
            _apiSignInManager = apiSignInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthenticationRequestModel model)
        {
            var result = await _apiSignInManager.PasswordSignIn(model.Username, model.Password, true);

            if (result.IsLockedOut || result.IsNotAllowed)
                return Unauthorized(new DefaultResponseMessage
                {
                    Message = new[] {Messages.NotAllowedToAuthenticate}
                });

            if (!result.Succeeded)
                return Unauthorized(new DefaultResponseMessage
                {
                    Message = new[] {Messages.InvalidCredentials}
                });
            
            return Ok(new AuthenticationResponseModel
            {
                Token = BuildJwtToken()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Install(InstallRequestModel model)
        {
            var userManager = _apiSignInManager.UserManager;
            
            if (await userManager.Users.AnyAsync())
                return StatusCode(403, new DefaultResponseMessage
                {
                    Message = new[] {Messages.InvalidCredentials}
                });

            var user = Mapper.MapToEntity(model);
            user.EmailConfirmed = true;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var userResult = await userManager.CreateAsync(user, model.Password);

            if (userResult.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(user, Security.Roles.Administrator);

                if (roleResult.Succeeded)
                {
                    await transaction.CommitAsync();

                    return await Authenticate(model);
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

        private string BuildJwtToken()
        {
            var key = new X509SecurityKey(Security.Certificate);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new(HttpContext.User.Claims),
                Issuer = Security.Issuer,
                IssuedAt = DateTime.Now,
                Audience = Security.Audience,
                Expires = DateTime.Now.AddMinutes(60),
                NotBefore = DateTime.Now.AddMinutes(30),
                SigningCredentials = new(key, SecurityAlgorithms.RsaSha256Signature)
            };

            return new JwtSecurityTokenHandler().CreateEncodedJwt(tokenDescriptor);
        }
    }
}