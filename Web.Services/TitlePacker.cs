using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;

namespace WeebReader.Web.Services
{
    public class TitlePacker<TTitle> where TTitle : Title
    {
        private readonly TitleManager<TTitle> _titleManager;
        private readonly IWebHostEnvironment _environment;

        public TitlePacker(TitleManager<TTitle> titleManager, IWebHostEnvironment environment)
        {
            _titleManager = titleManager;
            _environment = environment;
        }

        public async Task<bool> AddTitle(TTitle title, Stream cover)
        {
            try
            {
                if (!await _titleManager.Add(title))
                    return false;
                
                try
                {
                    Utilities.WriteImage(new DirectoryInfo($"{Utilities.GetContentFolder(_environment)}{Path.DirectorySeparatorChar}{title.Id}"), Utilities.ProcessImage(cover), "cover");

                    return true;
                }
                catch
                {
                    await _titleManager.Delete(title.Id);

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EditTitle(TTitle title, Stream cover)
        {
            try
            {
                if (!await _titleManager.Edit(title))
                    return false;

                try
                {
                    Utilities.WriteImage(new DirectoryInfo($"{Utilities.GetContentFolder(_environment)}{Path.DirectorySeparatorChar}{title.Id}"), Utilities.ProcessImage(cover), "cover");
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

        public async Task<bool> DeleteTitle(TTitle title)
        {
            try
            {
                if (!await _titleManager.Delete(title.Id))
                    return false;

                try
                {
                    new DirectoryInfo($"{Utilities.GetContentFolder(_environment)}{Path.DirectorySeparatorChar}{title.Id}").Delete(true);
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
    }
}