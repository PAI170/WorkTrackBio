namespace WorkTrackBio.API.Common
{
    /// <summary>
    /// Respuesta estándar para todas las operaciones de la API
    /// </summary>
    /// <typeparam name="T">Tipo de datos de la respuesta</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo de la operación
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Datos de la respuesta (puede ser null)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Lista de errores de validación (si los hay)
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Código de estado HTTP
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Timestamp de la respuesta
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Constructor para respuesta exitosa
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Constructor para respuesta exitosa con datos
        /// </summary>
        /// <param name="data">Datos de la respuesta</param>
        /// <param name="message">Mensaje de éxito</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        public ApiResponse(T data, string message = "Operación exitosa", int statusCode = 200)
        {
            Success = true;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }

        /// <summary>
        /// Constructor para respuesta de error
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        /// <param name="errors">Lista de errores específicos</param>
        public ApiResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            Success = false;
            Message = message;
            StatusCode = statusCode;
            Errors = errors;
        }

        /// <summary>
        /// Crea una respuesta exitosa
        /// </summary>
        /// <param name="data">Datos de la respuesta</param>
        /// <param name="message">Mensaje de éxito</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        /// <returns>ApiResponse exitosa</returns>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Operación exitosa", int statusCode = 200)
        {
            return new ApiResponse<T>(data, message, statusCode);
        }

        /// <summary>
        /// Crea una respuesta exitosa sin datos
        /// </summary>
        /// <param name="message">Mensaje de éxito</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        /// <returns>ApiResponse exitosa sin datos</returns>
        public static ApiResponse<object> SuccessResponse(string message = "Operación exitosa", int statusCode = 200)
        {
            return new ApiResponse<object>(null!, message, statusCode);
        }

        /// <summary>
        /// Crea una respuesta de error
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        /// <param name="errors">Lista de errores específicos</param>
        /// <returns>ApiResponse de error</returns>
        public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new ApiResponse<T>(message, statusCode, errors);
        }

        /// <summary>
        /// Crea una respuesta de error con excepción
        /// </summary>
        /// <param name="ex">Excepción que causó el error</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        /// <returns>ApiResponse de error</returns>
        public static ApiResponse<T> ErrorResponse(Exception ex, int statusCode = 500)
        {
            return new ApiResponse<T>(ex.Message, statusCode, new List<string> { ex.Message });
        }
    }

    /// <summary>
    /// Respuesta estándar sin datos genéricos
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// Constructor para respuesta exitosa sin datos
        /// </summary>
        /// <param name="message">Mensaje de éxito</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        public ApiResponse(string message = "Operación exitosa", int statusCode = 200) 
            : base(null!, message, statusCode)
        {
        }

        /// <summary>
        /// Constructor para respuesta de error
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        /// <param name="errors">Lista de errores específicos</param>
        public ApiResponse(string message, int statusCode = 400, List<string>? errors = null) 
            : base(message, statusCode, errors)
        {
        }
    }
}
