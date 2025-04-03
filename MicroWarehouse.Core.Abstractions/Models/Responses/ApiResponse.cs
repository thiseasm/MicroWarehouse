using System.Net;

namespace MicroWarehouse.Core.Abstractions.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? Error { get; set; }

        public int Code => (int)StatusCode;

        public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data, StatusCode = HttpStatusCode.OK };
        public static ApiResponse<T> Created(T data) => new() { Success = true, Data = data, StatusCode = HttpStatusCode.Created };
        public static ApiResponse<T> BadRequest(string error) => new() { Success = false, Error = error, StatusCode = HttpStatusCode.BadRequest };
        public static ApiResponse<T> NotFound(string error) => new() { Success = false, Error = error, StatusCode = HttpStatusCode.NotFound };
        public static ApiResponse<T> Conflict(string error) => new() { Success = false, Error = error, StatusCode = HttpStatusCode.Conflict };
        public static ApiResponse<T> InternalError(string error) => new() { Success = false, Error = error, StatusCode = HttpStatusCode.InternalServerError };
    }
}
