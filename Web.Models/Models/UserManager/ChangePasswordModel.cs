using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Models.Models.UserManager
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG010")]
        [MinLength(8, ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG006")]
        public string Password { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG005")]
        [MinLength(8, ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG006")]
        public string NewPassword { get; set; }
    }
}