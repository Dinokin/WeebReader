﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Portal.Others.Extensions;

namespace WeebReader.Web.Portal.Controllers
{
    [ApiController]
    [Route("Api")]
    public class ApiController : Controller
    {
        private readonly TitlesManager<Title> _titlesManager;
        private readonly ChapterManager<Chapter> _chapterManager;
        private readonly PagesManager<Page> _pagesManager;

        public ApiController(TitlesManager<Title> titlesManager, ChapterManager<Chapter> chapterManager, PagesManager<Page> pagesManager)
        {
            _titlesManager = titlesManager;
            _chapterManager = chapterManager;
            _pagesManager = pagesManager;
        }

        [HttpGet("Titles")]
        public async Task<IActionResult> Titles()
        {
            var titles = new List<object>();
            
            foreach (var title in (await _titlesManager.GetAll(false)).ToArray())
                titles.Add(new
                {
                    title.Id,
                    title.Name,
                    Type = title.GetType().Name,
                    title.Author,
                    title.Artist,
                    title.Status,
                    title.Nsfw,
                    CoverUrl = $"/content/{title.Id}/cover.png",
                    UpdatedAt = (await _chapterManager.GetLatestChapter(title, false))?.ReleaseDate
                });

            return Json(titles);
        }

        [HttpGet("Titles/{titleId:guid}")]
        public async Task<IActionResult> Chapters(Guid titleId)
        {
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
                Synopsis = title.Synopsis.RemoveHtmlTags(),
                title.Status,
                title.Nsfw,
                CoverUrl = $"/content/{title.Id}/cover.png",
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

            return Json(result);
        }

        [HttpGet("Chapters/{chapterId:guid}")]
        public async Task<IActionResult> Content(Guid chapterId)
        {
            if (await _chapterManager.GetById(chapterId) is var chapter && chapter == null)
                return NotFound(); 
            
            if (await _titlesManager.GetById(chapter.TitleId) is var title && title == null)
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
                        pageUrl = $"/content/{comicChapter.TitleId}/{comicChapter.Id}/{page.Id}{(page.Animated ? ".gif" : ".png")}"
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
                    novelChapter.Content,
                    Pages = (await _pagesManager.GetAll(novelChapter)).Select(page => new
                    {
                        page.Id,
                        pageUrl = $"/content/{novelChapter.TitleId}/{novelChapter.Id}/{page.Id}{(page.Animated ? ".gif" : ".png")}"
                    }).ToArray()
                },
                _ => throw new ArgumentException($"{chapter.GetType().Name} is not a supported type.")
            };
            
            return Json(result);
        }

        [HttpGet("Titles/Search")]
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
                return Json(Array.Empty<object>());
            
            if (term.Length > 200)
                term = term[..200];

            term = term.ToLower();

            var titles = new List<object>();

            foreach (var title in (await _titlesManager.Search(term, false)).ToArray())
                titles.Add(new
                {
                    title.Id,
                    title.Name,
                    Type = title.GetType().Name,
                    title.Author,
                    title.Artist,
                    title.Status,
                    title.Nsfw,
                    CoverUrl = $"/content/{title.Id}/cover.png",
                    UpdatedAt = (await _chapterManager.GetLatestChapter(title, false))?.ReleaseDate
                });
            
            return Json(titles);
        }
    }
}