using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Models.Models.SignIn
{
    public class SignInModel
    {
        [Required(ErrorMessage = "An username is required to sign in.")]
        [MinLength(3, ErrorMessage = "An username must have at least 3 characters.")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "A password is required to sign in.")]
        [MinLength(8, ErrorMessage = "A password must have at least 8 characters.")]
        public string Password { get; set; }
    }
}