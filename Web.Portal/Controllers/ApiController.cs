using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;

namespace WeebReader.Web.Portal.Controllers
{
    [ApiController]
    [Route("Api")]
    public class ApiController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly TitlesManager<Title> _titlesManager;
        private readonly ChapterManager<Chapter> _chapterManager;
        private readonly PagesManager<Page> _pagesManager;
        private readonly NovelChapterContentManager _novelChapterContentManager;

        public ApiController(IMemoryCache memoryCache, TitlesManager<Title> titlesManager, ChapterManager<Chapter> chapterManager, PagesManager<Page> pagesManager, NovelChapterContentManager novelChapterContentManager)
        {
            _memoryCache = memoryCache;
            _titlesManager = titlesManager;
            _chapterManager = chapterManager;
            _pagesManager = pagesManager;
            _novelChapterContentManager = novelChapterContentManager;
        }

        [HttpGet("Titles")]
        public async Task<IActionResult> Titles()
        {
            var result = await _memoryCache.GetOrCreateAsync("titles", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var titles = new List<object>();

                foreach (var title in (await _titlesManager.GetAll(false)).ToArray())
                    titles.Add(new
                    {
                        title.Id,
                        title.Name,
                        Type = title.GetType().Name,
                        title.Author,
                        title.Artist,
                        title.Synopsis,
                        CoverUrl = $"/content/{title.Id}/cover.png?v={title.Version}",
                        UpdatedAt = (await _chapterManager.GetLatestChapter(title, false))?.ReleaseDate,
                        Tags = (await _titlesManager.GetTags(title)).Select(tag => tag.Name).ToArray()
                    });

                return titles;
            });
            
            return new JsonResult(result);
        }

        [HttpGet("Titles/{titleId:guid}")]
        public async Task<IActionResult> Chapters(Guid titleId)
        {
            var key = $"title_{titleId}";
            
            if (_memoryCache.TryGetValue(key, out var value))
                return new JsonResult(value);

            var title = await _titlesManager.GetById(titleId);

            if (title == null || !title.Visible)
                return NotFound();

            var result = new
            {
                title.Id,
                title.Name,
                Type = title.GetType().Name,
                title.Author,
                title.Artist,
                title.Synopsis,
                CoverUrl = $"/content/{title.Id}/cover.png?v={title.Version}",
                UpdatedAt = (await _chapterManager.GetLatestChapter(title, false))?.ReleaseDate,
                Tags = (await _titlesManager.GetTags(title)).Select(tag => tag.Name).ToArray(),
                Chapters = (await _chapterManager.GetAll(title, false)).Select(chapter => new
                {
                    chapter.Id,
                    chapter.Volume,
                    chapter.Number,
                    chapter.Name,
                    chapter.ReleaseDate
                }).ToArray()
            };

            _memoryCache.Set(key, result, TimeSpan.FromMinutes(10));
            return new JsonResult(result);
        }

        [HttpGet("Titles/{titleId:guid}/Chapters/{chapterId:guid}")]
        public async Task<IActionResult> Content(Guid titleId, Guid chapterId)
        {
            var key = $"title_{titleId}_chapter_{chapterId}";
            
            if (_memoryCache.TryGetValue(key, out var value))
                return new JsonResult(value);

            if (await _titlesManager.GetById(titleId) is var title && title == null)
                return NotFound();
            
            if (await _chapterManager.GetById(chapterId) is var chapter && chapter == null)
                return NotFound(); 
            
            if (!title.Visible || !chapter.Visible)
                return NotFound();

            object result = chapter switch
            {
                ComicChapter comicChapter => new
                {
                    chapter.Id,
                    chapter.Volume,
                    chapter.Number,
                    chapter.Name,
                    chapter.ReleaseDate,
                    chapter.TitleId,
                    Pages = (await _pagesManager.GetAll(comicChapter)).Select(page => (ComicPage) page).OrderBy(page => page.Number).Select(page => new
                    {
                        page.Id,
                        page.Number,
                        Format = page.Animated ? ".gif" : ".png"
                    }).ToArray()
                },
                NovelChapter novelChapter => new
                {
                    chapter.Id,
                    chapter.Volume,
                    chapter.Number,
                    chapter.Name,
                    chapter.ReleaseDate,
                    chapter.TitleId,
                    (await _novelChapterContentManager.GetContentByChapter(novelChapter)).Content,
                    Pages = (await _pagesManager.GetAll(novelChapter)).Select(page => new
                    {
                        page.Id,
                        Format = page.Animated ? ".gif" : ".png"
                    }).ToArray()
                },
                _ => throw new ArgumentException()
            };
            
            _memoryCache.Set(key, result, TimeSpan.FromMinutes(10));
            return new JsonResult(result);
        }
    }
}