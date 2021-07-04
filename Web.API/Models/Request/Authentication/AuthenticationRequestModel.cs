using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.API.Models.Request.Authentication
{
    public class AuthenticationRequestModel
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "UsernameRequired")]
        [MinLength(1, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MinimumUsernameLength")]
        public string Username { get; init; } = string.Empty;
        
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "PasswordRequired")]
        [MinLength(8, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MinimumPasswordLength")]
        public string Password { get; init; } = string.Empty;
    }
}