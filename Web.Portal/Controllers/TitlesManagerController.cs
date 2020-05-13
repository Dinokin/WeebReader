using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Localization.Utilities;
using WeebReader.Web.Models.Controllers.TitlesManager;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize(Roles = RoleTranslator.Administrator + "," + RoleTranslator.Moderator + "," + RoleTranslator.Uploader)]
    [Route("Admin/Titles/")]
    public class TitlesManager : Controller
    {
        private readonly TitlesManager<Title> _titleManager;
        private readonly TitleArchiver<Title> _titleArchiver;

        public TitlesManager(TitlesManager<Title> titleManager, TitleArchiver<Title> titleArchiver)
        {
            _titleManager = titleManager;
            _titleArchiver = titleArchiver;
        }

        [HttpGet("{page:int?}")]
        public async Task<IActionResult> Index(ushort page = 1)
        {
            var totalPages = Math.Ceiling(await _titleManager.Count() / (decimal) Constants.ItemsPerPageTitleAdmin);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1);

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["DeletionRoute"] = Url.Action("Delete", new {titleId = Guid.Empty}).Replace(Guid.Empty.ToString(), string.Empty);
            
            return View((await _titleManager.GetRange(Constants.ItemsPerPageTitleAdmin * (page - 1), Constants.ItemsPerPageTitleAdmin)).Select(title => Mapper.Map(title)));
        }
        
        [HttpGet("{action}")]
        public IActionResult Add(string type)
        {
            ViewData["Title"] = Labels.AddTitle;
            ViewData["ActionRoute"] = Url.Action("Add");
            ViewData["Method"] = "POST";

            return GetEditor(type, null, null);
        }
        
        [HttpPost("{action}")]
        public async Task<IActionResult> Add(ComicModel comicModel)
        {
            if (ModelState.IsValid)
            {
                if ((await _titleManager.GetAll()).Any(entity => entity.Name == comicModel.Name))
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.TitleNameAlreadyExist} 
                    });

                await using var coverStream = comicModel.Cover?.OpenReadStream();

                if (await _titleArchiver.AddTitle(Mapper.Map(comicModel), comicModel.Tags?.Split(","), coverStream))
                {
                    TempData["SuccessMessage"] = new[] {OtherMessages.TitleAddedSuccessfully};
                    
                    return new JsonResult(new
                    {
                        success = true,
                        destination = Url.Action("Index")
                    });
                }

                ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
        
        [Authorize(Roles = RoleTranslator.Administrator + "," + RoleTranslator.Moderator)]
        [HttpGet("{titleId:guid}")]
        public async Task<IActionResult> Edit(Guid titleId)
        {
            if (await _titleManager.GetById(titleId) is var title && title == null)
            {
                TempData["ErrorMessage"] = new[] {ValidationMessages.TitleNotFound};
                
                return RedirectToAction("Index");
            }
            
            ViewData["Title"] = Labels.EditTitle;
            ViewData["ActionRoute"] = Url.Action("Edit", new {titleId});
            ViewData["Method"] = "PATCH";

            return GetEditor(title, await _titleManager.GetTags(title));
        }
        
        [Authorize(Roles = RoleTranslator.Administrator + "," + RoleTranslator.Moderator)]
        [HttpPatch("{titleId:guid}")]
        public async Task<IActionResult> Edit(ComicModel comicModel)
        {
            if (ModelState.IsValid)
            {
                if (comicModel.TitleId == null || await _titleManager.GetById(comicModel.TitleId.Value) is var title && title == null)
                {
                    TempData["ErrorMessage"] = new[] {ValidationMessages.TitleNotFound};
                
                    return RedirectToAction("Index");
                }
                
                if ((await _titleManager.GetAll()).Any(entity => entity.Name == comicModel.Name && title != entity))
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.TitleNameAlreadyExist} 
                    });
                
                var comic = (Comic) title;
                Mapper.Map(comicModel, ref comic);
                await using var coverStream = comicModel.Cover?.OpenReadStream();

                if (await _titleArchiver.EditTitle(comic, comicModel.Tags?.Split(","), coverStream))
                {
                    TempData["SuccessMessage"] = new[] {OtherMessages.TitleUpdatedSuccessfully};
                    
                    return new JsonResult(new
                    {
                        success = true,
                        destination = Url.Action("Index")
                    });
                }
                
                ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
        
        [Authorize(Roles = RoleTranslator.Administrator + "," + RoleTranslator.Moderator)]
        [HttpDelete("{titleId:guid}")]
        public async Task<IActionResult> Delete(Guid titleId)
        {
            if (await _titleManager.GetById(titleId) is var title && title == null)
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.TitleNotFound}
                });

            if (await _titleArchiver.DeleteTitle(title))
            {
                TempData["SuccessMessage"] = new[] {OtherMessages.TitleDeletedSuccessfully};
                
                return new JsonResult(new
                {
                    success = true
                });
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = new[] {OtherMessages.SomethingWrong}
            });
        }

        private IActionResult GetEditor(string type, Title? title, IEnumerable<Tag>? tags) => type switch
        {
            "comic" => View("ComicEditor", title == null ? null : Mapper.Map((Comic) title, tags)),
            _ => RedirectToAction("Index")
        };

        private IActionResult GetEditor(Title title, IEnumerable<Tag>? tags) => title switch
        {
            Comic comic => View("ComicEditor", Mapper.Map(comic, tags)),
            _ => RedirectToAction("Index")
        };
    }
}