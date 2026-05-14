using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace StoreWebApi.ExceptionHandler
{
    public class GlobalExceptionHandler:IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
                _logger.LogError(exception, "unHandled exception");
                context.Response.StatusCode = exception switch
                {
                    ArgumentException=>StatusCodes.Status404NotFound,
                    ApplicationException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError,
                };
                return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context,
                    Exception = exception,
                    ProblemDetails = new ProblemDetails
                    {
                            Type=exception.GetType().Name,
                            Title="error occured",
                            Detail=exception.Message,
                            Status = context.Response.StatusCode

                    }
                });
        }
    }
}
