using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using WeebReader.Data.Entities.Abstract;
using WeebReader.Data.Services;

namespace WeebReader.Web.Services
{
    public class TitlesArchiver<TTitle> where TTitle : Title
    {
        private readonly IWebHostEnvironment _environment;
        private readonly TitlesManager<TTitle> _titlesManager;

        public TitlesArchiver(IWebHostEnvironment environment, TitlesManager<TTitle> titlesManager)
        {
            _environment = environment;
            _titlesManager = titlesManager;
        }

        public async Task<bool> AddTitle(TTitle title, IEnumerable<string> tags, Stream cover)
        {
            try
            {
                if (!await _titlesManager.Add(title, tags))
                    return false;
                
                var directory = new DirectoryInfo($"{Utilities.GetContentFolder(_environment)}{Path.DirectorySeparatorChar}{title.Id}");
                directory.Create();

                try
                {
                    Utilities.WriteImage(directory, Utilities.ProcessImage(cover), "cover");

                    return true;
                }
                catch
                {
                    directory.Delete();
                    await _titlesManager.Delete(title);

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EditTitle(TTitle title, IEnumerable<string> tags, Stream cover)
        {
            try
            {
                if (!await _titlesManager.Edit(title, tags))
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
                if (!await _titlesManager.Delete(title))
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