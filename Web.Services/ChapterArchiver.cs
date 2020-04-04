using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;

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

            if (pages != null)
                await AddPages(chapter, pages);

            return true;
        }

        public async Task<bool> EditChapter(TChapter chapter, ZipArchive? pages)
        {
            if (!await _chapterManager.Edit(chapter))
                return false;

            if (pages != null)
                await AddPages(chapter, pages, true);

            return true;
        }

        public async Task<bool> DeleteChapter(TChapter chapter)
        {
            if (!await _chapterManager.Delete(chapter))
                return false;

            GetChapterFolder(chapter).Delete(true);

            return true;
        }

        private async Task<IEnumerable<FileInfo>> AddPages(TChapter chapter, ZipArchive pages, bool deleteOld = false)
        {
            if (deleteOld)
            {
                await _pageManager.DeleteRange(await _pageManager.GetRange(chapter));

                GetChapterFolder(chapter).Delete(true);
            }

            return chapter switch
            {
                ComicChapter comic => await AddComicPages(comic, pages),
                _ => new FileInfo[0]
            };
        }

        private async Task<IEnumerable<FileInfo>> AddComicPages(ComicChapter chapter, ZipArchive pages)
        {
            var location = GetChapterFolder(chapter);
            var entries = pages.Entries.ToArray().Where(entry => ValidateExtension(entry.Name)).OrderBy(entry => entry.Name).ToArray();
            var comicPages = new List<ComicPage>();
            var files = new List<FileInfo>();
            var zipFile = new FileInfo($"{GetChapterFolder(chapter)}/package.zip");

            if (!location.Exists)
                location.Create();

            if (zipFile.Exists)
                zipFile.Delete();

            comicPages.AddRange(entries.Select((entry, i) => new ComicPage(IsAnimated(entry.Name), chapter.Id, Convert.ToUInt16(i))));
            await _pageManager.AddRange(comicPages);

            files.AddRange(comicPages.Select(page => page.Animated ? 
                Utilities.WriteAnimation(location, Utilities.ProcessAnimation(entries[page.Number].Open()), page.Id.ToString()) : 
                Utilities.WriteImage(location, Utilities.ProcessImage(entries[page.Number].Open()), page.Id.ToString())));

            using var zipArchive = new ZipArchive(zipFile.Create(), ZipArchiveMode.Create);

            for (var i = 0; i < files.Count; i++)
                zipArchive.CreateEntryFromFile($"{files[i]}", $"{i}{files[i].Extension}");
            
            return files;
        }

        private DirectoryInfo GetTitleFolder(Chapter chapter) => new DirectoryInfo($"{Utilities.GetContentFolder(_environment)}{Path.DirectorySeparatorChar}{chapter.TitleId}");

        private DirectoryInfo GetChapterFolder(Chapter chapter) => new DirectoryInfo($"{GetTitleFolder(chapter)}{Path.DirectorySeparatorChar}{chapter.Id}");

        private static bool ValidateExtension(string fileName) => Path.GetExtension(fileName).ToLower() == ".png" || Path.GetExtension(fileName).ToLower() == ".jpg" || Path.GetExtension(fileName).ToLower() == ".jpeg" || Path.GetExtension(fileName).ToLower() == ".gif";

        private static bool IsAnimated(string fileName) => Path.GetExtension(fileName).ToLower() == ".gif";
    }
}