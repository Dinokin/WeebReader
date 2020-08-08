using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Web.Localization;
using WeebReader.Web.Portal.Others;

namespace WeebReader.Web.Portal.Controllers
{
    public partial class HomeController
    {
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["CurrentPage"] = 1;
            ViewData["TotalPages"] = Math.Ceiling(await CountReleases() / (decimal) Constants.ItemsPerPageReleases);
            
            return View(await GetReleases(0, Constants.ItemsPerPageReleases));
        }

        [HttpGet("{page:int}")]
        public async Task<IActionResult> Index(ushort page) => PartialView("Partials/Index", await GetReleases(Constants.ItemsPerPageReleases * (page - 1), Constants.ItemsPerPageReleases));

        [HttpGet("RSS")]
        public async Task<IActionResult> IndexRss()
        {
            var feedItems = (await GetReleases(0, Constants.ItemsPerPageReleasesRss)).Select(tuple =>
            {
                var title = $"{tuple.title.Name} - {Labels.Chapter} {tuple.chapter.Number}";
                var description = tuple.title.Synopsis.RemoveHtmlTags() is var desc && desc.Length > 200 ? $"{desc.Substring(0, 200)}..." : desc;
                var url = new Uri(Url.Action("ReadChapter", "Home", new {chapterId = tuple.chapter.Id}, Request.Scheme));
                var feedItem = new SyndicationItem(title, description, url)
                {
                    Id = tuple.chapter.Id.ToString(),
                    PublishDate = tuple.chapter.ReleaseDate
                };
                
                feedItem.ElementExtensions.Add("titleLink", null, new Uri(Url.Action("Titles", "Home", new {titleId = tuple.title.Id}, Request.Scheme)));

                return feedItem;
            });
            
            var siteName = await _parametersManager.GetValue<string>(Parameter.Types.SiteName);
            var feed = new SyndicationFeed($"{siteName} RSS", $"{Labels.Home} - {siteName}", new Uri(Url.Action("Index", "Home", null, Request.Scheme)), feedItems)
            {
                BaseUri = new Uri(Url.Action("IndexRss", "Home", null, Request.Scheme)),
                ImageUrl = new Uri($"{Request.Scheme}://{Request.Host}/assets/icon.png"),
                LastUpdatedTime = DateTimeOffset.Now,
                TimeToLive = TimeSpan.FromMinutes(1)
            };

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
            
            ViewData["CurrentPage"] = 1;
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
        
        [HttpGet("{action}/{titleId:Guid}/JSON/{page:int?}")]
        public async Task<IActionResult> TitlesJson(Guid titleId, ushort? page)
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
            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return NotFound();

            if (!_signInManager.IsSignedIn(User) && !title.Visible)
                return NotFound();
            
            var chapters = (await _chapterManager.GetRange(title, 0, Constants.ItemsPerPageChaptersRss, _signInManager.IsSignedIn(User))).ToArray();
            var feedItems = chapters.OrderByDescending(chapter => chapter.ReleaseDate).Select(chapter =>
            {
                var itemTitle = $"{title.Name} - {Labels.Chapter} {chapter.Number}";
                var description = title.Synopsis.RemoveHtmlTags() is var desc && desc.Length > 200 ? $"{desc.Substring(0, 200)}..." : desc;
                var url = new Uri(Url.Action("ReadChapter", "Home", new {chapterId = chapter.Id}, Request.Scheme));
                var feedItem = new SyndicationItem(itemTitle, description, url)
                {
                    Id = chapter.Id.ToString(),
                    PublishDate = chapter.ReleaseDate
                };

                return feedItem;
            });
            
            var siteName = await _parametersManager.GetValue<string>(Parameter.Types.SiteName);
            var feed = new SyndicationFeed($"{title.Name} RSS", $"{title.Name} - {siteName}", new Uri(Url.Action("Titles", "Home", new {titleId = title.Id}, Request.Scheme)), feedItems)
            {
                BaseUri = new Uri(Url.Action("TitlesRss", "Home", new {titleId = title.Id}, Request.Scheme)),
                ImageUrl = new Uri($"{Request.Scheme}://{Request.Host}/content/{title.Id}/cover_thumb.jpg"),
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
            
            if (title.Nsfw && !HasNsfwCookie()) 
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
            
            if (!_signInManager.IsSignedIn(User) && (!title.Visible || !chapter.Visible))
                return RedirectToAction("Titles");

            return GetDownload(title, chapter);
        }
    }
}