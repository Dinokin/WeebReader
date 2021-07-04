using System.ComponentModel.DataAnnotations;
using WeebReader.Web.API.Models.Request.Authentication;

namespace WeebReader.Web.API.Models.Request.Installer
{
    public class InstallRequestModel : AuthenticationRequestModel
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "ValidEmailRequired")]
        public string Email { get; set; } = string.Empty;
    }
}