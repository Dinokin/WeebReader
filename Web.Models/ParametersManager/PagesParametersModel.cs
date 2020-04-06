using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeebReader.Data.Entities;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.ParametersManager
{
    public class PagesParametersModel : IValidatableObject
    {
        [Parameter(Parameter.Types.PageBlogEnabled)]
        public bool BlogEnabled { get; set; }
        
        [Parameter(Parameter.Types.PageSupportUsEnabled)]
        public bool SupportUsEnabled { get; set; }
        
        [Parameter(Parameter.Types.PageSupportUsPatreonEnabled)]
        public bool PatreonEnabled { get; set; }
        
        [Parameter(Parameter.Types.PageSupportUsKofiEnabled)]
        public bool KofiEnabled { get; set; }
        
        [Parameter(Parameter.Types.PageSupportUsPatreonLink)]
        [Url(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PatreonLinkValidUrl")]
        public string? PatreonLink { get; set; }
        
        [Parameter(Parameter.Types.PageSupportUsPatreonMessage)]
        public string? PatreonMessage { get; set; }
        
        [Parameter(Parameter.Types.PageSupportUsKofiLink)]
        [Url(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "KofiLinkValidUrl")]
        public string? KofiLink { get; set; }
        
        [Parameter(Parameter.Types.PageSupportUsKofiMessage)]
        public string? KofiMessage { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (SupportUsEnabled)
            {
                if (PatreonEnabled)
                    if (string.IsNullOrWhiteSpace(PatreonLink))
                        results.Add(new ValidationResult(ValidationMessages.ReCaptchaClientKeyRequired, new[] {nameof(PatreonLink)}));

                if (KofiEnabled)
                    if (string.IsNullOrWhiteSpace(KofiLink))
                        results.Add(new ValidationResult(ValidationMessages.ReCaptchaClientKeyRequired, new[] {nameof(KofiLink)}));
            }

            return results;
        }
    }
}