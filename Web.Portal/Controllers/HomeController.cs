using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Localization.Utilities;
using WeebReader.Web.Models.ChaptersManager;
using WeebReader.Web.Models.Home;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.TitlesManager;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Services;
using Parameter = WeebReader.Data.Entities.Parameter;

namespace WeebReader.Web.Portal.Controllers
{ 
    public class HomeController : Controller
    {
        private readonly BaseContext _context;
        private readonly EmailSender _emailSender;
        private readonly ParametersManager _parameterManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly TitlesManager<Title> _titlesManager;
        private readonly ChapterManager<Chapter> _chapterManager;
        private readonly ChapterArchiver<Chapter> _chapterArchiver;
        private readonly PagesManager<Page> _pagesManager;

        public HomeController(BaseContext context, EmailSender emailSender, ParametersManager parameterManager, UserManager<IdentityUser<Guid>> userManager, SignInManager<IdentityUser<Guid>> signInManager, TitlesManager<Title> titlesManager, ChapterManager<Chapter> chapterManager, ChapterArchiver<Chapter> chapterArchiver, PagesManager<Page> pagesManager)
        {
            _context = context;
            _emailSender = emailSender;
            _parameterManager = parameterManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _titlesManager = titlesManager;
            _chapterManager = chapterManager;
            _chapterArchiver = chapterArchiver;
            _pagesManager = pagesManager;
        }

        [HttpGet("")]
        public IActionResult Index() => View();

        [HttpGet("{action}")]
        public IActionResult Titles() => throw new NotImplementedException();
        
        [HttpGet("{action}/{titleId:Guid}/{page:int?}")]
        public async Task<IActionResult> Titles(Guid titleId, ushort page = 1)
        {
            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return RedirectToAction("Titles");

            if (!_signInManager.IsSignedIn(User) && !title.Visible)
                return RedirectToAction("Titles");

            var totalPages = Math.Ceiling(await _chapterManager.Count(title) / (decimal) Constants.ItemsPerPage);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1);
            var chapters = (await _chapterManager.GetRange(title, Constants.ItemsPerPage * (page - 1), Constants.ItemsPerPage, _signInManager.IsSignedIn(User))).Select(Mapper.Map);

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            
            return View("Title", new Tuple<TitleModel, IEnumerable<ChapterModel>>(Mapper.Map(title, await _titlesManager.GetTags(title)), chapters));
        }
        
        [HttpGet("{action}/{titleId:Guid}/{chapterId:Guid}")]
        public async Task<IActionResult> Titles(Guid titleId, Guid chapterId, bool download = false)
        {
            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return RedirectToAction("Titles");
            
            if (await _chapterManager.GetById(chapterId) is var chapter && (chapter == null || chapter.TitleId != title.Id))
                return RedirectToAction("Titles", new { titleId });

            if (!_signInManager.IsSignedIn(User) && (!title.Visible || !chapter.Visible))
                return RedirectToAction("Titles");

            if (download)
                return GetDownload(title, chapter);

            return await GetReader(title, chapter);
        }
        
        [HttpGet("{action}")]
        public async Task<IActionResult> Install() => await AllowInstaller() ? View() : (IActionResult) RedirectToAction("Index");

        [HttpPost("{action}")]
        public async Task<IActionResult> Install(InstallerModel installerModel)
        {
            if (!await AllowInstaller())
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {ValidationMessages.CannotProceedAlreadyInstalled}
                });
            
            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                var user = Mapper.Map(installerModel);
                
                var userResult = await _userManager.CreateAsync(user, installerModel.Password);

                if (userResult.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, RoleTranslator.Administrator);

                    if (roleResult.Succeeded)
                    {
                        await transaction.CommitAsync();
                        
                        TempData["SuccessMessage"] = new[] {OtherMessages.InstalledSuccessfully};

                        return new JsonResult(new
                        {
                            success = true,
                            destination = Url.Action("SignIn")
                        });
                    }
                }

                ModelState.AddModelError("SomethingWrong", OtherMessages.SomethingWrong);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpGet("Admin/{returnUrl?}")]
        public IActionResult SignIn(string returnUrl)
        {
            TempData["ReturnUrl"] = !string.IsNullOrWhiteSpace(returnUrl) ? returnUrl : null;
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UsersManager") : (IActionResult) View();
        }
        
        [HttpPost("Admin/{action:slugify}")]
        public async Task<IActionResult> SignIn(SignInModel signInModel)
        {
            if (_signInManager.IsSignedIn(User))
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {OtherMessages.AlreadySignedIn}
                });
            
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(signInModel.Username, signInModel.Password, false, true);

                if (result.Succeeded)
                    return new JsonResult(new
                    {
                        success = true,
                        destination = string.IsNullOrWhiteSpace(TempData["ReturnUrl"]?.ToString()) ? Url.Action("YourProfile", "UsersManager") : TempData["ReturnUrl"]
                    });
                
                if (result.IsLockedOut)
                    ModelState.AddModelError("LockedOut", OtherMessages.TooManyFailedSignIn);
                else
                    ModelState.AddModelError("InvalidCredentials", ValidationMessages.InvalidCredentials);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [Authorize]
        [HttpGet("Admin/{action:slugify}")]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index");
        }

        [HttpGet("Admin/{action:slugify}")]
        public async Task<IActionResult> ForgotPassword()
        {
            if (await _parameterManager.GetValue<bool>(Parameter.Types.EmailSenderEnabled) || _signInManager.IsSignedIn(User))
                return RedirectToAction("Index");
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UsersManager") : (IActionResult) View();
        }

        [HttpPost("Admin/{action:slugify}")]
        public async Task<IActionResult> ForgotPassword(EmailModel forgotPasswordModel)
        {
            if (await _parameterManager.GetValue<bool>(Parameter.Types.EmailSenderEnabled))
            {
                ModelState.AddModelError("FunctionalityDisabled", OtherMessages.DisableFunctionality);

                return new JsonResult(new
                {
                    success = false,
                    messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
                });
            }
            
            if (_signInManager.IsSignedIn(User))
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {OtherMessages.AlreadySignedInChangePassword}
                });

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var siteName = await _parameterManager.GetValue<string>(Parameter.Types.SiteName);
                    var siteAddress = await _parameterManager.GetValue<string>(Parameter.Types.SiteAddress);
                    var siteEmail = await _parameterManager.GetValue<string>(Parameter.Types.SiteEmail);

                    var message = string.Format(Emails.PasswordResetEmailBody, user.UserName, siteName, $"{siteAddress}{Url.Action("ResetPassword", new {userId = user.Id, token})}");

                    await _emailSender.SendEmail(siteEmail, user.Email, string.Format(Emails.PasswordResetEmailSubject, siteName), message);
                }

                TempData["SuccessMessage"] = new[] {OtherMessages.EmailAlreadyInDatabaseWarning};
                
                return new JsonResult(new
                {
                    success = true,
                    destination = Url.Action("SignIn")
                });
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        } 
        
        [HttpGet("Admin/{action:slugify}")]
        public async Task<IActionResult> ResetPassword(Guid userId, string token)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("YourProfile", "UsersManager");

            if (await _userManager.FindByIdAsync(userId.ToString()) != null && !string.IsNullOrWhiteSpace(token))
                return View(new ResetPasswordModel
                {
                    UserId = userId,
                    Token = token
                });

            TempData["ErrorMessage"] = new[] {OtherMessages.PasswordResetInvalidData};

            return RedirectToAction("Index");
        } 
        
        [HttpPatch("Admin/{action:slugify}")]
        public async Task<IActionResult> ChangePassword(ResetPasswordModel resetPasswordModel)
        {
            if (_signInManager.IsSignedIn(User))
                return new JsonResult(new
                {
                    success = false,
                    messages = new[] {OtherMessages.AlreadySignedInChangePassword}
                });

            if (ModelState.IsValid && await _userManager.FindByIdAsync(resetPasswordModel.UserId.ToString()) is var user && user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.NewPassword);

                if (result.Succeeded)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    
                    TempData["SuccessMessage"] = new[] {OtherMessages.PasswordChangedSuccessfully};
                    
                    return new JsonResult(new
                    {
                        success = true,
                        destination = Url.Action("SignIn")
                    });
                }
                
                ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotChangePassword);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpGet("Admin/{action:slugify}")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailModel changeEmailModel)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(changeEmailModel.Email) is var candidate && candidate != null)
                    TempData["ErrorMessage"] = new[] {ValidationMessages.EmailAlreadyInUse};
                else
                {
                    var user = await _userManager.FindByIdAsync(changeEmailModel.UserId.ToString());

                    if (user != null)
                    {
                        var result = await _userManager.ChangeEmailAsync(user, changeEmailModel.Email, changeEmailModel.Token);

                        if (result.Succeeded)
                            TempData["SuccessMessage"] = new[] {OtherMessages.EmailChangedSuccessfully};
                    }
                    else
                        ModelState.AddModelError("NotSucceeded", OtherMessages.CouldNotChangeEmail);
                }
            }
            else
                TempData["ErrorMessage"] = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage);
            
            return _signInManager.IsSignedIn(User) ? RedirectToAction("YourProfile", "UsersManager") : RedirectToAction("Index");
        }
        
        private IActionResult GetDownload(Title title, Chapter chapter) => chapter switch
        {
            ComicChapter comicChapter => File(_chapterArchiver.GetChapterDownload(comicChapter)?.OpenRead(), "application/zip", $"{GetDownloadName(title, comicChapter)}.zip"),
            _ => RedirectToAction("Titles", new { titleId = chapter.TitleId })
        };
        
        private async Task<IActionResult> GetReader(Title title, Chapter chapter) => chapter switch
        {
            ComicChapter comicChapter => await GetComicReader((Comic) title, comicChapter),
            _ => RedirectToAction("Titles", new { titleId = chapter.TitleId })
        };

        private async Task<IActionResult> GetComicReader(Comic comic, ComicChapter comicChapter)
        {
            var comicModel = Mapper.Map(comic, null);
            var comicChapterModel = Mapper.Map(comicChapter);
            var pages = (IEnumerable<ComicPage>) await _pagesManager.GetRange(comicChapter);

            return View("ComicReader", new Tuple<ComicModel, ComicChapterModel, IEnumerable<ComicPage>>(comicModel, comicChapterModel, pages));
        }
        
        private async Task<bool> AllowInstaller() => !await _userManager.Users.AnyAsync();
        
        private string GetDownloadName(Title title, Chapter chapter) => $"{title.Name} - {Labels.Chapter} {chapter.Number}";
    }
}