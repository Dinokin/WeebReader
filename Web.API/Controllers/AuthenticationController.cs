using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WeebReader.Web.API.Models.Request.Authentication;
using WeebReader.Web.API.Models.Response;
using WeebReader.Web.API.Models.Response.Authentication;
using WeebReader.Web.API.Others.Utilities;

namespace WeebReader.Web.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;

        public AuthenticationController(SignInManager<IdentityUser<Guid>> signInManager) => _signInManager = signInManager;

        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthenticationRequestModel model)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(model.Username);

            if (user == null)
                return Unauthorized(new DefaultResponseMessage
                {
                    Message = Messages.InvalidCredentials
                });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);

            if (!result.Succeeded)
                return Unauthorized(new DefaultResponseMessage
                {
                    Message = result.IsLockedOut || result.IsNotAllowed ? Messages.NotAllowedToAuthenticate : Messages.InvalidCredentials
                });

            return Ok(new AuthenticationResponseModel
            {
                Token = BuildJwtToken()
            });
        }

        private string BuildJwtToken()
        {
            var key = new X509SecurityKey(Security.Certificate);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(Security.Issuer, Security.Audience, HttpContext.User.Claims, null, DateTime.Now.AddMinutes(30), credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}