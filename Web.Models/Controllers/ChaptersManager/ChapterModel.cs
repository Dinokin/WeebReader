using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Controllers.ChaptersManager
{
    [ModelBinder(BinderType = typeof(ChapterModelBinder))]
    public class ChapterModel
    {
        public Guid? ChapterId { get; set; }
        
        public ushort? Volume { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "ChapterMustHaveNumber")]
        [Range(typeof(decimal), "0", "9999.9", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "ChapterNumberOutOfRange")]
        public decimal Number { get; set; }
        
        [StringLength(200, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "TitleNameMaxLength")]
        public string? Name { get; set; }
        
        public DateTime? ReleaseDate { get; set; }
        
        public bool Visible { get; set; }
        
        public Guid TitleId { get; set; }
    }
}