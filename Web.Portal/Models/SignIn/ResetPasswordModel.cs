using System;
using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Portal.Models.SignIn
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "An user ID is necessary to reset a password.")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "A token is necessary to reset a password.")]
        public string Token { get; set; }
        [Required(ErrorMessage = "A new password is necessary to proceed with the reset.")]
        [MinLength(8, ErrorMessage = "The password must have at least 8 characters.")]
        public string NewPassword { get; set; }
    }
}