using System;
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
using Microsoft.Extensions.Caching.Memory;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Localization;
using WeebReader.Web.Models.Others;
using WeebReader.Web.Models.Others.Extensions;
using WeebReader.Web.Portal.Others;
using WeebReader.Web.Services;

namespace WeebReader.Web.Portal.Controllers
{
    public class ContentController : Controller
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly TitlesManager<Title> _titlesManager;
        private readonly ChapterManager<Chapter> _chapterManager;
        private readonly PagesManager<Page> _pagesManager;
        private readonly NovelChapterContentManager _novelChapterContentManager;
        private readonly ChapterArchiver<Chapter> _chapterArchiver;
        private readonly ParametersManager _parametersManager;
        private readonly IMemoryCache _memoryCache;

        public ContentController(SignInManager<IdentityUser<Guid>> signInManager, TitlesManager<Title> titlesManager, ChapterManager<Chapter> chapterManager, PagesManager<Page> pagesManager, NovelChapterContentManager novelChapterContentManager, ChapterArchiver<Chapter> chapterArchiver, ParametersManager parametersManager, IMemoryCache memoryCache)
        {
            _signInManager = signInManager;
            _titlesManager = titlesManager;
            _chapterManager = chapterManager;
            _pagesManager = pagesManager;
            _novelChapterContentManager = novelChapterContentManager;
            _chapterArchiver = chapterArchiver;
            _parametersManager = parametersManager;
            _memoryCache = memoryCache;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["TotalPages"] = Math.Ceiling(await CountReleases() / (decimal) Constants.ItemsPerPageReleases);
            
            return View(await GetReleases(0, Constants.ItemsPerPageReleases));
        }

        [HttpGet("{page:int}")]
        public async Task<IActionResult> Index(ushort page) => PartialView("Partials/Index", await GetReleases(Constants.ItemsPerPageReleases * (page - 1), Constants.ItemsPerPageReleases));

        [HttpGet("RSS")]
        public async Task<IActionResult> IndexRss()
        {
            const string key = "index_rss";
            
            if (_memoryCache.TryGetValue(key, out var value))
                return await GetRssFeed((SyndicationFeed) value);
            
            var feedItems = (await GetReleases(0, Constants.ItemsPerPageReleasesRss, false)).Select(tuple =>
            {
                var title = $"{tuple.title.Name} - {Labels.Chapter} {tuple.chapter.Number}";
                var description = tuple.title.Synopsis.RemoveHtmlTags() is var desc && desc.Length > 200 ? $"{desc.Substring(0, 200)}..." : desc;
                var url = new Uri(Url.Action("ReadChapter", "Content", new {chapterId = tuple.chapter.Id}, Request.Scheme));
                var feedItem = new SyndicationItem(title, description, url)
                {
                    Id = tuple.chapter.Id.ToString(),
                    PublishDate = tuple.chapter.ReleaseDate
                };
                
                feedItem.ElementExtensions.Add("titleLink", null, new Uri(Url.Action("Titles", "Content", new {titleId = tuple.title.Id}, Request.Scheme)));

                return feedItem;
            }).ToArray();
            
            var siteName = await _parametersManager.GetValue<string>(ParameterTypes.SiteName);
            var feed = new SyndicationFeed($"{siteName} RSS", $"{Labels.Home} - {siteName}", new Uri(Url.Action("Index", "Content", null, Request.Scheme)), feedItems)
            {
                BaseUri = new Uri(Url.Action("IndexRss", "Content", null, Request.Scheme)),
                ImageUrl = new Uri($"{Request.Scheme}://{Request.Host}/assets/icon.png"),
                LastUpdatedTime = DateTimeOffset.Now,
                TimeToLive = TimeSpan.FromMinutes(10)
            };

            _memoryCache.Set(key, feed, TimeSpan.FromMinutes(10));
            
            return await GetRssFeed(feed);
        }
        
        [HttpGet("{action}")]
        public async Task<IActionResult> Titles() => View((await _titlesManager.GetAll(_signInManager.IsSignedIn(User))).OrderBy(title => title.Status).ThenBy(title => title.Name));

        [HttpGet("{action}/{titleId:Guid}")]
        public async Task<IActionResult> Titles(Guid titleId)
        {
            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return RedirectToAction("Titles");

            if (!_signInManager.IsSignedIn(User) && !title.Visible)
                return RedirectToAction("Titles");

            if (title.Nsfw && !HasNsfwCookie()) 
                return View("NSFWTitleWarning", title);
            
            ViewData["TotalPages"] = Math.Ceiling(await _chapterManager.Count(title, _signInManager.IsSignedIn(User)) / (decimal) Constants.ItemsPerPageChapters);
            
            return View("Title", ValueTuple.Create(title, await _titlesManager.GetTags(title), await _chapterManager.GetRange(title, 0, Constants.ItemsPerPageChapters, _signInManager.IsSignedIn(User))));
        }

        [HttpGet("{action}/{titleId:Guid}/{page:int}")]
        public async Task<IActionResult> Titles(Guid titleId, ushort page)
        {
            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return NotFound();

            if (!_signInManager.IsSignedIn(User) && !title.Visible)
                return NotFound();

            if (page == 0)
                page = 1;

            return PartialView("Partials/TitleChapters", await _chapterManager.GetRange(title, Constants.ItemsPerPageChapters * (page - 1), Constants.ItemsPerPageChapters, _signInManager.IsSignedIn(User)));
        }
        
        [HttpGet("{action}/{titleId:Guid}/JSON")]
        public async Task<IActionResult> TitlesJson(Guid titleId)
        {
            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return NotFound();

            if (!_signInManager.IsSignedIn(User) && !title.Visible)
                return NotFound();
            
            return new JsonResult(new
            {
                success = true,
                chapters = (await _chapterManager.GetAll(title, _signInManager.IsSignedIn(User))).Select(chapter => new
                {
                    number = chapter.Number,
                    readAddress = Url.Action("ReadChapter", new {chapterId = chapter.Id})
                })
            });
        }

        [HttpGet("Titles/{titleId:Guid}/RSS")]
        public async Task<IActionResult> TitlesRss(Guid titleId)
        {
            var key = $"titles_rss_{titleId}";
            
            if (_memoryCache.TryGetValue(key, out var value))
                return await GetRssFeed((SyndicationFeed) value);
            
            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return NotFound();

            if (!_signInManager.IsSignedIn(User) && !title.Visible)
                return NotFound();
            
            var chapters = (await _chapterManager.GetRange(title, 0, Constants.ItemsPerPageChaptersRss, _signInManager.IsSignedIn(User))).ToArray();
            var feedItems = chapters.OrderByDescending(chapter => chapter.ReleaseDate).Select(chapter =>
            {
                var itemTitle = $"{title.Name} - {Labels.Chapter} {chapter.Number}";
                var description = title.Synopsis.RemoveHtmlTags() is var desc && desc.Length > 200 ? $"{desc.Substring(0, 200)}..." : desc;
                var url = new Uri(Url.Action("ReadChapter", "Content", new {chapterId = chapter.Id}, Request.Scheme));
                var feedItem = new SyndicationItem(itemTitle, description, url)
                {
                    Id = chapter.Id.ToString(),
                    PublishDate = chapter.ReleaseDate
                };

                return feedItem;
            }).ToArray();
            
            var siteName = await _parametersManager.GetValue<string>(ParameterTypes.SiteName);
            var feed = new SyndicationFeed($"{title.Name} RSS", $"{title.Name} - {siteName}", new Uri(Url.Action("Titles", "Content", new {titleId = title.Id}, Request.Scheme)), feedItems)
            {
                BaseUri = new Uri(Url.Action("TitlesRss", "Content", new {titleId = title.Id}, Request.Scheme)),
                ImageUrl = new Uri($"{Request.Scheme}://{Request.Host}/content/{title.Id}/cover_thumb.jpg"),
                LastUpdatedTime = DateTimeOffset.Now,
                TimeToLive = TimeSpan.FromMinutes(10)
            };

            _memoryCache.Set(key, feed, TimeSpan.FromMinutes(10));
            
            return await GetRssFeed(feed);
        }
        
        [HttpGet("Chapters/{chapterId:Guid}/Read")]
        public async Task<IActionResult> ReadChapter(Guid chapterId)
        {
            if (await _chapterManager.GetById(chapterId) is var chapter && chapter == null)
                return RedirectToAction("Titles");

            var title = await _titlesManager.GetById(chapter.TitleId);
            
            if (!_signInManager.IsSignedIn(User) && (!title!.Visible || !chapter.Visible))
                return RedirectToAction("Titles");

            Request.Cookies.TryGetValue("reader_theme", out var value);
            ViewData["BodyClass"] = value switch
            {
                "light" => Constants.LightModeClasses,
                _ => Constants.DarkModeClasses
            };
            
            if (title!.Nsfw && !HasNsfwCookie()) 
                return View("NSFWChapterWarning", (title, chapter));

            ViewData["PreviousChapter"] = (await _chapterManager.GetPreviousChapter(chapter, _signInManager.IsSignedIn(User)))?.Id;
            ViewData["NextChapter"] = (await _chapterManager.GetNextChapter(chapter, _signInManager.IsSignedIn(User)))?.Id;

            return await GetReader(title, chapter);
        }
        
        [HttpGet("Chapters/{chapterId:Guid}/Download")]
        public async Task<IActionResult> DownloadChapter(Guid chapterId)
        {
            if (await _chapterManager.GetById(chapterId) is var chapter && chapter == null)
                return RedirectToAction("Titles");

            var title = await _titlesManager.GetById(chapter.TitleId);
            
            if (!_signInManager.IsSignedIn(User) && (!title!.Visible || !chapter.Visible))
                return RedirectToAction("Titles");

            return GetDownload(title!, chapter);
        }
        
        private IActionResult GetDownload(Title title, Chapter chapter) => chapter switch
        {
            ComicChapter comicChapter => File(_chapterArchiver.GetChapterDownload(comicChapter).OpenRead(), "application/zip", $"{GetDownloadName(title, comicChapter)}.zip"),
            NovelChapter novelChapter => File(_chapterArchiver.GetChapterDownload(novelChapter).OpenRead(), "application/pdf", $"{GetDownloadName(title, novelChapter)}.pdf"),
            _ => RedirectToAction("Titles", new { titleId = chapter.TitleId })
        };
        
        private static string GetDownloadName(Title title, Chapter chapter) => $"{title.Name} - {Labels.Chapter} {chapter.Number}";

        private async Task<IActionResult> GetReader(Title title, Chapter chapter) => chapter switch
        {
            ComicChapter comicChapter => await GetComicReader((Comic) title, comicChapter),
            NovelChapter novelChapter => await GetNovelReader((Novel) title, novelChapter),
            _ => RedirectToAction("Titles", new { titleId = chapter.TitleId })
        };

        private async Task<IActionResult> GetComicReader(Comic comic, ComicChapter comicChapter)
        {
            var pages = (await _pagesManager.GetAll(comicChapter)).Select(page => (ComicPage) page).OrderBy(page => page.Number);

            Request.Cookies.TryGetValue($"{comic.Id}_long_strip", out var value);

            if (string.IsNullOrWhiteSpace(value))
                Response.Cookies.Append($"{comic.Id}_long_strip", comic.LongStrip.ToString().ToLower(), new CookieOptions{ MaxAge = TimeSpan.FromDays(365 * 10) });

            ViewData["LongStrip"] = Convert.ToBoolean(value) || comic.LongStrip;
            
            return View("ComicReader", ValueTuple.Create<Comic, ComicChapter, IEnumerable<ComicPage>>(comic, comicChapter, pages));
        }

        private async Task<IActionResult> GetNovelReader(Novel novel, NovelChapter novelChapter) => View("NovelReader", ValueTuple.Create(novel, novelChapter, await _novelChapterContentManager.GetContentByChapter(novelChapter)));

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
        
        private async Task<IEnumerable<(Title title, Chapter chapter)>> GetReleases(int skip, int take, bool? includeHidden = null)
        {
            includeHidden ??= _signInManager.IsSignedIn(User);
            var titles = (await _titlesManager.GetAll(includeHidden.Value)).AsQueryable();
            var chapters = (await _chapterManager.GetAll(includeHidden.Value)).AsQueryable();

            var releases = includeHidden.Value
                ? titles.Join(chapters, title => title.Id, chapter => chapter.TitleId, (title, chapter) => new {title, chapter}).OrderByDescending(tuple => tuple.chapter.ReleaseDate).Skip(skip).Take(take)
                : titles.Join(chapters, title => title.Id, chapter => chapter.TitleId, (title, chapter) => new {title, chapter}).Where(tuple => tuple.title.Visible && tuple.chapter.Visible)
                    .OrderByDescending(tuple => tuple.chapter.ReleaseDate).Skip(skip).Take(take);

            return releases.Select(tuple => ValueTuple.Create(tuple.title, tuple.chapter));
        }
        
        private bool HasNsfwCookie()
        {
            Request.Cookies.TryGetValue("seek_nsfw_content", out var value);

            if (string.IsNullOrWhiteSpace(value))
                value = false.ToString();

            return Convert.ToBoolean(value);
        }
    }
}