using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Rest.Azure;

namespace Api.Filters
{
    public class ErrorFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is CloudException exception)
            {
                context.ModelState.AddModelError("Error", exception.Message);
                context.HttpContext.Response.StatusCode = (int)exception.Response.StatusCode;
            }

            context.Result = new JsonResult(context.ModelState);
        }
    }
}
