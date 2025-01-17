using System.Net;
using System.Text.Json;

namespace WebApi.Middleware
{
    public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment environment)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            logger.LogError(exception, "An error occurred.");

            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                NotFoundException => ((int)HttpStatusCode.NotFound, exception.Message),
                ValidationException => ((int)HttpStatusCode.BadRequest, exception.Message),
                ConflictException => ((int)HttpStatusCode.Conflict, exception.Message),
                ForbiddenException => ((int)HttpStatusCode.Forbidden, exception.Message),
                UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "Access denied."),
                ArgumentException => ((int)HttpStatusCode.BadRequest, "Invalid argument provided."),
                InvalidOperationException => ((int)HttpStatusCode.BadRequest, "Invalid operation attempted."),
                _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
            };

            context.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = message,
                Details = environment.IsDevelopment() ? exception.ToString() : null
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }

    // Custom exception classes
    public class NotFoundException(string message) : Exception(message)
    {
    }

    public class ValidationException(string message) : Exception(message)
    {
    }

    public class ConflictException(string message) : Exception(message)
    {
    }

    public class ForbiddenException(string message) : Exception(message)
    {
    }
}
