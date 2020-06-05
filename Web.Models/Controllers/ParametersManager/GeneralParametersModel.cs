using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using WeebReader.Data.Entities;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Controllers.ParametersManager
{
    public class GeneralParametersModel : IValidatableObject
    {
        [Parameter(Parameter.Types.SiteName)]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "SiteNameRequired")]
        public string SiteName { get; set; } = string.Empty;
        
        [Parameter(Parameter.Types.SiteDescription)]
        public string? SiteDescription { get; set; }
        
        [Parameter(Parameter.Types.SiteGoogleAnalyticsEnabled)]
        public bool GoogleAnalyticsEnabled { get; set; }
        
        [Parameter(Parameter.Types.SiteGoogleAnalyticsCode)]
        public string? GoogleAnalyticsCode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (GoogleAnalyticsEnabled)
                if (string.IsNullOrWhiteSpace(GoogleAnalyticsCode))
                    results.Add(new ValidationResult(ValidationMessages.GoogleAnalyticsCodeRequired, new[] {nameof(GoogleAnalyticsCode)}));
                else if (!Regex.IsMatch(GoogleAnalyticsCode, @"^UA-[0-9]+-?[0-9]*$"))
                    results.Add(new ValidationResult(ValidationMessages.InvalidGoogleAnalyticsCode, new[] {nameof(GoogleAnalyticsCode)}));

            return results;
        }
    }
}