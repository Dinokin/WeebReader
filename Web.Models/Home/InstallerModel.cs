using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.Home
{
    public class InstallerModel
    {
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "UsernameRequired")]
        [MinLength(3, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "MinimumUsernameLength")]
        public string Username { get; set; }
        
        [MinLength(8, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "MinimumPasswordLength")]
        public string Password { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "ValidEmailRequired")]
        public string Email { get; set; }
    }
}