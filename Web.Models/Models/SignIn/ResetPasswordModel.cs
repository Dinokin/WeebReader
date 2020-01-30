using System;
using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Models.Models.SignIn
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG003")]
        public Guid UserId { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG004")]
        public string Token { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG005")]
        [MinLength(8, ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG006")]
        public string NewPassword { get; set; }
    }
}