using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeebReader.Web.API.Utilities;

namespace WeebReader.Web.API.Filters
{
    public class ExceptionHandlerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var result = new JsonResult(ModelMapper.MapToDefaultResponse(context.Exception.Message));

            context.Result = result;
        }
    }
}