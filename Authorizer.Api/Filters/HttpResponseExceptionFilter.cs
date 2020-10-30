using Authorizer.Infrastructure.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorizer.Filter
{
    public class HttpResponseExceptionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is BusinessException bex)
            {
                var responseException = new HttpResponseException()
                {
                    StatusCode = System.Net.HttpStatusCode.UnprocessableEntity
                };

                foreach (var error in bex.Errors)
                    responseException.Violations.Add(new HttpResponseMessage() { Message = error.ErrorMessage });

                context.Result = new ObjectResult(responseException)
                {
                    StatusCode = (int)System.Net.HttpStatusCode.UnprocessableEntity
                };
                context.ExceptionHandled = true;
            }

            else if (context.Result is BadRequestObjectResult && context.ModelState?.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                var responseException = new HttpResponseException()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                foreach (var ms in context.ModelState)
                    foreach (var error in ms.Value.Errors)
                        responseException.Violations.Add(new HttpResponseMessage() { Message = error.ErrorMessage });

                context.Result = new ObjectResult(responseException)
                {
                    StatusCode = (int)System.Net.HttpStatusCode.BadRequest
                };

            }

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
