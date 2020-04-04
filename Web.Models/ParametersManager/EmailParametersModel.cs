using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeebReader.Data.Entities;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.ParametersManager
{
    public class EmailParametersModel : IValidatableObject
    {
        [Parameter(Parameter.Types.EmailEnabled)]
        public bool EmailEnabled { get; set; }

        [Parameter(Parameter.Types.SmtpServer)]
        public string? SmtpServer { get; set; }
        
        [Parameter(Parameter.Types.SmtpServerPort)]
        public ushort? SmtpServerPort { get; set; }

        [Parameter(Parameter.Types.SmtpServerUser)]
        public string? SmtpServerUser { get; set; }

        [Parameter(Parameter.Types.SmtpServerPassword)]
        public string? SmtpServerPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!EmailEnabled)
                return results;

            if (string.IsNullOrWhiteSpace(SmtpServer))
                results.Add(new ValidationResult(ValidationMessages.SmtpServerRequired, new []{ nameof(SmtpServer) }));

            if (SmtpServerPassword == null)
                results.Add(new ValidationResult(ValidationMessages.SmtpServerPortRequired, new []{ nameof(SmtpServerPort) }));

            if (string.IsNullOrWhiteSpace(SmtpServerUser))
                results.Add(new ValidationResult(ValidationMessages.SmtpServerUserRequired, new []{ nameof(SmtpServerUser) }));

            if (string.IsNullOrWhiteSpace(SmtpServerPassword))
                results.Add(new ValidationResult(ValidationMessages.SmtpServerPasswordRequired, new []{ nameof(SmtpServerPassword) }));

            return results;
        }
    }
}