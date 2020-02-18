﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Entities;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Localization.Utilities;
using WeebReader.Web.Models.BlogManager;
using WeebReader.Web.Portal.Others;

namespace WeebReader.Web.Portal.Controllers
{
    
    [Authorize(Roles = RoleTranslator.Administrator + "," + RoleTranslator.Moderator)]
    [Route("Admin/Blog/")]
    public class BlogManagerController : Controller
    {
        private readonly SettingsManager _settingsManager;
        private readonly GenericManager<Post> _postsManager;

        public BlogManagerController(SettingsManager settingsManager, GenericManager<Post> postsManager)
        {
            _settingsManager = settingsManager;
            _postsManager = postsManager;
        }

        [HttpGet("{page:int?}")]
        public async Task<IActionResult> Index(ushort page = 1)
        {
            var totalPages = Math.Ceiling(await _postsManager.Count() / (decimal) Constants.ItemsPerPage);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1);

            var posts = (await _postsManager.GetRange(Constants.ItemsPerPage * (page - 1), Constants.ItemsPerPage))
                .Select(post => new PostModel
                {
                    PostId = post.Id,
                    Name = post.Name,
                    Content = post.Content,
                    Date = post.Date
                });

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            
            return View(posts);
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
            if (TryValidateModel(postModel))
            {
                if (await _postsManager.Entities.AnyAsync(entity => entity.Name == postModel.Name))
                    return new JsonResult(new
                    {
                        success = false,
                        messages = new[] {ValidationMessages.PostNameAlreadyExist} 
                    });

                var post = new Post
                {
                    Name = postModel.Name,
                    Content = postModel.Content,
                    Date = postModel.Date ?? DateTime.UtcNow
                };

                if (await _postsManager.Add(post))
                {
                    ViewData["SuccessMessage"] = new[] {OtherMessages.PostAddedSuccessfully};
                    
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

        [HttpGet("{action}/{postId:guid}")]
        public async Task<IActionResult> Edit(Guid postId)
        {
            if (await _postsManager.GetById(postId) is var post && post == null)
            {
                ViewData["ErrorMessage"] = new[] {ValidationMessages.PostNotFound};
                
                return RedirectToAction("Index");
            }

            return View("PostEditor", new PostModel
            {
                PostId = post.Id,
                Name = post.Name,
                Content = post.Content,
                Date = post.Date
            });
        }

        [HttpPatch("{action}")]
        public async Task<IActionResult> Edit(PostModel postModel)
        {
            if (TryValidateModel(postModel))
            {
                if (await _postsManager.GetById(postModel.PostId) is var post && post == null)
                {
                    ViewData["ErrorMessage"] = new[] {ValidationMessages.PostNotFound};
                
                    return RedirectToAction("Index");
                }

                post.Name = postModel.Name;
                post.Content = postModel.Content;
                post.Date = postModel.Date ?? post.Date;

                if (await _postsManager.Edit(post))
                {
                    ViewData["SuccessMessage"] = new[] {OtherMessages.PostUpdatedSuccessfully};
                    
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
                messages = new[] {OtherMessages.SomethingWrong}
            });
        }

        [HttpDelete("{postId:guid}")]
        public async Task<IActionResult> Delete(Guid postId)
        {
            if (await _postsManager.GetById(postId) is var post && post == null)
            {
                ViewData["ErrorMessage"] = new[] {ValidationMessages.PostNotFound};
                
                return RedirectToAction("Index");
            }

            if (await _postsManager.Delete(post))
            {
                TempData["SuccessMessage"] = new[] {OtherMessages.PostDeletedSuccessfully};
                
                return new JsonResult(new
                {
                    success = true,
                    destination = Url.Action("Index")
                });
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = new[] {OtherMessages.SomethingWrong}
            });
        }
    }
}