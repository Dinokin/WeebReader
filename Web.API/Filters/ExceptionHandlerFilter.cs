using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeebReader.Web.API.Models.Response;

namespace WeebReader.Web.API.Filters
{
    public class ExceptionHandlerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var result = new JsonResult(new DefaultResponseMessage
            {
                Message = context.Exception.Message
            });

            context.Result = result;
        }
    }
}