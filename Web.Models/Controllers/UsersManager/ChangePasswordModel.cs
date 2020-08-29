using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.Controllers.UsersManager
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PasswordRequired")]
        [MinLength(8, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "MinimumPasswordLength")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "NewPasswordRequired")]
        [MinLength(8, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "MinimumPasswordLength")]
        public string NewPassword { get; set; } = string.Empty;
    }
}