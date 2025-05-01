using ForgettingCurve.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace ForgettingCurve.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception has occurred");

            var code = HttpStatusCode.InternalServerError;
            var problem = new
            {
                type = "server_error",
                title = "An error occurred while processing your request",
                status = (int)code,
                detail = "An unexpected error occurred. Please try again later."
            };

            if (exception is UnauthorizedAccessException)
            {
                code = HttpStatusCode.Unauthorized;
                problem = new
                {
                    type = "authentication_error",
                    title = "Authentication required",
                    status = (int)code,
                    detail = "You must be authenticated to access this resource"
                };
            }
            else if (exception is AuthorizationException)
            {
                code = HttpStatusCode.Forbidden;
                problem = new
                {
                    type = "authorization_error",
                    title = "Not authorized",
                    status = (int)code,
                    detail = "You are not authorized to access this resource"
                };
            }
            else if (exception is NotFoundException)
            {
                code = HttpStatusCode.NotFound;
                problem = new
                {
                    type = "not_found",
                    title = "Resource not found",
                    status = (int)code,
                    detail = exception.Message
                };
            }
            else if (exception is ValidationException)
            {
                code = HttpStatusCode.BadRequest;
                problem = new
                {
                    type = "validation_error",
                    title = "Validation failed",
                    status = (int)code,
                    detail = exception.Message
                };
            }
            else if (exception is DomainException)
            {
                code = HttpStatusCode.BadRequest;
                problem = new
                {
                    type = "domain_error",
                    title = "Domain rule violation",
                    status = (int)code,
                    detail = exception.Message
                };
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)code;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(problem, options);
            await context.Response.WriteAsync(json);
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
} 