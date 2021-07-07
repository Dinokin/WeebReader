using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeebReader.Web.API.Utilities;

namespace WeebReader.Web.API.Filters
{
    public class ExceptionHandlerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context) =>
            context.Result = new JsonResult(ModelMapper.MapToDefaultResponse(context.Exception.Message));
    }
}