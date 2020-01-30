using System;
using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Models.Models.Shared;

namespace WeebReader.Web.Models.Models.SignIn
{
    public class ChangeEmailModel : EmailModel
    {
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG003")]
        public Guid UserId { get; set; }
        [Required(ErrorMessageResourceType = typeof(ModelMessages), ErrorMessageResourceName = "MSG004")]
        public string Token { get; set; }
    }
}