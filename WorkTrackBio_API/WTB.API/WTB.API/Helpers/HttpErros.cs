using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WTB.API.Helpers
{
    public static class HttpErrors
    {
        public static BadRequestObjectResult BadRequest(string? message = null, object? data = null)
        {
            return new BadRequestObjectResult(GetErrorAPIResponse(HttpStatusCode.BadRequest, message, data));
        }

        public static NotFoundObjectResult NotFound(string? message = null, object? data = null)
        {
            return new NotFoundObjectResult(GetErrorAPIResponse(HttpStatusCode.NotFound, message, data));
        }

        public static UnauthorizedObjectResult Unauthorized(string? message = null, object? data = null)
        { 
            return new UnauthorizedObjectResult(GetErrorAPIResponse(HttpStatusCode.Unauthorized, message, data));
        }

        public static ObjectResult Forbidden(string? message = null, object? data = null)
        {
            return new ObjectResult(GetErrorAPIResponse(HttpStatusCode.Forbidden, message, data))
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }

        public static ObjectResult InternalServerError(string? message = null, object? data = null)
        {
            return new ObjectResult(GetErrorAPIResponse(HttpStatusCode.InternalServerError, message, data))
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        public static ObjectResult Conflict(string? message = null, object? data = null)
        {
            return new ObjectResult(GetErrorAPIResponse(HttpStatusCode.Conflict, message, data))
            {
                StatusCode = (int)HttpStatusCode.Conflict
            };
        }

        public static ObjectResult ValidationError(List<string> validationErrors, object? data = null)
        {
            return new ObjectResult(GetErrorAPIResponse(HttpStatusCode.BadRequest, "Validation failed", data, validationErrors))
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        private static APIResponse GetErrorAPIResponse(HttpStatusCode statusCode, string? message, object? data, List<string>? additionalMessages = null)
        {
            var response = new APIResponse
            {
                StatusCode = statusCode,
                Success = false,
                Data = data
            };

            if (!string.IsNullOrWhiteSpace(message))
            {
                response.Messages.Add(message);
            }

            if (additionalMessages != null)
            {
                response.Messages.AddRange(additionalMessages);
            }

            return response;
        }
    }
}
