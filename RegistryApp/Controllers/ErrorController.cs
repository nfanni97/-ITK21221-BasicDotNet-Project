using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RegistryApp.Controllers
{
    public class ErrorsController : ControllerBase
    {
        private readonly ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger) {
            _logger = logger;
        }

        [Route("error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public MyErrorResponse Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;
            var code = 500;
            var message = "Internal server error";

            _logger.LogError(exception, "Something went wrong");

            // log etc.
            if(exception is InvalidOperationException) {
                code = 409;
                message = "Operation cannot be executed in the current state of the application";
            } else if(exception is ArgumentException) {
                code = 400;
                message = "Something is wrong with the parameters provided";
            } else if(exception is HttpResponseException httpExp) {
                code = httpExp.Status;
                message = httpExp.Message;
            }

            Response.StatusCode = code;
            return new MyErrorResponse(message);
        }
    }

    public class MyErrorResponse
    {
        public string Message { get; init; }

        public MyErrorResponse(string m)
        {
            Message = m;
        }
    }

    public class HttpResponseException : Exception
    {
        public HttpResponseException(string m) : base(m) {}

        public int Status { get; set; } = 500;
    }
}