using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Models.Models.SignIn
{
    public class SignInModel
    {
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG007")]
        [MinLength(3, ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG008")]
        public string Username { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG009")]
        [MinLength(8, ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG006")]
        public string Password { get; set; }
    }
}