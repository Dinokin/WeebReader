using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using iText.Html2pdf;
using Microsoft.AspNetCore.Hosting;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Services.Others;

namespace WeebReader.Web.Services
{
    public class ChapterArchiver<TChapter> where TChapter : Chapter
    {
        private readonly IWebHostEnvironment _environment;
        private readonly BaseContext _context;
        private readonly ChapterManager<TChapter> _chapterManager;
        private readonly PagesManager<Page> _pageManager;

        public ChapterArchiver(IWebHostEnvironment environment, BaseContext context, ChapterManager<TChapter> chapterManager, PagesManager<Page> pageManager)
        {
            _environment = environment;
            _context = context;
            _chapterManager = chapterManager;
            _pageManager = pageManager;
        }

        public async Task<bool> AddChapter(TChapter chapter, ZipArchive? pages)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            
            if (!await _chapterManager.Add(chapter))
            {
                await transaction.RollbackAsync();
                
                return false;
            }
            
            await ProcessContent(chapter, pages);
            await transaction.CommitAsync();
            
            return true;
        }

        public async Task<bool> EditChapter(TChapter chapter, ZipArchive? pages)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (!await _chapterManager.Edit(chapter))
            {
                await transaction.RollbackAsync();

                return false;
            }
            
            await ProcessContent(chapter, pages, pages != null);
            await transaction.CommitAsync();

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
                    await ProcessComicChapter(comicChapter, pages);
                    return;
                case NovelChapter novelChapter:
                    await ProcessNovelChapter(novelChapter);
                    return;
                default:
                    return;
            }
        }

        private async Task ProcessComicChapter(ComicChapter comicChapter, ZipArchive pages)
        {
            var location = Utilities.GetChapterFolder(_environment, comicChapter.TitleId, comicChapter.Id);
            var entries = pages.Entries.Where(entry => ValidateExtension(entry.Name, false)).OrderBy(entry => entry.Name).ToArray();
            var comicPages = new List<ComicPage>();
            var files = new List<FileInfo>();
            var zipFile = new FileInfo($"{location}/package.zip");

            if (!location.Exists)
                location.Create();

            if (zipFile.Exists)
                zipFile.Delete();

            comicPages.AddRange(entries.Select((entry, i) => new ComicPage(IsAnimated(entry.Name, false), comicChapter.Id, Convert.ToUInt16(i))));
            await _pageManager.AddRange(comicPages);

            files.AddRange(comicPages.Select(page => page.Animated ? 
                Utilities.WriteAnimation(location, Utilities.ProcessAnimation(entries[page.Number].Open()), page.Id.ToString()) : 
                Utilities.WriteImage(location, Utilities.ProcessImage(entries[page.Number].Open()), page.Id.ToString())));

            using var zipArchive = new ZipArchive(zipFile.Create(), ZipArchiveMode.Create);

            for (var i = 0; i < files.Count; i++)
                zipArchive.CreateEntryFromFile($"{files[i]}", $"{i}{files[i].Extension}", CompressionLevel.NoCompression);
        }

        private async Task ProcessNovelChapter(NovelChapter novelChapter)
        {
            var location = Utilities.GetChapterFolder(_environment, novelChapter.TitleId, novelChapter.Id);
            var pdfFile = new FileInfo($"{location}/chapter.pdf");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(novelChapter.Content);
            var imageNodes = htmlDocument.DocumentNode.SelectNodes("//img").ToArray();

            foreach (var node in imageNodes)
            {
                var src = node.GetAttributeValue("src", string.Empty);

                if (!ValidateExtension(src, true))
                    node.Remove();
            }

            if (!location.Exists)
                location.Create();

            if (pdfFile.Exists)
                pdfFile.Delete();

            HtmlConverter.ConvertToPdf(htmlDocument.DocumentNode.OuterHtml, pdfFile.Create());
            
            imageNodes = htmlDocument.DocumentNode.SelectNodes("//img").ToArray();
            var base64Images = imageNodes.Select(node => node.GetAttributeValue("src", string.Empty)).ToArray();
            var pages = base64Images.Select(base64Image => new NovelPage(IsAnimated(base64Image, true), novelChapter.Id)).ToArray();

            await _pageManager.AddRange(pages);

            Parallel.For(0, base64Images.Length, i =>
            {
                var stream = new MemoryStream(Convert.FromBase64String(base64Images[i].Substring(base64Images[i].IndexOf(',') + 1)));

                if (pages[i].Animated)
                    Utilities.WriteAnimation(location, Utilities.ProcessAnimation(stream), pages[i].Id.ToString());
                else
                    Utilities.WriteImage(location, Utilities.ProcessImage(stream), pages[i].Id.ToString());
            });

            for (var i = 0; i < imageNodes.Length; i++)
                imageNodes[i].SetAttributeValue("src", $"/content/{novelChapter.TitleId}/{novelChapter.Id}/{pages[i].Id}{(pages[i].Animated ? ".gif" : ".png")}");
            
            novelChapter.Content = htmlDocument.DocumentNode.OuterHtml;
            await _chapterManager.Edit((TChapter) (Chapter) novelChapter);
        }

        private static bool ValidateExtension(string content, bool base64)
        {
            if (base64)
                return Regex.IsMatch(content, "^data:image/(png|jpg|jpeg|gif);base64,.*");

            return Path.GetExtension(content).ToLower() == ".png" || Path.GetExtension(content).ToLower() == ".jpg" || Path.GetExtension(content).ToLower() == ".jpeg" || Path.GetExtension(content).ToLower() == ".gif";
        }

        private static bool IsAnimated(string content, bool base64) => base64 ? Regex.Match(content, "^data:image/(png|jpg|jpeg|gif);base64,.*").Value == "gif" : Path.GetExtension(content).ToLower() == ".gif";
    }
}