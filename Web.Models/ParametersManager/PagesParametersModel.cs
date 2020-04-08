using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeebReader.Data.Entities;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.ParametersManager
{
    public class PagesParametersModel : IValidatableObject
    {
        [Parameter(Parameter.Types.PageAboutUsContent)]
        public string AboutUsContent { get; set; }

        [Parameter(Parameter.Types.PageAboutUsPatreonEnabled)]
        public bool PatreonEnabled { get; set; }
        
        [Parameter(Parameter.Types.PageAboutUsKofiEnabled)]
        public bool KofiEnabled { get; set; }
        
        [Parameter(Parameter.Types.PageAboutUsPatreonLink)]
        [Url(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PatreonLinkValidUrl")]
        public string? PatreonLink { get; set; }
        
        [Parameter(Parameter.Types.PageAboutUsPatreonNotice)]
        public string? PatreonNotice { get; set; }
        
        [Parameter(Parameter.Types.PageAboutUsKofiLink)]
        [Url(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "KofiLinkValidUrl")]
        public string? KofiLink { get; set; }
        
        [Parameter(Parameter.Types.PageAboutUsKofiNotice)]
        public string? KofiNotice { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            
            if (PatreonEnabled)
                if (string.IsNullOrWhiteSpace(PatreonLink))
                    results.Add(new ValidationResult(ValidationMessages.PatreonLinkRequired, new[] {nameof(PatreonLink)}));
                
            if (KofiEnabled)
                if (string.IsNullOrWhiteSpace(KofiLink))
                    results.Add(new ValidationResult(ValidationMessages.KoFiLinkRequired, new[] {nameof(KofiLink)}));

            return results;
        }
    }
}