using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeebReader.Web.API.Models.Response;

namespace WeebReader.Web.API.Filters
{
    public class ModelValidatorFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new DefaultResponseMessage
                {
                    Message = context.ModelState.SelectMany(state => state.Value.Errors).Select(error => error.ErrorMessage)
                });
            }
        }
    }
}