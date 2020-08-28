using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Controllers.Home;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly TitlesManager<Title> _titlesManager;
        private readonly ChapterManager<Chapter> _chapterManager;
        private readonly PagesManager<Page> _pagesManager;
        private readonly NovelChapterContentManager _novelChapterContentManager;
        private readonly ChapterArchiver<Chapter> _chapterArchiver;
        private readonly PostsManager _postsManager;
        private readonly EmailSender _emailSender;
        private readonly ParametersManager _parametersManager;
        private readonly ReCaptchaValidator _reCaptchaValidator;
        private readonly IMemoryCache _memoryCache;

        public HomeController(SignInManager<IdentityUser<Guid>> signInManager, TitlesManager<Title> titlesManager, ChapterManager<Chapter> chapterManager, PagesManager<Page> pagesManager, NovelChapterContentManager novelChapterContentManager,
            ChapterArchiver<Chapter> chapterArchiver, PostsManager postsManager, EmailSender emailSender, ParametersManager parametersManager, ReCaptchaValidator reCaptchaValidator, IMemoryCache memoryCache)
        {
            _signInManager = signInManager;
            _titlesManager = titlesManager;
            _chapterManager = chapterManager;
            _pagesManager = pagesManager;
            _novelChapterContentManager = novelChapterContentManager;
            _chapterArchiver = chapterArchiver;
            _postsManager = postsManager;
            _emailSender = emailSender;
            _parametersManager = parametersManager;
            _reCaptchaValidator = reCaptchaValidator;
            _memoryCache = memoryCache;
        }

        [HttpGet("{action}")]
        public async Task<IActionResult> Blog()
        {
            if (!await _parametersManager.GetValue<bool>(Parameter.Types.PageBlogEnabled))
                return RedirectToAction("Index");

            ViewData["TotalPages"] = Math.Ceiling(await _postsManager.Count(_signInManager.IsSignedIn(User)) / (decimal) Constants.ItemsPerPagePosts);

            return View(await _postsManager.GetRange(0, Constants.ItemsPerPagePosts, _signInManager.IsSignedIn(User)));
        }

        [HttpGet("{action}/{page:int}")]
        public async Task<IActionResult> Blog(ushort page)
        {
            if (page == 0)
                page = 1;
            
            return PartialView("Partials/BlogPosts", await _postsManager.GetRange(Constants.ItemsPerPagePosts * (page - 1), Constants.ItemsPerPagePosts, _signInManager.IsSignedIn(User)));
        }

        [HttpGet("{action}")]
        public async Task<IActionResult> About()
        {
            var aboutEnabled = await _parametersManager.GetValue<bool>(Parameter.Types.PageAboutEnabled);
            var kofiEnabled = await _parametersManager.GetValue<bool>(Parameter.Types.PageAboutKofiEnabled);
            var patreonEnabled = await _parametersManager.GetValue<bool>(Parameter.Types.PageAboutPatreonEnabled);

            if (aboutEnabled || kofiEnabled || patreonEnabled)
                return View();

            return RedirectToAction("Index");
        }

        [HttpGet("{action}")]
        public async Task<IActionResult> Contact()
        {
            var emailSenderEnabled = await _parametersManager.GetValue<bool>(Parameter.Types.EmailSenderEnabled);
            var emailContactEnabled = await _parametersManager.GetValue<bool>(Parameter.Types.ContactEmailEnabled);
            var discordEnabled = await _parametersManager.GetValue<bool>(Parameter.Types.ContactDiscordEnabled);

            if (emailSenderEnabled && emailContactEnabled || discordEnabled)
                return View();

            return RedirectToAction("Index");
        }

        [HttpPost("{action}")]
        public async Task<IActionResult> Contact(ContactModel contactModel)
        {
            if (ModelState.IsValid)
            {
                var emailSenderEnabled = await _parametersManager.GetValue<bool>(Parameter.Types.EmailSenderEnabled);
                var emailContactEnabled = await _parametersManager.GetValue<bool>(Parameter.Types.ContactEmailEnabled);
                
                if (emailSenderEnabled && emailContactEnabled)
                {
                    var reCaptchaEnabled = await _parametersManager.GetValue<bool>(Parameter.Types.ContactEmailRecaptchaEnabled);

                    if (!reCaptchaEnabled || await _reCaptchaValidator.Validate(contactModel.ReCaptchaResponse!, null))
                    {
                        if (await _emailSender.SendEmail(contactModel.Email, await _parametersManager.GetValue<string>(Parameter.Types.SiteEmail), string.Format(OtherMessages.MessageFrom, contactModel.Nickname), contactModel.Message))
                        {
                            TempData["SuccessMessage"] = new[] {OtherMessages.MessageSentSuccessfully};

                            return new JsonResult(new
                            {
                                success = true
                            });
                        }
                    }
                    else
                        ModelState.AddModelError("CouldNotVerifyRobot", OtherMessages.CouldntVerifyRobot);
                } 
                
                ModelState.AddModelError("MessageNotSent", OtherMessages.MessageCouldntBeSent);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }
    }
}