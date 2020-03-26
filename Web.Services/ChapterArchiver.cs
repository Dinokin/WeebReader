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
        private readonly PageManager<Page> _pageManager;

        public ChapterArchiver(IWebHostEnvironment environment, ChapterManager<TChapter> chapterManager, PageManager<Page> pageManager)
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
        
        private DirectoryInfo GetChapterFolder(Chapter chapter) => new DirectoryInfo($"{Utilities.GetContentFolder(_environment)}{Path.DirectorySeparatorChar}{chapter.TitleId}{Path.DirectorySeparatorChar}{chapter.Id}");

        private async Task<IEnumerable<FileInfo>> AddPages(TChapter chapter, ZipArchive pages, bool deleteOld = false)
        {
            if (deleteOld)
                GetChapterFolder(chapter).Delete(true);

            return chapter switch
            {
                ComicChapter comic => await AddComicPages(comic, pages),
                _ => throw new InvalidOperationException()
            };
        }
        
        private async Task<IEnumerable<FileInfo>> AddComicPages(ComicChapter chapter, ZipArchive pages)
        {
            var location = GetChapterFolder(chapter);
            location.Create();
            
            var pagesInfo = pages.Entries.Where(entry => Path.GetExtension(entry.Name) == ".png" || Path.GetExtension(entry.Name) == ".gif").OrderBy(entry => entry.Name)
                .Select((entry, enumerator) =>
                {
                    using var stream = entry.Open();
                    
                    var fileInfo = Utilities.WriteImage(location, Utilities.ProcessImage(stream), enumerator.ToString());
                    var page = new ComicPage(fileInfo.Extension == ".zip", chapter.Id, Convert.ToUInt16(enumerator));
                    (ComicPage, FileInfo) pageInfo = (page, fileInfo);

                    return pageInfo;
                }).ToArray();

            await _pageManager.AddRange(pagesInfo.Select(tuple => tuple.Item1));

            return pagesInfo.Select(tuple => tuple.Item2);
        }
    }
}