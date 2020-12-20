using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
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
    public class ChapterArchiver
    {
        private readonly IWebHostEnvironment _environment;
        private readonly BaseContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ChapterManager<Chapter> _chapterManager;
        private readonly PagesManager<Page> _pageManager;

        public ChapterArchiver(IWebHostEnvironment environment, BaseContext context, IHttpClientFactory httpClientFactory, ChapterManager<Chapter> chapterManager, PagesManager<Page> pageManager)
        {
            _environment = environment;
            _context = context;
            _httpClientFactory = httpClientFactory;
            _chapterManager = chapterManager;
            _pageManager = pageManager;
        }

        public async Task<bool> AddChapter(Chapter chapter, byte[]? content)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            
            if (!await _chapterManager.Add(chapter))
            {
                await transaction.RollbackAsync();
                
                return false;
            }
            
            await ProcessContent(chapter, content);
            await transaction.CommitAsync();
            
            return true;
        }

        public async Task<bool> EditChapter(Chapter chapter, byte[]? content)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (!await _chapterManager.Edit(chapter))
            {
                await transaction.RollbackAsync();

                return false;
            }
            
            await ProcessContent(chapter, content);
            await transaction.CommitAsync();

            return true;
        }

        public async Task<bool> DeleteChapter(Chapter chapter)
        {
            if (!await _chapterManager.Delete(chapter))
                return false;

            Utilities.GetChapterFolder(_environment, chapter.TitleId, chapter.Id).Delete(true);

            return true;
        }

        public FileInfo GetChapterDownload(Chapter chapter) => chapter switch
        {
            ComicChapter _ => new FileInfo($"{Utilities.GetChapterFolder(_environment, chapter.TitleId, chapter.Id)}/package.zip"),
            NovelChapter _ => new FileInfo($"{Utilities.GetChapterFolder(_environment, chapter.TitleId, chapter.Id)}/chapter.pdf"),
            _ => throw new ArgumentException()
        };

        public async Task<bool> IsValidComicChapterContent(byte[] content)
        {
            if (!content.Any())
                return false;

            await using var memoryStream = new MemoryStream(content);
            var zipArchive = new ZipArchive(memoryStream);

            return zipArchive.Entries.Any(entry => HasValidExtension(entry.Name, false));
        }

        private async Task ProcessContent(Chapter chapter, byte[]? content)
        {
            switch (chapter)
            {
                case ComicChapter comicChapter when content?.Length > 0:
                    var chapterFolder = Utilities.GetChapterFolder(_environment, chapter.TitleId, chapter.Id);
                    var oldFiles = new List<FileInfo>();

                    if (chapterFolder.Exists)
                    {
                        oldFiles.AddRange(chapterFolder.GetFiles().Where(file => HasValidExtension(file.Name, false)));
                        await _pageManager.DeleteRange(chapter);
                    }

                    await ProcessComicChapter(comicChapter, content);
                    Parallel.ForEach(oldFiles, file => file.Delete());
                    return;
                case NovelChapter novelChapter when content?.Length > 0:
                    await ProcessNovelChapter(novelChapter, content);
                    return;
                default:
                    return;
            }
        }

        private async Task ProcessComicChapter(ComicChapter comicChapter, byte[] pages)
        {
            using var zippedPages = new ZipArchive(new MemoryStream(pages));
            
            var location = Utilities.GetChapterFolder(_environment, comicChapter.TitleId, comicChapter.Id);
            var entries = zippedPages.Entries.Where(entry => HasValidExtension(entry.Name, false))
                .OrderBy(entry => Regex.Replace(entry.Name, @"\d+", match => match.Value.PadLeft(10, '0'))).ToArray();
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

        private async Task ProcessNovelChapter(NovelChapter novelChapter, byte[] content)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var location = Utilities.GetChapterFolder(_environment, novelChapter.TitleId, novelChapter.Id);
            var pdfFile = new FileInfo($"{location}/chapter.pdf");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(Encoding.Default.GetString(content));
            var imageNodes = htmlDocument.DocumentNode.SelectNodes("//img")?.ToArray() ?? new HtmlNode[0];

            foreach (var node in imageNodes)
            {
                var src = node.GetAttributeValue("src", string.Empty);

                if (HasValidExtension(src, false))
                {
                    if (Uri.IsWellFormedUriString(src, UriKind.Absolute))
                    {
                        try
                        {
                            var image = $"data:image/{(IsAnimated(src, false) ? "gif" : "png")};base64,";
                            image += Convert.ToBase64String(await httpClient.GetByteArrayAsync(src));
                            node.SetAttributeValue("src", image);
                        }
                        catch
                        {
                            node.Remove();
                        }
                    }
                    else
                    {
                        src = src.Replace("../../../..", string.Empty);
                        var path = $"{_environment.WebRootPath}{src}";

                        if (File.Exists(path))
                        {
                            src = $"data:image/{(IsAnimated(path, false) ? "gif" : "png")};base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(path))}";
                            node.SetAttributeValue("src", src);
                        }
                        else
                            node.Remove();
                    }
                }
                else if (!HasValidExtension(src, true))
                    node.Remove();
            }

            if (!location.Exists)
                location.Create();

            if (pdfFile.Exists)
                pdfFile.Delete();

            HtmlConverter.ConvertToPdf(htmlDocument.DocumentNode.OuterHtml, pdfFile.Create());

            imageNodes = htmlDocument.DocumentNode.SelectNodes("//img")?.ToArray();

            if (imageNodes != null)
            {
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
            }

            novelChapter.Content = htmlDocument.DocumentNode.OuterHtml;

            await _chapterManager.Edit(novelChapter);
        }

        private static bool HasValidExtension(string content, bool base64)
        {
            if (base64)
                return Regex.IsMatch(content, "^data:image/(png|jpg|jpeg|gif);base64,.*");

            return Path.GetExtension(content).ToLower() == ".png" || Path.GetExtension(content).ToLower() == ".jpg" || Path.GetExtension(content).ToLower() == ".jpeg" || Path.GetExtension(content).ToLower() == ".gif";
        }

        private static bool IsAnimated(string content, bool base64) => base64 ? Regex.Match(content, "^data:image/(png|jpg|jpeg|gif);base64,.*").Value == "gif" : Path.GetExtension(content).ToLower() == ".gif";
    }
}