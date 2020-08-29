using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.Controllers.SignIn
{
    public class EmailModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "ValidEmailRequired")]
        public string Email { get; set; } = string.Empty;
    }
}