using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Models.Models.SignIn;

namespace WeebReader.Web.Models.Models.UserManager
{
    public class CreateUserModel : SignInModel
    {
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG001")]
        [EmailAddress(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG002")]
        public string Email { get; set; }
    }
}