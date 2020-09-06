using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Controllers.ParametersManager
{
    public class EmailParametersModel : IValidatableObject
    {
        [Parameter(ParameterTypes.EmailSenderEnabled)]
        public bool EmailSenderEnabled { get; set; }
        
        [Parameter(ParameterTypes.SiteEmail)]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "ValidEmailRequiredSiteEmail")]
        public string? SiteEmail { get; set; }

        [Parameter(ParameterTypes.SmtpServer)]
        public string? SmtpServer { get; set; }
        
        [Parameter(ParameterTypes.SmtpServerPort)]
        [Range(0, ushort.MaxValue, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "SmtpServerPortOutOfRange")]
        public ushort? SmtpServerPort { get; set; }

        [Parameter(ParameterTypes.SmtpServerUser)]
        public string? SmtpServerUser { get; set; } = string.Empty;

        [Parameter(ParameterTypes.SmtpServerPassword)]
        public string? SmtpServerPassword { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!EmailSenderEnabled)
                return results;
            
            if (string.IsNullOrWhiteSpace(SiteEmail))
                results.Add(new ValidationResult(ValidationMessages.SiteEmailRequired, new []{ nameof(SiteEmail) }));

            if (string.IsNullOrWhiteSpace(SmtpServer))
                results.Add(new ValidationResult(ValidationMessages.SmtpServerRequired, new []{ nameof(SmtpServer) }));

            if (SmtpServerPort == null)
                results.Add(new ValidationResult(ValidationMessages.SmtpServerPortRequired, new []{ nameof(SmtpServerPort) }));

            return results;
        }
    }
}