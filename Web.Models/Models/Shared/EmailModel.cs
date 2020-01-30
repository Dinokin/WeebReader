using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Models.Models.Shared
{
    public class EmailModel
    {
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG001")]
        [EmailAddress(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG002")]
        public string Email { get; set; }
    }
}