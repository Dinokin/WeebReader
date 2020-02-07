using System;
using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Models.Models.SignIn
{
    public class ChangeEmailModel
    {
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG003")]
        public Guid UserId { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG004")]
        public string Token { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG001")]
        [EmailAddress(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG002")]
        public string Email { get; set; }
    }
}