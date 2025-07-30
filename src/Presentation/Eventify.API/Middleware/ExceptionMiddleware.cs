using Eventify.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Eventify.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = exception switch
            {
                ValidationException validationException => new ErrorResponse
                {
                    Error = new ErrorDetails
                    {
                        Message = "Validation failed",
                        Details = validationException.Errors
                    }
                }.WithStatusCode(HttpStatusCode.BadRequest),

                NotFoundException => new ErrorResponse
                {
                    Error = new ErrorDetails
                    {
                        Message = exception.Message
                    }
                }.WithStatusCode(HttpStatusCode.NotFound),

                ForbiddenAccessException => new ErrorResponse
                {
                    Error = new ErrorDetails
                    {
                        Message = exception.Message
                    }
                }.WithStatusCode(HttpStatusCode.Forbidden),

                _ => new ErrorResponse
                {
                    Error = new ErrorDetails
                    {
                        Message = "An internal server error occurred",
                        Details = exception.StackTrace
                    }
                }.WithStatusCode(HttpStatusCode.InternalServerError)
            };

            context.Response.StatusCode = (int)errorResponse.StatusCode;

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public class ErrorResponse
    {
        public ErrorDetails Error { get; set; } = new();

        [System.Text.Json.Serialization.JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        public ErrorResponse WithStatusCode(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            return this;
        }
    }

    public class ErrorDetails
    {
        public string Message { get; set; } = string.Empty;
        public object? Details { get; set; }
    }
}