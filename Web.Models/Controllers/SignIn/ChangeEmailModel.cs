using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.Controllers.SignIn
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class ChangeEmailModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "UserIdRequired")]
        public Guid UserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TokenRequired")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "ValidEmailRequired")]
        public string Email { get; set; } = string.Empty;
    }
}