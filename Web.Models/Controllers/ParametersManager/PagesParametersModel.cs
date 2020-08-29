using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using WeebReader.Data.Entities;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Controllers.ParametersManager
{
    public class PagesParametersModel : IValidatableObject
    {
        [Parameter(Parameter.Types.PageBlogEnabled)]
        public bool BlogEnabled { get; set; }

        [Parameter(Parameter.Types.PageAboutEnabled)]
        public bool AboutEnabled { get; set; }
        
        [Parameter(Parameter.Types.PageAboutContent)]
        public string? AboutContent { get; set; }

        [Parameter(Parameter.Types.PageAboutPatreonEnabled)]
        public bool PatreonEnabled { get; set; }
        
        [Parameter(Parameter.Types.PageAboutKofiEnabled)]
        public bool KofiEnabled { get; set; }
        
        [Parameter(Parameter.Types.PageAboutPatreonLink)]
        [Url(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PatreonLinkValidUrl")]
        public string? PatreonLink { get; set; }
        
        [Parameter(Parameter.Types.PageAboutPatreonNotice)]
        public string? PatreonNotice { get; set; }
        
        [Parameter(Parameter.Types.PageAboutKofiLink)]
        [Url(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "KofiLinkValidUrl")]
        public string? KofiLink { get; set; }
        
        [Parameter(Parameter.Types.PageAboutKofiNotice)]
        public string? KofiNotice { get; set; }
        
        [Parameter(Parameter.Types.PageDisqusEnabled)]
        public bool DisqusEnabled { get; set; }
        
        [Parameter(Parameter.Types.PageDisqusShortname)]
        public string? DisqusShortname { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (AboutEnabled)
                if (string.IsNullOrWhiteSpace(Regex.Replace(AboutContent ?? string.Empty, "<.*?>", string.Empty)))
                    results.Add(new ValidationResult(ValidationMessages.AboutPageContentRequired, new []{nameof(AboutContent)}));
                    
            if (PatreonEnabled)
                if (string.IsNullOrWhiteSpace(PatreonLink))
                    results.Add(new ValidationResult(ValidationMessages.PatreonLinkRequired, new[] {nameof(PatreonLink)}));
                
            if (KofiEnabled)
                if (string.IsNullOrWhiteSpace(KofiLink))
                    results.Add(new ValidationResult(ValidationMessages.KoFiLinkRequired, new[] {nameof(KofiLink)}));
            
            if (DisqusEnabled)
                if (string.IsNullOrWhiteSpace(DisqusShortname))
                    results.Add(new ValidationResult(ValidationMessages.DisqusShortnameRequired, new[] {nameof(DisqusShortname)}));

            return results;
        }
    }
}