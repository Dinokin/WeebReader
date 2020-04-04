using System.ComponentModel.DataAnnotations;
using WeebReader.Data.Entities;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.ParametersManager
{
    public class GeneralParametersModel
    {
        [Parameter(Parameter.Types.SiteName)]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "SiteNameRequired")]
        public string SiteName { get; set; } = string.Empty;
        
        [Parameter(Parameter.Types.SiteDescription)]
        public string? SiteDescription { get; set; }

        [Parameter(Parameter.Types.SiteAddress)]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "SiteAddressRequired")]
        public string SiteAddress { get; set; } = string.Empty;
    }
}