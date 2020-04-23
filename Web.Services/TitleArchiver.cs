using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;
using WeebReader.Web.Services.Others;

namespace WeebReader.Web.Services
{
    public class TitleArchiver<TTitle> where TTitle : Title
    {
        private readonly IWebHostEnvironment _environment;
        private readonly TitlesManager<TTitle> _titleManager;

        public TitleArchiver(IWebHostEnvironment environment, TitlesManager<TTitle> titleManager)
        {
            _environment = environment;
            _titleManager = titleManager;
        }

        public async Task<bool> AddTitle(TTitle title, IEnumerable<string> tags, Stream? cover)
        {
            if (!await _titleManager.Add(title, tags))
                return false;

            if (cover != null)
                GenerateCover(title, cover);

            return true;
        }

        public async Task<bool> EditTitle(TTitle title, IEnumerable<string> tags, Stream? cover)
        {
            if (!await _titleManager.Edit(title, tags))
                return false;

            if (cover != null)
                GenerateCover(title, cover);
            
            return true;
        }

        public async Task<bool> DeleteTitle(TTitle title)
        {
            if (!await _titleManager.Delete(title))
                return false;
            
            Utilities.GetTitleFolder(_environment, title.Id).Delete(true);

            return true;
        }

        private void GenerateCover(TTitle title, Stream cover)
        {
            var image = Utilities.ProcessImage(cover);
            Utilities.WriteImage(Utilities.GetTitleFolder(_environment, title.Id), image, "cover", false, false);
            
            image.Resize(new Percentage(350d / image.Width * 100));
            Utilities.WriteImage(Utilities.GetTitleFolder(_environment, title.Id), image, "cover_thumb", true);
        }
    }
}