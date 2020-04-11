using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Controllers.TitlesManager
{
    public class TitleModel
    {
        public Guid? TitleId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TitleMustHaveName")]
        [StringLength(200, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TitleNameMaxLength")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TitleOriginalNameMaxLength")]
        public string? OriginalName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TitleMustHaveAuthor")]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TitleAuthorMaxLength")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TitleMustHaveArtist")]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TitleArtistMaxLength")]
        public string Artist { get; set; } = string.Empty;
        
        public string? Synopsis { get; set; }
        
        public Title.Statuses Status { get; set; }
        
        public bool Visible { get; set; }

        public string Tags { get; set; } = string.Empty;
        
        [AllowedFormats(".png,.jpg,.jpeg", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "InvalidCoverFormat")]
        public IFormFile? Cover { get; set; }
    }
}