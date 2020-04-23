﻿using System.ComponentModel.DataAnnotations;
using WeebReader.Data.Entities;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Controllers.ParametersManager
{
    public class GeneralParametersModel
    {
        [Parameter(Parameter.Types.SiteName)]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "SiteNameRequired")]
        public string SiteName { get; set; } = string.Empty;
        
        [Parameter(Parameter.Types.SiteDescription)]
        public string? SiteDescription { get; set; }
    }
}