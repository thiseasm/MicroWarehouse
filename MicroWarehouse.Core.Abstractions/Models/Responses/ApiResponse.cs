using System.Net;

namespace MicroWarehouse.Core.Abstractions.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Error? Error { get; set; }

        public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data, StatusCode = HttpStatusCode.OK };
        public static ApiResponse<T> Created(T data) => new() { Success = true, Data = data, StatusCode = HttpStatusCode.Created };
        public static ApiResponse<T> BadRequest(Error error) => new() { Success = false, Error = error, StatusCode = HttpStatusCode.BadRequest };
        public static ApiResponse<T> NotFound(Error error) => new() { Success = false, Error = error, StatusCode = HttpStatusCode.NotFound };
        public static ApiResponse<T> Conflict(Error error) => new() { Success = false, Error = error, StatusCode = HttpStatusCode.Conflict };
        public static ApiResponse<T> InternalError(Error error) => new() { Success = false, Error = error, StatusCode = HttpStatusCode.InternalServerError };
    }
}
