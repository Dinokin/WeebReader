using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Controllers.Home;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Extensions;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly PostsManager _postsManager;
        private readonly EmailSender _emailSender;
        private readonly ParametersManager _parametersManager;
        private readonly ReCaptchaValidator _reCaptchaValidator;

        public HomeController(SignInManager<IdentityUser<Guid>> signInManager, PostsManager postsManager, EmailSender emailSender, ParametersManager parametersManager, ReCaptchaValidator reCaptchaValidator)
        {
            _signInManager = signInManager;
            _postsManager = postsManager;
            _emailSender = emailSender;
            _parametersManager = parametersManager;
            _reCaptchaValidator = reCaptchaValidator;
        }

        [HttpGet("{action}")]
        public async Task<IActionResult> Blog()
        {
            if (!await _parametersManager.GetValue<bool>(ParameterTypes.PageBlogEnabled))
                return RedirectToAction("Index", "Content");

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
            var aboutEnabled = await _parametersManager.GetValue<bool>(ParameterTypes.PageAboutEnabled);
            var kofiEnabled = await _parametersManager.GetValue<bool>(ParameterTypes.PageAboutKofiEnabled);
            var patreonEnabled = await _parametersManager.GetValue<bool>(ParameterTypes.PageAboutPatreonEnabled);

            if (aboutEnabled || kofiEnabled || patreonEnabled)
                return View();

            return RedirectToAction("Index", "Content");
        }

        [HttpGet("{action}")]
        public async Task<IActionResult> Contact()
        {
            var emailSenderEnabled = await _parametersManager.GetValue<bool>(ParameterTypes.EmailSenderEnabled);
            var emailContactEnabled = await _parametersManager.GetValue<bool>(ParameterTypes.ContactEmailEnabled);
            var discordEnabled = await _parametersManager.GetValue<bool>(ParameterTypes.ContactDiscordEnabled);

            if (emailSenderEnabled && emailContactEnabled || discordEnabled)
                return View();

            return RedirectToAction("Index", "Content");
        }

        [HttpPost("{action}")]
        public async Task<IActionResult> Contact(ContactModel contactModel)
        {
            if (ModelState.IsValid)
            {
                var emailSenderEnabled = await _parametersManager.GetValue<bool>(ParameterTypes.EmailSenderEnabled);
                var emailContactEnabled = await _parametersManager.GetValue<bool>(ParameterTypes.ContactEmailEnabled);
                
                if (emailSenderEnabled && emailContactEnabled)
                {
                    var reCaptchaEnabled = await _parametersManager.GetValue<bool>(ParameterTypes.ContactEmailRecaptchaEnabled);

                    if (!reCaptchaEnabled || await _reCaptchaValidator.Validate(contactModel.ReCaptchaResponse!, null))
                    {
                        if (await _emailSender.SendEmail(contactModel.Email, await _parametersManager.GetValue<string>(ParameterTypes.SiteEmail), string.Format(OtherMessages.MessageFrom, contactModel.Nickname), contactModel.Message))
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