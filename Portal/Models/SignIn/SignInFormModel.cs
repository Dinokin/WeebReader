using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Portal.Models.SignIn
{
    public class SignInFormModel
    {
        [Required(ErrorMessage = "A username is required to sign in.")]
        [MinLength(3, ErrorMessage = "A username must have at least 3 characters.")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "A password is required to sign in.")]
        [MinLength(8, ErrorMessage = "A password must have at least 8 characters.")]
        public string Password { get; set; }
    }
}