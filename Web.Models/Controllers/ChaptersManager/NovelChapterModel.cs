using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Models.Controllers.ChaptersManager
{
    public class NovelChapterModel : ChapterModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "ChapterMustHaveContent")]
        public string Content { get; set; } = string.Empty;
    }
}