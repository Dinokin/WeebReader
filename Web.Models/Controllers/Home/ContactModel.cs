using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.Controllers.Home
{
    public class ContactModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "NickNameRequired")]
        public string Nickname { get; set; } = string.Empty;
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "ValidEmailRequired")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "MessageRequired")]
        public string Message { get; set; } = string.Empty;
        
        public string? ReCaptchaResponse { get; set; }
    }
}