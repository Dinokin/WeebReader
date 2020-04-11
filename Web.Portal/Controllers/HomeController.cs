using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Controllers.BlogManager;
using WeebReader.Web.Models.Controllers.ChaptersManager;
using WeebReader.Web.Models.Controllers.Home;
using WeebReader.Web.Models.Controllers.TitlesManager;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{ 
    public class HomeController : Controller
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly TitlesManager<Title> _titlesManager;
        private readonly ChapterManager<Chapter> _chapterManager;
        private readonly ChapterArchiver<Chapter> _chapterArchiver;
        private readonly PagesManager<Page> _pagesManager;
        private readonly PostsManager _postsManager;
        private readonly EmailSender _emailSender;
        private readonly ParametersManager _parametersManager;

        public HomeController(SignInManager<IdentityUser<Guid>> signInManager, TitlesManager<Title> titlesManager, ChapterManager<Chapter> chapterManager, ChapterArchiver<Chapter> chapterArchiver, PagesManager<Page> pagesManager, PostsManager postsManager, EmailSender emailSender, ParametersManager parametersManager)
        {
            _signInManager = signInManager;
            _titlesManager = titlesManager;
            _chapterManager = chapterManager;
            _chapterArchiver = chapterArchiver;
            _pagesManager = pagesManager;
            _postsManager = postsManager;
            _emailSender = emailSender;
            _parametersManager = parametersManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var posts = (await _postsManager.GetRange(0, 5, _signInManager.IsSignedIn(User))).Select(Mapper.Map);
            var releases = (await _chapterManager.GetRange(0, 8, _signInManager.IsSignedIn(User))).Select(Mapper.Map).ToArray();
            var titles = releases.Select(async chapter => Mapper.Map(await _titlesManager.GetById(chapter.TitleId), null)).Select(task => task.Result);

            return View(new Tuple<IEnumerable<PostModel>, IEnumerable<Tuple<TitleModel, ChapterModel>>>(posts,
                releases.Join(titles, chapter => chapter.TitleId, title => title.TitleId, (chapter, title) => new Tuple<TitleModel, ChapterModel>(title, chapter)).Distinct(new ReleaseComparer()).OrderByDescending(tuple => tuple.Item2.ReleaseDate)));
        }

        [HttpGet("{action}/{page:int?}")]
        public async Task<IActionResult> Titles(ushort page)
        {
            var totalPages = Math.Ceiling(await _titlesManager.Count(_signInManager.IsSignedIn(User)) / (decimal) Constants.ItemsPerPage);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1);
            var titles = (await _titlesManager.GetRange(Constants.ItemsPerPage * (page - 1), Constants.ItemsPerPage, _signInManager.IsSignedIn(User)))
                .OrderBy(title => title.Status).ThenBy(title => title.Name).Select(title => Mapper.Map(title, null));
            
            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;

            return View(titles);
        }

        [HttpGet("{action:slugify}")]
        public IActionResult AboutUs() => View();

        [HttpGet("{action:slugify}")]
        public IActionResult ContactUs() => View();

        [HttpPost("{action:slugify}")]
        public async Task<IActionResult> ContactUs(ContactModel contactModel)
        {
            if (ModelState.IsValid)
            {
                if (await _emailSender.SendEmail(contactModel.Email, await _parametersManager.GetValue<string>(Parameter.Types.SiteEmail), string.Format(OtherMessages.MessageFrom, contactModel.Nickname), contactModel.Email))
                {
                    TempData["SuccessMessage"] = new[] {OtherMessages.MessageSentSuccessfully};
                    
                    return new JsonResult(new
                    {
                        success = true
                    });
                }

                ModelState.AddModelError("MessageNotSent", OtherMessages.MessageCouldntBeSent);
            }
            
            return new JsonResult(new
            {
                success = false,
                messages = ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
            });
        }

        [HttpGet("{action}/{titleId:Guid}/{page:int?}")]
        public async Task<IActionResult> Title(Guid titleId, ushort page = 1)
        {
            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return RedirectToAction("Titles");

            if (!_signInManager.IsSignedIn(User) && !title.Visible)
                return RedirectToAction("Titles");

            var totalPages = Math.Ceiling(await _chapterManager.Count(title, _signInManager.IsSignedIn(User)) / (decimal) Constants.ItemsPerPage);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1);
            var chapters = (await _chapterManager.GetRange(title, Constants.ItemsPerPage * (page - 1), Constants.ItemsPerPage, _signInManager.IsSignedIn(User))).Select(Mapper.Map);

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            
            return View("Title", new Tuple<TitleModel, IEnumerable<ChapterModel>>(Mapper.Map(title, await _titlesManager.GetTags(title)), chapters));
        }

        [HttpGet("Chapter/Read/{chapterId:Guid}")]
        public async Task<IActionResult> ChapterRead(Guid chapterId)
        {
            if (await _chapterManager.GetById(chapterId) is var chapter && chapter == null)
                return RedirectToAction("Titles");

            var title = await _titlesManager.GetById(chapter.TitleId);
            
            if (!_signInManager.IsSignedIn(User) && (!title.Visible || !chapter.Visible))
                return RedirectToAction("Titles");

            Request.Cookies.TryGetValue("reader_theme", out var value);
            ViewData["BodyClass"] = value switch
            {
                "light" => Constants.LightModeClasses,
                _ => Constants.DarkModeClasses
            };

            var chapters = (await _chapterManager.GetAll(title, _signInManager.IsSignedIn(User))).ToArray();
            ViewData["PreviousChapter"] = chapters.Where(entity => entity.Number < chapter.Number).OrderByDescending(entity => entity.Number).FirstOrDefault()?.Id;
            ViewData["NextChapter"] = chapters.Where(entity => entity.Number > chapter.Number).OrderBy(entity => entity.Number).FirstOrDefault()?.Id;
            ViewData["Chapters"] = chapters.OrderByDescending(entity => entity.Number).Select(Mapper.Map);

            return await GetReader(title, chapter);
        }
        
        [HttpGet("Chapter/Download/{chapterId:Guid}")]
        public async Task<IActionResult> ChapterDownload(Guid chapterId)
        {
            if (await _chapterManager.GetById(chapterId) is var chapter && chapter == null)
                return RedirectToAction("Titles");

            var title = await _titlesManager.GetById(chapter.TitleId);
            
            if (!_signInManager.IsSignedIn(User) && (!title.Visible || !chapter.Visible))
                return RedirectToAction("Titles");

            return GetDownload(title, chapter);
        }

        private IActionResult GetDownload(Title title, Chapter chapter) => chapter switch
        {
            ComicChapter comicChapter => File(_chapterArchiver.GetChapterDownload(comicChapter)?.OpenRead(), "application/zip", $"{GetDownloadName(title, comicChapter)}.zip"),
            _ => RedirectToAction("Title", new { titleId = chapter.TitleId })
        };
        
        private async Task<IActionResult> GetReader(Title title, Chapter chapter) => chapter switch
        {
            ComicChapter comicChapter => await GetComicReader((Comic) title, comicChapter),
            _ => RedirectToAction("Title", new { titleId = chapter.TitleId })
        };

        private async Task<IActionResult> GetComicReader(Comic comic, ComicChapter comicChapter)
        {
            var comicModel = Mapper.Map(comic, null);
            var comicChapterModel = Mapper.Map(comicChapter);
            var pages = (await _pagesManager.GetAll(comicChapter)).Select(page => (ComicPage) page).OrderBy(page => page.Number);

            Request.Cookies.TryGetValue($"{comic.Id}_long_strip", out var value);

            if (string.IsNullOrWhiteSpace(value))
                Response.Cookies.Append($"{comic.Id}_long_strip", comic.LongStrip.ToString().ToLower(), new CookieOptions{ MaxAge = TimeSpan.FromDays(365 * 10) });

            ViewData["LongStrip"] = Convert.ToBoolean(value) || comic.LongStrip;
            
            return View("ComicReader", new Tuple<ComicModel, ComicChapterModel, IEnumerable<ComicPage>>(comicModel, comicChapterModel, pages));
        }
        
        private static string GetDownloadName(Title title, Chapter chapter) => $"{title.Name} - {Labels.Chapter} {chapter.Number}";
    }
}