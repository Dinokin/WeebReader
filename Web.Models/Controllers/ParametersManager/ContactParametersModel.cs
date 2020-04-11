using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeebReader.Data.Entities;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Controllers.ParametersManager
{
    public class ContactParametersModel : IValidatableObject
    {
        [Parameter(Parameter.Types.ContactEmailEnabled)]
        public bool EmailEnabled { get; set; }

        [Parameter(Parameter.Types.ContactEmailRecaptchaEnabled)]
        public bool RecaptchaEnabled { get; set; }

        [Parameter(Parameter.Types.ContactEmailRecaptchaClientKey)]
        public string? RecaptchaClientKey { get; set; }

        [Parameter(Parameter.Types.ContactEmailRecaptchaServerKey)]
        public string? RecaptchaServerKey { get; set; }

        [Parameter(Parameter.Types.ContactDiscordEnabled)]
        public bool ContactDiscordEnabled { get; set; }

        [Parameter(Parameter.Types.ContactDiscordLink)]
        [Url(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "DiscordLinkValidUrl")]
        public string? DiscordLink { get; set; }

        [Parameter(Parameter.Types.ContactDiscordNotice)]
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