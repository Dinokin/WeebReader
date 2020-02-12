using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using iText.Html2pdf;
using Microsoft.AspNetCore.Hosting;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;

namespace WeebReader.Web.Services
{
    public class ChapterPacker<TChapter> where TChapter : Chapter
    {
        private readonly ChapterManager<TChapter> _chapterManager;
        private readonly PageManager<ComicPage> _pageManager;
        private readonly IWebHostEnvironment _environment;

        public ChapterPacker(ChapterManager<TChapter> chapterManager, PageManager<ComicPage> pageManager, IWebHostEnvironment environment)
        {
            _chapterManager = chapterManager;
            _pageManager = pageManager;
            _environment = environment;
        }

        public async Task<bool> AddChapter(TChapter chapter, Stream pages)
        {
            try
            {
                if (!await _chapterManager.Add(chapter))
                    return false;

                try
                {
                    switch (chapter)
                    {
                        case ComicChapter comicChapter:
                            await ProcessComicChapter(comicChapter, pages);
                            break;
                        case NovelChapter novelChapter:
                            await ProcessNovelChapter(novelChapter);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    return true;
                }
                catch
                {
                    await _chapterManager.Delete(chapter);
                    
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EditChapter(TChapter chapter, Stream pages)
        {
            try
            {
                if (!await _chapterManager.Edit(chapter))
                    return false;

                try
                {
                    switch (chapter)
                    {
                        case ComicChapter comicChapter:
                            await ProcessComicChapter(comicChapter, pages);
                            break;
                        case NovelChapter novelChapter:
                            await ProcessNovelChapter(novelChapter);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteChapter(TChapter chapter)
        {
            try
            {
                if (!await _chapterManager.Delete(chapter))
                    return false;

                try
                {
                    new DirectoryInfo($"{Utilities.GetContentFolder(_environment)}{Path.DirectorySeparatorChar}{chapter.TitleId}{Path.DirectorySeparatorChar}{chapter.Id}").Delete(true);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task ProcessComicChapter(ComicChapter chapter, Stream pages)
        {
            if (pages == null)
                return;

            var location = new DirectoryInfo($"{Utilities.GetContentFolder(_environment)}{Path.DirectorySeparatorChar}{chapter.TitleId}{Path.DirectorySeparatorChar}{chapter.Id}");
            location.Delete(true);
            var images = new ZipArchive(pages, ZipArchiveMode.Read).Entries.OrderBy(entry => entry.Name).Select(entry => Utilities.ProcessImage(entry.Open())).ToArray(); 
            chapter.Pages = images.Select((image, i) => new ComicPage
            {
                Number = (ushort) i,
                Animated = image.Format == MagickFormat.Gif,
                ChapterId = chapter.Id
            });

            foreach (var page in await _pageManager.GetPagesByChapter(chapter))
                await _pageManager.Delete(page);

            using var archive = new ZipArchive(new FileInfo($"{location}{Path.DirectorySeparatorChar}download.zip").Create(), ZipArchiveMode.Create);
            
            foreach (var page in chapter.Pages)
            {
                await _pageManager.Add(page);
                var file = Utilities.WriteImage(location, images[page.Number], page.ChapterId.ToString());
                archive.CreateEntryFromFile($"{file}", page.Number.ToString(), CompressionLevel.Optimal);
            }
        }

        private Task ProcessNovelChapter(NovelChapter chapter)
        {
            var location = new DirectoryInfo($"{Utilities.GetContentFolder(_environment)}{Path.DirectorySeparatorChar}{chapter.TitleId}{Path.DirectorySeparatorChar}{chapter.Id}");
            location.Delete(true);
            var file = new FileInfo($"{location}{Path.DirectorySeparatorChar}download.pdf");
            HtmlConverter.ConvertToPdf(chapter.Content, file.Create());
            
            return Task.CompletedTask;
        }
    }
}