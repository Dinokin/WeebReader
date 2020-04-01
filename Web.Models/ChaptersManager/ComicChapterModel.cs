using Microsoft.AspNetCore.Http;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others.Validation;

namespace WeebReader.Web.Models.ChaptersManager
{
    public class ComicChapterModel : ChapterModel
    {
        [AllowedFormats(".zip", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "InvalidPagesFormat")]
        public IFormFile? Pages { get; set; }
    }
}