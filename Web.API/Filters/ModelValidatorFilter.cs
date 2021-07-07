using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeebReader.Web.API.Utilities;

namespace WeebReader.Web.API.Filters
{
    public class ModelValidatorFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(ModelMapper.MapToDefaultResponse(context.ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)));
            }
        }
    }
}