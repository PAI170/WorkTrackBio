namespace WorkTrackBio.API.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public ApiResponse()
        {
        }
        public ApiResponse(T data, string message = "Operación exitosa", int statusCode = 200)
        {
            Success = true;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }
        public ApiResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            Success = false;
            Message = message;
            StatusCode = statusCode;
            Errors = errors;
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operación exitosa", int statusCode = 200)
        {
            return new ApiResponse<T>(data, message, statusCode);
        }

        public static ApiResponse<object> SuccessResponse(string message = "Operación exitosa", int statusCode = 200)
        {
            return new ApiResponse<object>(null!, message, statusCode);
        }
        public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new ApiResponse<T>(message, statusCode, errors);
        }
        public static ApiResponse<T> ErrorResponse(Exception ex, int statusCode = 500)
        {
            return new ApiResponse<T>(ex.Message, statusCode, new List<string> { ex.Message });
        }
    }

    public class ApiResponse : ApiResponse<object>
    {

        public ApiResponse(string message = "Operación exitosa", int statusCode = 200) 
            : base(null!, message, statusCode)
        {
        }

        public ApiResponse(string message, int statusCode = 400, List<string>? errors = null) 
            : base(message, statusCode, errors)
        {
        }
    }
}
