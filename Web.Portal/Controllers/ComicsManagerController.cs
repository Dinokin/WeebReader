using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Web.Models.Mappers;
using WeebReader.Web.Models.Models.TitleManager;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize]
    public class ComicsManagerController : Controller
    {
        private readonly TitlePacker<Comic> _titlePacker;

        public ComicsManagerController(TitlePacker<Comic> titlePacker) => _titlePacker = titlePacker;

        public IActionResult Index() => View();
        
        [HttpGet]
        public IActionResult Add() => View();

        [HttpPost]
        [RequestSizeLimit(100000000)]
        public async Task<IActionResult> Add(ComicModel comicModel)
        {
            if (TryValidateModel(comicModel))
            {
                if (comicModel.Cover != null)
                {
                    if (await _titlePacker.AddTitle(TitleMapper.MapComicToEntity(comicModel), comicModel.Tags.Split(","), comicModel.Cover.OpenReadStream()))
                    {
                        TempData["SuccessMessage"] = new[] {"Title added successfully."};
                
                        return new JsonResult(new
                        {
                            success = true,
                            destination = Url.Action("Index")
                        });
                    }

                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {"Something went wrong and the title could not be added."}
                    });
                }
                else
                    ModelState.AddModelError("MissingCover", "A cover page is required to add a title.");
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
    }
}