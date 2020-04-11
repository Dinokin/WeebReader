using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.Controllers.Home
{
    public class SignInModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "UsernameRequired")]
        [MinLength(3, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "MinimumUsernameLength")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PasswordRequired")]
        [MinLength(8, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "MinimumPasswordLength")]
        public string Password { get; set; } = string.Empty;
    }
}