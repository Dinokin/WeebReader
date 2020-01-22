using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Web.Models.Attributes;

namespace WeebReader.Web.Models.Models.TitleManager
{
    public class TitleModel
    {
        [Required(ErrorMessage = "The name of the title is a required.")]
        [MaxLength(200, ErrorMessage = "The name of the title must not surpass 200 characters.")]
        public string Name { get; set; }
        [MaxLength(200, ErrorMessage = "The original name of the title must not surpass 200 characters.")]
        public string OriginalName { get; set; }
        [Required(ErrorMessage = "The name of the author is a required.")]
        [MaxLength(50, ErrorMessage = "The name of the author must not surpass 50 characters.")]
        public string Author { get; set; }
        [Required(ErrorMessage = "The name of the artist is a required.")]
        [MaxLength(50, ErrorMessage = "The name of the artist must not surpass 50 characters.")]
        public string Artist { get; set; }
        public string Tags { get; set; }
        [Required(ErrorMessage = "A synopsis is required for this title.")]
        public string Synopsis { get; set; }
        [Required(ErrorMessage = "A status is required for this title.")]
        public Title.Statuses Status { get; set; }
        [Required(ErrorMessage = "The visibility must be set.")]
        public bool Visible { get; set; }
        [AllowedFormats(".png,.jpg,.jpeg", ErrorMessage = "Unsupported cover format. The supported formats are: .png and .jpg/jpeg")]
        public IFormFile Cover { get; set; }
    }
}