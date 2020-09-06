using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Controllers.ParametersManager
{
    public class ContactParametersModel : IValidatableObject
    {
        [Parameter(ParameterTypes.ContactEmailEnabled)]
        public bool EmailEnabled { get; set; }

        [Parameter(ParameterTypes.ContactEmailRecaptchaEnabled)]
        public bool RecaptchaEnabled { get; set; }

        [Parameter(ParameterTypes.ContactEmailRecaptchaClientKey)]
        public string? RecaptchaClientKey { get; set; }

        [Parameter(ParameterTypes.ContactEmailRecaptchaServerKey)]
        public string? RecaptchaServerKey { get; set; }

        [Parameter(ParameterTypes.ContactDiscordEnabled)]
        public bool ContactDiscordEnabled { get; set; }

        [Parameter(ParameterTypes.ContactDiscordLink)]
        [Url(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "DiscordLinkValidUrl")]
        public string? DiscordLink { get; set; }

        [Parameter(ParameterTypes.ContactDiscordNotice)]
        public string? DiscordNotice { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (RecaptchaEnabled)
            {
                if (string.IsNullOrWhiteSpace(RecaptchaClientKey))
                    results.Add(new ValidationResult(ValidationMessages.ReCaptchaClientKeyRequired, new[] {nameof(RecaptchaClientKey)}));

                if (string.IsNullOrWhiteSpace(RecaptchaServerKey))
                    results.Add(new ValidationResult(ValidationMessages.ReCaptchaServerKeyRequired, new[] {nameof(RecaptchaServerKey)}));
            }

            if (ContactDiscordEnabled)
            {
                if (string.IsNullOrWhiteSpace(DiscordLink))
                    results.Add(new ValidationResult(ValidationMessages.DiscordLinkRequired, new[] {nameof(DiscordLink)}));
            }
            
            return results;
        }
    }
}