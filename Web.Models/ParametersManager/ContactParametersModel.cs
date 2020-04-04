using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.ParametersManager
{
    public class ContactParametersModel : IValidatableObject
    {
        public bool ContactEmailEnabled { get; set; }
        
        public bool ContactEmailRecaptchaEnabled { get; set; }
        
        public string ContactEmailRecaptchaClientKey { get; set; }
        
        public string ContactEmailRecaptchaServerKey { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!ContactEmailRecaptchaEnabled)
                return results;

            if (string.IsNullOrWhiteSpace(ContactEmailRecaptchaClientKey))
                results.Add(new ValidationResult(ValidationMessages.ReCaptchaClientKeyRequired, new []{ nameof(ContactEmailRecaptchaClientKey) }));

            if (string.IsNullOrWhiteSpace(ContactEmailRecaptchaServerKey))
                results.Add(new ValidationResult(ValidationMessages.ReCaptchaServerKeyRequired, new []{ nameof(ContactEmailRecaptchaServerKey) }));

            return results;
        }
    }
}