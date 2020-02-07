using System;
using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Models.Models.UserManager
{
    public class UserModel
    {
        public Guid UserId { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG007")]
        [MinLength(3, ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG008")]
        public string Username { get; set; }
        
        [MinLength(8, ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG006")]
        public string Password { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG001")]
        [EmailAddress(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG002")]
        public string Email { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG011")]
        public string Role { get; set; }
    }
}