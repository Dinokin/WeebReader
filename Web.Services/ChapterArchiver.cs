using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using iText.Html2pdf;
using Microsoft.AspNetCore.Hosting;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Services.Others;

namespace WeebReader.Web.Services
{
    public class ChapterArchiver<TChapter> where TChapter : Chapter
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ChapterManager<TChapter> _chapterManager;
        private readonly PagesManager<Page> _pageManager;

        public ChapterArchiver(IWebHostEnvironment environment, ChapterManager<TChapter> chapterManager, PagesManager<Page> pageManager)
        {
            _environment = environment;
            _chapterManager = chapterManager;
            _pageManager = pageManager;
        }

        public async Task<bool> AddChapter(TChapter chapter, ZipArchive? pages)
        {
            if (!await _chapterManager.Add(chapter))
                return false;
            
            await ProcessContent(chapter, pages);

            return true;
        }

        public async Task<bool> EditChapter(TChapter chapter, ZipArchive? pages)
        {
            if (!await _chapterManager.Edit(chapter))
                return false; 
            
            await ProcessContent(chapter, pages, pages != null);

            return true;
        }

        public async Task<bool> DeleteChapter(TChapter chapter)
        {
            if (!await _chapterManager.Delete(chapter))
                return false;

            Utilities.GetChapterFolder(_environment, chapter.TitleId, chapter.Id).Delete(true);

            return true;
        }

        public FileInfo? GetChapterDownload(Chapter chapter) => chapter switch
        {
            ComicChapter _ => new FileInfo($"{Utilities.GetChapterFolder(_environment, chapter.TitleId, chapter.Id)}/package.zip"),
            NovelChapter _ => new FileInfo($"{Utilities.GetChapterFolder(_environment, chapter.TitleId, chapter.Id)}/chapter.pdf"),
            _ => throw new ArgumentException()
        };

        private async Task ProcessContent(TChapter chapter, ZipArchive? pages, bool deleteOld = false)
        {
            if (deleteOld)
            {
                await _pageManager.DeleteRange(await _pageManager.GetAll(chapter));

                Utilities.GetChapterFolder(_environment, chapter.TitleId, chapter.Id).Delete(true);
            }

            switch (chapter)
            {
                case ComicChapter comicChapter when pages != null:
                    await AddComicPages(comicChapter, pages);
                    return;
                case NovelChapter novelChapter:
                    await BuildPdf(novelChapter);
                    return;
                default:
                    return;
            }
        }

        private async Task AddComicPages(ComicChapter comicChapter, ZipArchive pages)
        {
            var location = Utilities.GetChapterFolder(_environment, comicChapter.TitleId, comicChapter.Id);
            var entries = pages.Entries.ToArray().Where(entry => ValidateExtension(entry.Name)).OrderBy(entry => entry.Name).ToArray();
            var comicPages = new List<ComicPage>();
            var files = new List<FileInfo>();
            var zipFile = new FileInfo($"{location}/package.zip");

            if (!location.Exists)
                location.Create();

            if (zipFile.Exists)
                zipFile.Delete();

            comicPages.AddRange(entries.Select((entry, i) => new ComicPage(IsAnimated(entry.Name), comicChapter.Id, Convert.ToUInt16(i))));
            await _pageManager.AddRange(comicPages);

            files.AddRange(comicPages.Select(page => page.Animated ? 
                Utilities.WriteAnimation(location, Utilities.ProcessAnimation(entries[page.Number].Open()), page.Id.ToString()) : 
                Utilities.WriteImage(location, Utilities.ProcessImage(entries[page.Number].Open()), page.Id.ToString())));

            using var zipArchive = new ZipArchive(zipFile.Create(), ZipArchiveMode.Create);

            for (var i = 0; i < files.Count; i++)
                zipArchive.CreateEntryFromFile($"{files[i]}", $"{i}{files[i].Extension}");
        }

        private Task BuildPdf(NovelChapter novelChapter)
        {
            var location = Utilities.GetChapterFolder(_environment, novelChapter.TitleId, novelChapter.Id);
            var pdfFile = new FileInfo($"{location}/chapter.pdf");
            
            if (!location.Exists)
                location.Create();

            if (pdfFile.Exists)
                pdfFile.Delete();

            HtmlConverter.ConvertToPdf(novelChapter.Content, pdfFile.Create());
            return Task.CompletedTask;
        }

        private static bool ValidateExtension(string fileName) => Path.GetExtension(fileName).ToLower() == ".png" || Path.GetExtension(fileName).ToLower() == ".jpg" || Path.GetExtension(fileName).ToLower() == ".jpeg" || Path.GetExtension(fileName).ToLower() == ".gif";

        private static bool IsAnimated(string fileName) => Path.GetExtension(fileName).ToLower() == ".gif";
    }
}