using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WeebReader.Web.API.Models.Request.Authentication;
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
        public async Task<IActionResult> Authenticate(AuthenticationRequest model)
        {
            if (_apiSignInManager.IsSignedIn(User))
                return Unauthorized(ModelMapper.MapToDefaultResponse(Messages.AlreadyAuthenticated));

            var signInResult = await _apiSignInManager.PasswordSignIn(model.Username, model.Password);

            if (signInResult.IsLockedOut || signInResult.IsNotAllowed)
                return Unauthorized(ModelMapper.MapToDefaultResponse(Messages.NotAllowedToAuthenticate));

            if (!signInResult.Succeeded)
                return Unauthorized(ModelMapper.MapToDefaultResponse(Messages.InvalidCredentials));
            
            return Ok(ModelMapper.MapToAuthenticationResponse(BuildJwtToken()));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Refresh()
        {
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(User.Claims.Single(claim => claim.Type == "exp").Value));
            var timeRemaining = expirationDate - DateTimeOffset.Now;

            if (timeRemaining > TimeSpan.FromMinutes(5))
                return BadRequest(ModelMapper.MapToDefaultResponse(Messages.TokenStillValid));

            var refreshResult = await _apiSignInManager.RefreshSignInAsync(User);

            if (!refreshResult.Succeeded)
                return Unauthorized(ModelMapper.MapToDefaultResponse(Messages.NotAllowedToAuthenticate));
            
            return Ok(ModelMapper.MapToAuthenticationResponse(BuildJwtToken()));
        }

        private string BuildJwtToken()
        {
            var key = new X509SecurityKey(Security.Certificate);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new(User.Claims),
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