using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Models.Models.UserManager
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "The current password is necessary to proceed with the reset.")]
        [MinLength(8, ErrorMessage = "A password must have at least 8 characters.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "A new password is necessary to proceed with the reset.")]
        [MinLength(8, ErrorMessage = "The password must have at least 8 characters.")]
        public string NewPassword { get; set; }
    }
}