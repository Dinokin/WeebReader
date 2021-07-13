using System.ComponentModel.DataAnnotations;
using WeebReader.Web.API.Models.Request.Authentication;

namespace WeebReader.Web.API.Models.Request.Installer
{
    public class InstallRequest : AuthenticationRequest
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "ValidEmailRequired")]
        public string Email { get; init; } = string.Empty;
    }
}