﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Controllers.BlogManager;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Portal.Others;
using Utilities = WeebReader.Web.Localization.Others.Utilities;

namespace WeebReader.Web.Portal.Controllers
{
    
    [Authorize(Roles = Utilities.Roles.Administrator + "," + Utilities.Roles.Moderator)]
    [Route("Admin/Blog/")]
    public class BlogManagerController : Controller
    {
        private readonly PostsManager _postManager;

        public BlogManagerController(PostsManager postManager) => _postManager = postManager;

        [HttpGet("{page:int?}")]
        public async Task<IActionResult> Index(ushort page = 1)
        {
            var totalPages = Math.Ceiling(await _postManager.Count() / (decimal) Constants.ItemsPerPageBlogAdmin);
            page = page >= 1 && page <= totalPages ? page : (ushort) 1;

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["DeletionRoute"] = Url.Action("Delete", new {postId = Guid.Empty}).Replace(Guid.Empty.ToString(), string.Empty);
            
            return View((await _postManager.GetRange(Constants.ItemsPerPageBlogAdmin * (page - 1), Constants.ItemsPerPageBlogAdmin)));
        }

        [HttpGet("{action}")]
        public IActionResult Add()
        {
            ViewData["Title"] = Labels.AddPost;
            ViewData["ActionRoute"] = Url.Action("Add");
            ViewData["Method"] = "POST";
            
            return View("PostEditor");
        }

        [HttpPost("{action}")]
        public async Task<IActionResult> Add(PostModel postModel)
        {
            if (ModelState.IsValid)
            {
                if ((await _postManager.GetAll()).Any(entity => entity.Title == postModel.Title))
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.PostNameAlreadyExist} 
                    });

                if (await _postManager.Add(Mapper.MapToEntity(postModel)))
                {
                    TempData["SuccessMessage"] = new[] {OtherMessages.PostAddedSuccessfully};
                    
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

        [HttpGet("{postId:guid}")]
        public async Task<IActionResult> Edit(Guid postId)
        {
            if (await _postManager.GetById(postId) is var post && post == null)
            {
                TempData["ErrorMessage"] = new[] {ValidationMessages.PostNotFound};
                
                return RedirectToAction("Index");
            }
            
            ViewData["Title"] = Labels.EditPost;
            ViewData["ActionRoute"] = Url.Action("Edit", new {postId});
            ViewData["Method"] = "PATCH";

            return View("PostEditor", Mapper.MapToModel(post));
        }

        [HttpPatch("{postId:guid}")]
        public async Task<IActionResult> Edit(PostModel postModel)
        {
            if (ModelState.IsValid)
            {
                if (postModel.PostId == null || await _postManager.GetById(postModel.PostId.Value) is var post && post == null)
                {
                    TempData["ErrorMessage"] = new[] {ValidationMessages.PostNotFound};
                
                    return RedirectToAction("Index");
                }
                
                if ((await _postManager.GetAll()).Any(entity => entity.Title == postModel.Title && post != entity))
                    return Json(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.PostNameAlreadyExist} 
                    });

                Mapper.MapEditModelToEntity(postModel, ref post);
                
                if (await _postManager.Edit(post))
                {
                    TempData["SuccessMessage"] = new[] {OtherMessages.PostUpdatedSuccessfully};
                    
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

        [HttpDelete("{postId:guid}")]
        public async Task<IActionResult> Delete(Guid postId)
        {
            if (await _postManager.GetById(postId) is var post && post == null)
                return Json(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.PostNotFound}
                });

            if (await _postManager.Delete(post))
            {
                TempData["SuccessMessage"] = new[] {OtherMessages.PostDeletedSuccessfully};
                
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
    }
}