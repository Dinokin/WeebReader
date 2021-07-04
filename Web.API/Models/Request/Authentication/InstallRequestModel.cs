using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.API.Models.Request.Authentication
{
    public class InstallRequestModel : AuthenticationRequestModel
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "ValidEmailRequired")]
        public string Email { get; set; } = string.Empty;
    }
}