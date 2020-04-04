using WeebReader.Data.Entities;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.ParametersManager
{
    public class EmailParametersModel
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
    }
}