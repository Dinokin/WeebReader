using System;
using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.BlogManager
{
    public class PostModel
    {
        public Guid? PostId { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PostMustHaveTitle")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PostTitleMaxLength")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PostMustHaveContent")]
        public string Content { get; set; } = string.Empty;
        
        public DateTime? ReleaseDate { get; set; }
        
        public bool Visible { get; set; }
    }
}