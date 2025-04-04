using Microsoft.AspNetCore.Mvc;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using System.Net;

namespace MicroWarehouse.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult HandleResponse<T>(ApiResponse<T> response)
        {
            if (response.Success)
            {
                return response.StatusCode switch
                {
                    HttpStatusCode.OK => Ok(response.Data),
                    HttpStatusCode.Created => Created(string.Empty, response.Data),
                    _ => StatusCode((int)response.StatusCode, response.Data)
                };
            }

            var problemDetails = new ProblemDetails
            {
                Status = (int)response.StatusCode,
                Title = response.Error?.Message,
                Detail = response.Error?.Details
            };

            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => BadRequest(problemDetails),
                HttpStatusCode.NotFound => NotFound(problemDetails),
                HttpStatusCode.Conflict => Conflict(problemDetails),
                HttpStatusCode.InternalServerError => StatusCode((int)HttpStatusCode.InternalServerError, problemDetails),
                _ => StatusCode((int)response.StatusCode, problemDetails)
            };
        }
    }
}
