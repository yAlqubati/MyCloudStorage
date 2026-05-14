using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MyCloudStorage.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                NotFoundException          => (StatusCodes.Status404NotFound,          "Not Found"),
                ConflictException          => (StatusCodes.Status409Conflict,          "Conflict"),
                ForbiddenException         => (StatusCodes.Status403Forbidden,         "Forbidden"),
                ValidationException        => (StatusCodes.Status400BadRequest,        "Validation Error"),
                InvalidOperationException  => (StatusCodes.Status400BadRequest,        "Bad Request"),
                UnauthorizedAccessException=> (StatusCodes.Status401Unauthorized,      "Unauthorized"),
                _                          => (StatusCodes.Status500InternalServerError,"Server Error")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Unhandled exception at {Method} {Path}",
                    context.Request.Method, context.Request.Path);
            else
                _logger.LogWarning("Handled exception {ExType} at {Method} {Path}: {Message}",
                    exception.GetType().Name,
                    context.Request.Method,
                    context.Request.Path,
                    exception.Message);

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = context.Request.Path
            };
            if (_env.IsDevelopment())
                problem.Extensions["stackTrace"] = exception.ToString();

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}