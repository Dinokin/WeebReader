using System;
using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.ChaptersManager
{
    public class ChapterModel
    {
        public Guid? ChapterId { get; set; }
        
        public ushort? Volume { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "ChapterMustHaveNumber")]
        public decimal Number { get; set; }
        
        [StringLength(200, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TitleNameMaxLength")]
        public string? Name { get; set; }
        
        public DateTime? ReleaseDate { get; set; }
        
        public bool Visible { get; set; }
        
        public Guid TitleId { get; set; }
    }
}