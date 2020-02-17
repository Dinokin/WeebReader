using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Data.Services;
using WeebReader.Web.Localization.Utilities;
using WeebReader.Web.Models.BlogManager;

namespace WeebReader.Web.Portal.Controllers
{
    
    [Authorize(Roles = RoleTranslator.Administrator + "," + RoleTranslator.Moderator)]
    [Route("Admin/Blog/")]
    public class BlogManagerController : Controller
    {
        private readonly SettingsManager _settingsManager;
        private readonly GenericManager<Post> _postManager;

        public BlogManagerController(SettingsManager settingsManager, GenericManager<Post> postManager)
        {
            _settingsManager = settingsManager;
            _postManager = postManager;
        }

        [HttpGet("{page:int?}")]
        public IActionResult Index(ushort page = 1)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{action}")]
        public IActionResult Add()
        {
            throw new NotImplementedException();
        }

        [HttpPost("{action}")]
        public IActionResult Add(PostModel postModel)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{action}/{postId:guid}")]
        public IActionResult Edit(Guid postId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{action}")]
        public IActionResult Edit(PostModel postModel)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{postId:guid}")]
        public IActionResult Delete(Guid postId)
        {
            throw new NotImplementedException();
        }
    }
}