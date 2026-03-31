using System.Text.Json;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.Exceptions;

namespace WorkTrackBio.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = ex switch
            {
                UnauthorizedException => 401,
                NotFoundException => 404,
                BadRequestException => 400,
                ConflictException => 409,
                _ => 500
            };

            if (statusCode == 500)
                _logger.LogError(ex, "Error no manejado: {Message}", ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = ApiResponse<object>.ErrorResponse(
                ex.Message,
                statusCode);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}