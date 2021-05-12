using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WeebReader.Web.Models.Controllers.ChaptersManager;

namespace WeebReader.Web.Models.Others.Attributes
{
    public class ChapterModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ChapterModel model;
            
            if (bindingContext.HttpContext.Request.Form.Files.GetFile(nameof(ComicChapterModel.Pages)) != null)
            {
                var comicChapterModel = new ComicChapterModel
                {
                    Pages = bindingContext.HttpContext.Request.Form.Files.FirstOrDefault()
                };
                model = comicChapterModel;
            }
            else if (bindingContext.HttpContext.Request.Form.ContainsKey(nameof(NovelChapterModel.Content)))
                model = new NovelChapterModel();
            else
                model = new ChapterModel();

            model.TitleId = Guid.Parse(bindingContext.ValueProvider.GetValue(nameof(ChapterModel.TitleId)).FirstValue!);
            
            foreach (var (key, value) in bindingContext.HttpContext.Request.Form)
            {
                var property = model.GetType().GetProperties().SingleOrDefault(info => string.Equals(info.Name, key, StringComparison.CurrentCultureIgnoreCase));

                if (property == null) 
                    continue;

                if (property.PropertyType.IsEnum)
                    property.SetValue(model, Enum.Parse(property.PropertyType, value.First()));
                else if (property.PropertyType == typeof(Guid?) || property.PropertyType == typeof(Guid))
                    property.SetValue(model, property.PropertyType == typeof(Guid?) && string.IsNullOrWhiteSpace(value.FirstOrDefault()) ? null : Guid.Parse(value.First()));
                else
                    property.SetValue(model, string.IsNullOrEmpty(value.FirstOrDefault()) ? null : Convert.ChangeType(value.FirstOrDefault(), Nullable.GetUnderlyingType(property.PropertyType) is var underlyingType && underlyingType != null ? underlyingType : property.PropertyType));
            }
            
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}