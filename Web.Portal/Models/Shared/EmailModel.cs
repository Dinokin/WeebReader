using System.ComponentModel.DataAnnotations;

namespace WeebReader.Web.Portal.Models.Shared
{
    public class EmailModel
    {
        [Required(ErrorMessage = "An e-mail address is required.")]
        [EmailAddress(ErrorMessage = "A valid e-mail address is required.")]
        public string Email { get; set; }
    }
}