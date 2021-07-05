using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WeebReader.Web.API.Models.Request.Authentication;
using WeebReader.Web.API.Models.Response;
using WeebReader.Web.API.Models.Response.Authentication;
using WeebReader.Web.API.Services;
using WeebReader.Web.API.Utilities;

namespace WeebReader.Web.API.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthenticationController : ApiController
    {
        private readonly ApiSignInManager<IdentityUser<Guid>> _apiSignInManager;

        public AuthenticationController(ApiSignInManager<IdentityUser<Guid>> apiSignInManager) => _apiSignInManager = apiSignInManager;

        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthenticationRequestModel model)
        {
            var result = await _apiSignInManager.PasswordSignIn(model.Username, model.Password);

            if (result.IsLockedOut || result.IsNotAllowed)
                return Unauthorized(new DefaultResponseMessage
                {
                    Messages = new[] {Messages.NotAllowedToAuthenticate}
                });

            if (!result.Succeeded)
                return Unauthorized(new DefaultResponseMessage
                {
                    Messages = new[] {Messages.InvalidCredentials}
                });
            
            return Ok(new AuthenticationResponseModel
            {
                Token = BuildJwtToken()
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Refresh()
        {
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(HttpContext.User.Claims.Single(claim => claim.Type == "exp").Value));
            var timeRemaining = expirationDate - DateTimeOffset.Now;

            if (timeRemaining > TimeSpan.FromMinutes(5))
            {
                return BadRequest(new DefaultResponseMessage
                {
                    Messages = new[] {Messages.TokenStillValid}
                });
            }

            var result = await _apiSignInManager.RefreshSignInAsync(HttpContext.User);

            if (!result.Succeeded)
                return Unauthorized(new DefaultResponseMessage
                {
                    Messages = new[] {Messages.NotAllowedToAuthenticate}
                });
            
            return Ok(new AuthenticationResponseModel
            {
                Token = BuildJwtToken()
            });
        }

        private string BuildJwtToken()
        {
            var key = new X509SecurityKey(Security.Certificate);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new(HttpContext.User.Claims),
                Issuer = Security.Issuer,
                Audience = Security.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = new(key, SecurityAlgorithms.RsaSha256Signature)
            };

            return new JwtSecurityTokenHandler().CreateEncodedJwt(tokenDescriptor);
        }
    }
}