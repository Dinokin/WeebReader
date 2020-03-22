using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;

namespace WeebReader.Web.Services
{
    public class TitleArchiver<TTitle> where TTitle : Title
    {
        private readonly IWebHostEnvironment _environment;
        private readonly TitleManager<TTitle> _titleManager;

        public TitleArchiver(IWebHostEnvironment environment, TitleManager<TTitle> titleManager)
        {
            _environment = environment;
            _titleManager = titleManager;
        }

        public async Task<bool> AddTitle(TTitle title, IEnumerable<string> tags, Stream cover)
        {
            if (!await _titleManager.Add(title, tags))
                return false;
                
            var directory = GetTitleFolder(title);
            
            directory.Create();
            Utilities.WriteImage(directory, Utilities.ProcessImage(cover), "cover", true);

            return true;
        }

        public async Task<bool> EditTitle(TTitle title, IEnumerable<string> tags, Stream? cover)
        {
            if (!await _titleManager.Edit(title, tags))
                return false;

            if (cover != null)
                Utilities.WriteImage(GetTitleFolder(title), Utilities.ProcessImage(cover), "cover", true);

            return true;
        }

        public async Task<bool> DeleteTitle(TTitle title)
        {
            if (!await _titleManager.Delete(title))
                return false;
            
            GetTitleFolder(title).Delete(true);

            return true;
        }

        private DirectoryInfo GetTitleFolder(TTitle title) => new DirectoryInfo($"{Utilities.GetContentFolder(_environment)}{Path.DirectorySeparatorChar}{title.Id}");
    }
}