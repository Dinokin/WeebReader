using System;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Localization.Utilities;
using WeebReader.Web.Models.Controllers.ChaptersManager;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize(Roles = RoleTranslator.Administrator + "," + RoleTranslator.Moderator + "," + RoleTranslator.Uploader)]
    [Route("Admin/Titles/{titleId:guid}/Chapters")]
    public class ChaptersManagerController : Controller
    {
        private readonly TitlesManager<Title> _titleManager;
        private readonly ChapterManager<Chapter> _chapterManager;
        private readonly ChapterArchiver<Chapter> _chapterArchiver;

        public ChaptersManagerController(TitlesManager<Title> titleManager, ChapterManager<Chapter> chapterManager, ChapterArchiver<Chapter> chapterArchiver)
        {
            _titleManager = titleManager;
            _chapterManager = chapterManager;
            _chapterArchiver = chapterArchiver;
        }

        [HttpGet("{page:int?}")]
        public async Task<IActionResult> Index(Guid titleId, ushort page = 1)
        {
            if (await _titleManager.GetById(titleId) is var title && title == null)
            {
                TempData["ErrorMessage"] = new[] {ValidationMessages.TitleNotFound};
                
                return RedirectToAction("Index", "TitlesManager");
            }
            
            var totalPages = Math.Ceiling(await _chapterManager.Count(title) / (decimal) Constants.ItemsPerPageChapterAdmin);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1);

            ViewData["Title"] = $"{Labels.Chapters} - {title.Name}";
            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["DeletionRoute"] = Url.Action("Delete", new {titleId, chapterId = Guid.Empty}).Replace(Guid.Empty.ToString(), string.Empty);

            return View((await _chapterManager.GetRange(title, Constants.ItemsPerPageChapterAdmin * (page - 1), Constants.ItemsPerPageChapterAdmin)).Select(Mapper.MapToModel));
        }
        
        [HttpGet("{action}")]
        public async Task<IActionResult> Add(Guid titleId)
        {
            if (await _titleManager.GetById(titleId) is var title && title == null)
            {
                TempData["ErrorMessage"] = new[] {ValidationMessages.TitleNotFound};
                
                return RedirectToAction("Index", "TitlesManager");
            }
            
            ViewData["Title"] = $"{Labels.AddChapter} - {title.Name}";
            ViewData["ActionRoute"] = Url.Action("Add", new { titleId });
            ViewData["Method"] = "POST";
            ViewData["Function"] = Labels.AddChapter;

            return GetEditor(title);
        }
        
        [HttpPost("{action}")]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> Add(ChapterModel chapterModel)
        {
            if (ModelState.IsValid)
            {
                if (await _titleManager.GetById(chapterModel.TitleId) is var title && title == null)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.TitleNotFound} 
                    });
                
                if ((await _chapterManager.GetAll()).Any(entity => entity.TitleId == title.Id && entity.Number == chapterModel.Number))
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.ChapterNumberAlreadyExist} 
                    });

                ZipArchive? pagesZip = null;

                if (chapterModel is ComicChapterModel comicChapterModel)
                {
                    if (comicChapterModel.Pages == null)
                        return new JsonResult(new
                        {
                            success = false,
                            messages = new[] {ValidationMessages.ChapterMustHavePages} 
                        });
                    
                    var pagesStream = comicChapterModel.Pages.OpenReadStream();
                    pagesZip = new ZipArchive(pagesStream);
                }
                
                if (await _chapterArchiver.AddChapter(Mapper.MapToEntity(chapterModel), pagesZip))
                {
                    pagesZip?.Dispose();
                    TempData["SuccessMessage"] = new[] {OtherMessages.ChapterAddedSuccessfully};
                    
                    return new JsonResult(new
                    {
                        success = true,
                        destination = Url.Action("Index", new {titleId = title.Id})
                    });
                }
                
                pagesZip?.Dispose();
                ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
        
        [Authorize(Roles = RoleTranslator.Administrator + "," + RoleTranslator.Moderator)]
        [HttpGet("{chapterId:guid}")]
        public async Task<IActionResult> Edit(Guid titleId, Guid chapterId)
        {
            if (await _chapterManager.GetById(chapterId) is var chapter && chapter == null)
            {
                TempData["ErrorMessage"] = new[] {ValidationMessages.ChapterNotFound};
                
                return RedirectToAction("Index", new { titleId });
            }
            
            ViewData["Title"] = $"{Labels.EditChapter} - {(await _titleManager.GetById(titleId)).Name}";
            ViewData["ActionRoute"] = Url.Action("Edit");
            ViewData["Method"] = "PATCH";
            ViewData["Function"] = Labels.EditChapter;

            return GetEditor(chapter);
        }

        [Authorize(Roles = RoleTranslator.Administrator + "," + RoleTranslator.Moderator)]
        [HttpPatch("{chapterId:guid}")]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> Edit(ChapterModel chapterModel)
        {
            if (ModelState.IsValid)
            {
                if (chapterModel.ChapterId == null ||await _chapterManager.GetById(chapterModel.ChapterId.Value) is var chapter && chapter == null)
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.ChapterNotFound} 
                    });
                
                if ((await _chapterManager.GetAll()).Any(entity => entity.TitleId == chapter.TitleId && entity.Number == chapterModel.Number && entity != chapter))
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.ChapterNumberAlreadyExist} 
                    });
                
                ZipArchive? pagesZip = null;

                if (chapterModel is ComicChapterModel comicChapterModel)
                {
                    var pagesStream = comicChapterModel.Pages?.OpenReadStream();
                    pagesZip = pagesStream == null ? null : new ZipArchive(pagesStream);
                }
                
                Mapper.MapEditModelToEntity(chapterModel, ref chapter);

                if (await _chapterArchiver.EditChapter(chapter, pagesZip))
                {
                    TempData["SuccessMessage"] = new[] {OtherMessages.ChapterUpdatedSuccessfully};
                    
                    return new JsonResult(new
                    {
                        success = true,
                        destination = Url.Action("Index", new {titleId = chapterModel.TitleId})
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
        [HttpDelete("{chapterId:guid}")]
        public async Task<IActionResult> Delete(Guid chapterId)
        {
            if (await _chapterManager.GetById(chapterId) is var chapter && chapter == null)
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.ChapterNotFound}
                });

            if (await _chapterArchiver.DeleteChapter(chapter))
            {
                TempData["SuccessMessage"] = new[] {OtherMessages.ChapterDeletedSuccessfully};
                
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
        
        private IActionResult GetEditor(Title title) => title switch
        {
            Comic _ => View("ComicChapterEditor"),
            Novel _ => View("NovelChapterEditor"),
            _ => RedirectToAction("Index")
        };
        
        private IActionResult GetEditor(Chapter chapter) => chapter switch
        {
            ComicChapter comicChapter => View("ComicChapterEditor", (ComicChapterModel) Mapper.MapToModel(comicChapter)),
            NovelChapter novelChapter => View("NovelChapterEditor", (NovelChapterModel) Mapper.MapToModel(novelChapter)),
            _ => RedirectToAction("Index")
        };
    }
}