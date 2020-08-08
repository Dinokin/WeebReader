using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Web.Localization;

namespace WeebReader.Web.Portal.Controllers
{
    public partial class HomeController
    {
        private IActionResult GetDownload(Title title, Chapter chapter) => chapter switch
        {
            ComicChapter comicChapter => File(_chapterArchiver.GetChapterDownload(comicChapter)?.OpenRead(), "application/zip", $"{GetDownloadName(title, comicChapter)}.zip"),
            NovelChapter novelChapter => File(_chapterArchiver.GetChapterDownload(novelChapter)?.OpenRead(), "application/pdf", $"{GetDownloadName(title, novelChapter)}.pdf"),
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
        
        private bool HasNsfwCookie()
        {
            Request.Cookies.TryGetValue("seek_nsfw_content", out var value);

            if (string.IsNullOrWhiteSpace(value))
                value = false.ToString();

            return Convert.ToBoolean(value);
        }
    }
}