using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeebReader.Data.Entities;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Controllers.ParametersManager
{
    public class EmailParametersModel : IValidatableObject
    {
        [Parameter(Parameter.Types.EmailSenderEnabled)]
        public bool EmailSenderEnabled { get; set; }
        
        [Parameter(Parameter.Types.SiteEmail)]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "ValidEmailRequiredSiteEmail")]
        public string? SiteEmail { get; set; }

        [Parameter(Parameter.Types.SmtpServer)]
        public string? SmtpServer { get; set; }
        
        [Parameter(Parameter.Types.SmtpServerPort)]
        [Range(0, ushort.MaxValue, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "SmtpServerPortOutOfRange")]
        public ushort? SmtpServerPort { get; set; }

        [Parameter(Parameter.Types.SmtpServerUser)]
        public string? SmtpServerUser { get; set; } = string.Empty;

        [Parameter(Parameter.Types.SmtpServerPassword)]
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