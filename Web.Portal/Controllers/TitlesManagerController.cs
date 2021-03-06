﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Controllers.TitlesManager;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Services;
using Utilities = WeebReader.Web.Localization.Others.Utilities;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize(Roles = Utilities.Roles.Administrator + "," + Utilities.Roles.Moderator + "," + Utilities.Roles.Uploader)]
    [Route("Admin/Titles/")]
    public class TitlesManager : Controller
    {
        private readonly TitlesManager<Title> _titleManager;
        private readonly TitleArchiver _titleArchiver;

        public TitlesManager(TitlesManager<Title> titleManager, TitleArchiver titleArchiver)
        {
            _titleManager = titleManager;
            _titleArchiver = titleArchiver;
        }

        [HttpGet("{page:int?}")]
        public async Task<IActionResult> Index(ushort page = 1)
        {
            var totalPages = Math.Ceiling(await _titleManager.Count() / (decimal) Constants.ItemsPerPageTitleAdmin);
            page = page >= 1 && page <= totalPages ? page : (ushort) 1;

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["DeletionRoute"] = Url.Action("Delete", new {titleId = Guid.Empty}).Replace(Guid.Empty.ToString(), string.Empty);
            
            return View((await _titleManager.GetRange(Constants.ItemsPerPageTitleAdmin * (page - 1), Constants.ItemsPerPageTitleAdmin)));
        }
        
        [Authorize(Roles = Utilities.Roles.Administrator + "," + Utilities.Roles.Moderator)]
        [HttpGet("{action}")]
        public IActionResult Add(string type)
        {
            ViewData["Title"] = Labels.AddTitle;
            ViewData["ActionRoute"] = Url.Action("Add", new {type});
            ViewData["Method"] = "POST";

            return GetEditor(type);
        }
        
        [Authorize(Roles = Utilities.Roles.Administrator + "," + Utilities.Roles.Moderator)]
        [HttpPost("{action}")]
        public async Task<IActionResult> Add(TitleModel titleModel, string type)
        {
            if (ModelState.IsValid)
            {
                if ((await _titleManager.GetAll()).Any(entity => entity.Name == titleModel.Name))
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.TitleNameAlreadyExist} 
                    });

                await using var coverStream = titleModel.Cover?.OpenReadStream();
                
                if (await _titleArchiver.AddTitle(Mapper.MapToEntity(titleModel, type), titleModel.Tags?.Split(","), coverStream))
                {
                    TempData["SuccessMessage"] = new[] {OtherMessages.TitleAddedSuccessfully};
                    
                    return Json(new
                    {
                        success = true,
                        destination = Url.Action("Index")
                    });
                }

                ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
            }
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
        
        [Authorize(Roles = Utilities.Roles.Administrator + "," + Utilities.Roles.Moderator)]
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
        
        [Authorize(Roles = Utilities.Roles.Administrator + "," + Utilities.Roles.Moderator)]
        [HttpPatch("{titleId:guid}")]
        public async Task<IActionResult> Edit(TitleModel titleModel)
        {
            if (ModelState.IsValid)
            {
                if (titleModel.TitleId == null || await _titleManager.GetById(titleModel.TitleId.Value) is var title && title == null)
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.TitleNotFound} 
                    });
                
                if ((await _titleManager.GetAll()).Any(entity => entity.Name == titleModel.Name && title != entity))
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.TitleNameAlreadyExist} 
                    });
                
                Mapper.MapEditModelToEntity(titleModel, ref title);
                await using var coverStream = titleModel.Cover?.OpenReadStream();

                if (await _titleArchiver.EditTitle(title, titleModel.Tags?.Split(","), coverStream))
                {
                    TempData["SuccessMessage"] = new[] {OtherMessages.TitleUpdatedSuccessfully};
                    
                    return Json(new
                    {
                        success = true,
                        destination = Url.Action("Index")
                    });
                }
                
                ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
            }
            
            return Json(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
        
        [Authorize(Roles = Utilities.Roles.Administrator + "," + Utilities.Roles.Moderator)]
        [HttpDelete("{titleId:guid}")]
        public async Task<IActionResult> Delete(Guid titleId)
        {
            if (await _titleManager.GetById(titleId) is var title && title == null)
                return Json(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.TitleNotFound}
                });

            if (await _titleArchiver.DeleteTitle(title))
            {
                TempData["SuccessMessage"] = new[] {OtherMessages.TitleDeletedSuccessfully};
                
                return Json(new
                {
                    success = true
                });
            }
            
            return Json(new
            {
                success = false,
                messages = new[] {OtherMessages.SomethingWrong}
            });
        }

        private IActionResult GetEditor(string type) => type switch
        {
            "comic" => View("ComicEditor"),
            "novel" => View("TitleEditor"),
            _ => RedirectToAction("Index")
        };

        private IActionResult GetEditor(Title title, IEnumerable<Tag>? tags = null) => title switch
        {
            Comic comic => View("ComicEditor", (ComicModel) Mapper.MapToModel(comic, tags)),
            Novel novel => View("TitleEditor", Mapper.MapToModel(novel, tags)),
            _ => RedirectToAction("Index")
        };
    }
}