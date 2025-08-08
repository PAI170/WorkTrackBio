using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace WTB.API.Helpers
{
    /// <summary>
    /// Helper estático para generar respuestas de error HTTP estandarizadas
    /// </summary>
    public static class HttpErrors
    {
        /// <summary>
        /// Error 400 - Bad Request
        /// </summary>
        public static IActionResult BadRequest(string message = "Solicitud incorrecta")
        {
            var response = APIResponse.ErrorResponse(message, HttpStatusCode.BadRequest);
            return new BadRequestObjectResult(response);
        }

        /// <summary>
        /// Error 404 - Not Found
        /// </summary>
        public static IActionResult NotFound(string message = "Recurso no encontrado")
        {
            var response = APIResponse.ErrorResponse(message, HttpStatusCode.NotFound);
            return new NotFoundObjectResult(response);
        }

        /// <summary>
        /// Error 401 - Unauthorized
        /// </summary>
        public static IActionResult Unauthorized(string message = "No autorizado")
        {
            var response = APIResponse.ErrorResponse(message, HttpStatusCode.Unauthorized);
            return new UnauthorizedObjectResult(response);
        }

        /// <summary>
        /// Error 403 - Forbidden
        /// </summary>
        public static IActionResult Forbidden(string message = "Acceso prohibido")
        {
            var response = APIResponse.ErrorResponse(message, HttpStatusCode.Forbidden);
            return new ObjectResult(response) { StatusCode = 403 };
        }

        /// <summary>
        /// Error 500 - Internal Server Error
        /// </summary>
        public static IActionResult InternalServerError(string message = "Error interno del servidor")
        {
            var response = APIResponse.ErrorResponse(message, HttpStatusCode.InternalServerError);
            return new ObjectResult(response) { StatusCode = 500 };
        }

        /// <summary>
        /// Error 409 - Conflict
        /// </summary>
        public static IActionResult Conflict(string message = "Conflicto de recursos")
        {
            var response = APIResponse.ErrorResponse(message, HttpStatusCode.Conflict);
            return new ConflictObjectResult(response);
        }

        /// <summary>
        /// Error 422 - Validation Error (Unprocessable Entity)
        /// </summary>
        public static IActionResult ValidationError(ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = APIResponse.ErrorResponse(errors, HttpStatusCode.UnprocessableEntity);
            return new UnprocessableEntityObjectResult(response);
        }

        /// <summary>
        /// Error de validación con mensaje personalizado
        /// </summary>
        public static IActionResult ValidationError(string message)
        {
            var response = APIResponse.ErrorResponse(message, HttpStatusCode.UnprocessableEntity);
            return new UnprocessableEntityObjectResult(response);
        }

        /// <summary>
        /// Error de validación con múltiples mensajes
        /// </summary>
        public static IActionResult ValidationError(List<string> messages)
        {
            var response = APIResponse.ErrorResponse(messages, HttpStatusCode.UnprocessableEntity);
            return new UnprocessableEntityObjectResult(response);
        }
    }
}
