﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
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
        private readonly ReCaptchaValidator _reCaptchaValidator;

        public HomeController(SignInManager<IdentityUser<Guid>> signInManager, TitlesManager<Title> titlesManager, ChapterManager<Chapter> chapterManager, ChapterArchiver<Chapter> chapterArchiver, PagesManager<Page> pagesManager,
            PostsManager postsManager, EmailSender emailSender, ParametersManager parametersManager, ReCaptchaValidator reCaptchaValidator)
        {
            _signInManager = signInManager;
            _titlesManager = titlesManager;
            _chapterManager = chapterManager;
            _chapterArchiver = chapterArchiver;
            _pagesManager = pagesManager;
            _postsManager = postsManager;
            _emailSender = emailSender;
            _parametersManager = parametersManager;
            _reCaptchaValidator = reCaptchaValidator;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            const ushort itemsPerPage = 8;

            ViewData["Page"] = 1;
            ViewData["TotalPages"] = Math.Ceiling(await CountReleases() / (decimal) itemsPerPage);
            
            return View((await GetReleases(0, itemsPerPage)).Select(tuple => new Tuple<TitleModel, ChapterModel>(Mapper.Map(tuple.title), Mapper.Map(tuple.chapter))));
        }

        [HttpGet("JSON/{page:int?}")]
        public async Task<IActionResult> IndexJson(ushort page = 1)
        {
            const ushort itemsPerPage = 8;
            
            var totalPages = Math.Ceiling(await CountReleases() / (decimal) itemsPerPage);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1);

            return new JsonResult(new
            {
                success = true,
                page,
                totalPages,
                releases = (await GetReleases(itemsPerPage * (page - 1), itemsPerPage)).Select(tuple => new
                {
                    titleId = tuple.title.Id,
                    chapterId = tuple.chapter.Id,
                    titleName = tuple.title.Name,
                    chapterNumber = tuple.chapter.Number,
                    releaseDate = tuple.chapter.ReleaseDate,
                    titleAddress = Url.Action("Titles", new {titleId = tuple.title.Id}),
                    chapterAddress = Url.Action("ReadChapter", new {chapterId = tuple.chapter.Id})
                })
            });
        }

        [HttpGet("RSS")]
        public async Task<IActionResult> IndexRss()
        {
            var feedItems = (await GetReleases(0, 8)).Select(tuple =>
            {
                var title = $"{tuple.title.Name} - {Labels.Chapter} {tuple.chapter.Number}";
                var description = tuple.title.Synopsis.RemoveHtmlTags();
                var url = new Uri(Url.Action("ReadChapter", "Home", new {chapterId = tuple.chapter.Id}, Request.Scheme));
                var feedItem = new SyndicationItem(title, description, url)
                {
                    Id = tuple.chapter.Id.ToString(),
                    PublishDate = tuple.chapter.ReleaseDate
                };

                return feedItem;
            });
            
            
            var siteName = await _parametersManager.GetValue<string>(Parameter.Types.SiteName);
            var feed = new SyndicationFeed($"{siteName} RSS", $"{Labels.LatestReleases} - {siteName}", new Uri(Url.Action("Index", "Home", null, Request.Scheme)), feedItems)
            {
                BaseUri = new Uri(Url.Action("IndexRss", "Home", null, Request.Scheme)),
                ImageUrl = new Uri($"{Request.Scheme}://{Request.Host}/assets/icon.png"),
                LastUpdatedTime = DateTimeOffset.Now,
                TimeToLive = TimeSpan.FromMinutes(1)
            };

            return await GetRssFeed(feed);
        }

        [HttpGet("{action}/{page:int?}")]
        public async Task<IActionResult> Blog(ushort page = 1)
        {
            if (!await _parametersManager.GetValue<bool>(Parameter.Types.PageBlogEnabled))
                return RedirectToAction("Index");
            
            const ushort itemsPerPage = 5;

            var totalPages = Math.Ceiling(await _postsManager.Count(_signInManager.IsSignedIn(User)) / (decimal) itemsPerPage);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1);
            var posts = (await _postsManager.GetRange(itemsPerPage * (page - 1), itemsPerPage, _signInManager.IsSignedIn(User))).Select(Mapper.Map).OrderByDescending(post => post.ReleaseDate);

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;

            return View(posts);

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
        
        [HttpGet("{action}/{page:int?}")]
        public async Task<IActionResult> Titles() => View((await _titlesManager.GetAll(_signInManager.IsSignedIn(User))).OrderBy(title => title.Status).ThenBy(title => title.Name).Select(title => Mapper.Map(title)));

        [HttpGet("{action}/{titleId:Guid}/{page:int?}")]
        public async Task<IActionResult> Titles(Guid titleId, ushort page = 1)
        {
            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return RedirectToAction("Titles");

            if (!_signInManager.IsSignedIn(User) && !title.Visible)
                return RedirectToAction("Titles");

            const ushort itemsPerPage = 50;
            
            var totalPages = Math.Ceiling(await _chapterManager.Count(title, _signInManager.IsSignedIn(User)) / (decimal) itemsPerPage);
            page = (ushort) (page >= 1 && page <= totalPages ? page : 1);
            var chapters = (await _chapterManager.GetRange(title, itemsPerPage * (page - 1), itemsPerPage, _signInManager.IsSignedIn(User))).Select(Mapper.Map);

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            
            return View("Title", new Tuple<TitleModel, IEnumerable<ChapterModel>>(Mapper.Map(title, await _titlesManager.GetTags(title)), chapters));
        }

        [HttpGet("Titles/{titleId:Guid}/RSS")]
        public async Task<IActionResult> TitlesRss(Guid titleId)
        {
            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return RedirectToAction("IndexRss");

            if (!_signInManager.IsSignedIn(User) && !title.Visible)
                return RedirectToAction("IndexRss");
            
            var titleModel = Mapper.Map(title);
            var chapters = (await _chapterManager.GetRange(title, 0, 25, _signInManager.IsSignedIn(User))).Select(Mapper.Map).ToArray();
            var feedItems = chapters.OrderByDescending(chapter => chapter.ReleaseDate).Select(chapter =>
            {
                var itemTitle = $"{titleModel.Name} - {Labels.Chapter} {chapter.Number}";
                var description = titleModel.Synopsis.RemoveHtmlTags();
                var url = new Uri(Url.Action("ReadChapter", "Home", new {chapterId = chapter.ChapterId}, Request.Scheme));
                var feedItem = new SyndicationItem(itemTitle, description, url)
                {
                    Id = chapter.ChapterId.ToString(),
                    PublishDate = chapter.ReleaseDate ?? DateTime.Now
                };

                return feedItem;
            });
            
            var siteName = await _parametersManager.GetValue<string>(Parameter.Types.SiteName);
            var feed = new SyndicationFeed($"{titleModel.Name} RSS", $"{titleModel.Name} - {siteName}", new Uri(Url.Action("Titles", "Home", new {titleId = titleModel.TitleId}, Request.Scheme)), feedItems)
            {
                BaseUri = new Uri(Url.Action("TitlesRss", "Home", new {titleId = titleModel.TitleId}, Request.Scheme)),
                ImageUrl = new Uri($"{Request.Scheme}://{Request.Host}/content/{titleModel.TitleId}/cover_thumb.jpg"),
                LastUpdatedTime = DateTimeOffset.Now,
                TimeToLive = TimeSpan.FromMinutes(1)
            };

            return await GetRssFeed(feed);
        }

        [HttpGet("Chapters/{chapterId:Guid}/Read")]
        public async Task<IActionResult> ReadChapter(Guid chapterId)
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
        
        [HttpGet("Chapters/{chapterId:Guid}/Download")]
        public async Task<IActionResult> DownloadChapter(Guid chapterId)
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
            _ => RedirectToAction("Titles", new { titleId = chapter.TitleId })
        };
        
        private async Task<IActionResult> GetReader(Title title, Chapter chapter) => chapter switch
        {
            ComicChapter comicChapter => await GetComicReader((Comic) title, comicChapter),
            _ => RedirectToAction("Titles", new { titleId = chapter.TitleId })
        };

        private async Task<IActionResult> GetComicReader(Comic comic, ComicChapter comicChapter)
        {
            var comicModel = Mapper.Map(comic);
            var comicChapterModel = Mapper.Map(comicChapter);
            var pages = (await _pagesManager.GetAll(comicChapter)).Select(page => (ComicPage) page).OrderBy(page => page.Number);

            Request.Cookies.TryGetValue($"{comic.Id}_long_strip", out var value);

            if (string.IsNullOrWhiteSpace(value))
                Response.Cookies.Append($"{comic.Id}_long_strip", comic.LongStrip.ToString().ToLower(), new CookieOptions{ MaxAge = TimeSpan.FromDays(365 * 10) });

            ViewData["LongStrip"] = Convert.ToBoolean(value) || comic.LongStrip;
            
            return View("ComicReader", new Tuple<ComicModel, ComicChapterModel, IEnumerable<ComicPage>>(comicModel, comicChapterModel, pages));
        }

        private async Task<IActionResult> GetRssFeed(SyndicationFeed feed)
        {
            var stream = new MemoryStream();
            using var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings {Encoding = Encoding.UTF8, NewLineHandling = NewLineHandling.Entitize, Indent = true, Async = true});
            new Rss20FeedFormatter(feed, false).WriteTo(xmlWriter);
            await xmlWriter.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/rss+xml; charset=utf-8");
        }

        private async Task<long> CountReleases()
        {
            var includeHidden = _signInManager.IsSignedIn(User);
            var titles = (await _titlesManager.GetAll(includeHidden)).AsQueryable();
            var chapters = (await _chapterManager.GetAll(includeHidden)).AsQueryable();

            return includeHidden
                ? titles.Join(chapters, title => title.Id, chapter => chapter.TitleId, (title, chapter) => new {title, chapter})
                    .OrderByDescending(tuple => tuple.chapter.ReleaseDate).LongCount()
                : titles.Join(chapters, title => title.Id, chapter => chapter.TitleId, (title, chapter) => new {title, chapter}).Where(tuple => tuple.title.Visible && tuple.chapter.Visible)
                    .OrderByDescending(tuple => tuple.chapter.ReleaseDate).LongCount();
        }
        
        private async Task<IEnumerable<(Title title, Chapter chapter)>> GetReleases(int skip, int take)
        {
            var includeHidden = _signInManager.IsSignedIn(User);
            var titles = (await _titlesManager.GetAll(includeHidden)).AsQueryable();
            var chapters = (await _chapterManager.GetAll(includeHidden)).AsQueryable();

            var releases = includeHidden
                ? titles.Join(chapters, title => title.Id, chapter => chapter.TitleId, (title, chapter) => new {title, chapter}).OrderByDescending(tuple => tuple.chapter.ReleaseDate).Skip(skip).Take(take)
                : titles.Join(chapters, title => title.Id, chapter => chapter.TitleId, (title, chapter) => new {title, chapter}).Where(tuple => tuple.title.Visible && tuple.chapter.Visible)
                    .OrderByDescending(tuple => tuple.chapter.ReleaseDate).Skip(skip).Take(take);

            return releases.Select(tuple => ValueTuple.Create(tuple.title, tuple.chapter));
        }
        
        private static string GetDownloadName(Title title, Chapter chapter) => $"{title.Name} - {Labels.Chapter} {chapter.Number}";
    }
}