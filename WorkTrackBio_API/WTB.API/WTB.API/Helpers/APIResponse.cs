using System.Net;

namespace WTB.API.Helpers
{
    public class APIResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public T? Data { get; set; }
        public DateTime ResponseTime { get; set; }

        public APIResponse()
        {
            this.StatusCode = HttpStatusCode.OK;
            this.Success = true;
            this.Messages = new List<string>();
            this.ResponseTime = DateTime.UtcNow;
        }

        // Métodos estáticos para crear respuestas exitosas
        public static APIResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new APIResponse<T>
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = data,
                Messages = new List<string> { message }
            };
        }

        public static APIResponse<T> SuccessResponse(T data, List<string> messages)
        {
            return new APIResponse<T>
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Data = data,
                Messages = messages
            };
        }

        // Métodos estáticos para crear respuestas de error
        public static APIResponse<T> ErrorResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new APIResponse<T>
            {
                StatusCode = statusCode,
                Success = false,
                Messages = new List<string> { message }
            };
        }

        public static APIResponse<T> ErrorResponse(List<string> messages, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new APIResponse<T>
            {
                StatusCode = statusCode,
                Success = false,
                Messages = messages
            };
        }

        // Método para agregar mensajes
        public void AddMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Messages.Add(message);
            }
        }
    }

    // Clase no genérica para respuestas simples
    public class APIResponse : APIResponse<object>
    {
        public static APIResponse SuccessResponse(string message = "Success")
        {
            return new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                Success = true,
                Messages = new List<string> { message },
                Data = null
            };
        }

        public static new APIResponse ErrorResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new APIResponse
            {
                StatusCode = statusCode,
                Success = false,
                Messages = new List<string> { message },
                Data = null
            };
        }
    }
}
