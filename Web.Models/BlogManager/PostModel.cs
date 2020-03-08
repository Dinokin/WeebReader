using System;
using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.BlogManager
{
    public class PostModel
    {
        public Guid? PostId { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PostMustHaveTitle")]
        public string Title { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "PostMustHaveContent")]
        public string Content { get; set; }
        
        public DateTime? Date { get; set; }
        
        public bool Visible { get; set; }
    }
}