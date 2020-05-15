using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WeebReader.Web.Models.Controllers.TitlesManager;

namespace WeebReader.Web.Models.Others.Attributes
{
    public class TitleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var model = bindingContext.HttpContext.Request.Form.ContainsKey(nameof(ComicModel.LongStrip)) ? new ComicModel() : new TitleModel();

            foreach (var (key, value) in bindingContext.HttpContext.Request.Form)
            {
                var property = model.GetType().GetProperties().SingleOrDefault(info => string.Equals(info.Name, key, StringComparison.CurrentCultureIgnoreCase));

                if (property == null) 
                    continue;

                if (property.PropertyType.IsEnum)
                    property.SetValue(model, Enum.Parse(property.PropertyType, value.First()));
                else if (property.PropertyType == typeof(Guid?) || property.PropertyType == typeof(Guid))
                    property.SetValue(model, property.PropertyType == typeof(Guid?) && string.IsNullOrWhiteSpace(value.FirstOrDefault()) ? (object?) null : Guid.Parse(value.First()));
                else
                    property.SetValue(model, string.IsNullOrEmpty(value.FirstOrDefault()) ? null : Convert.ChangeType(value.FirstOrDefault(), Nullable.GetUnderlyingType(property.PropertyType) is var underlyingType && underlyingType != null ? underlyingType : property.PropertyType));
            }

            model.Cover = bindingContext.HttpContext.Request.Form.Files.FirstOrDefault();
            
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}