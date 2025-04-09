using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController(IMediator mediator) : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Category>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllCategoriesRequest(), cancellationToken);
            return HandleResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Category>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return HandleResponse(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<Category>), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ApiResponse<ProblemDetails>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return HandleResponse(result);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProblemDetails>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<ProblemDetails>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategoryAsync([FromBody] DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return HandleResponse(result);
        }
    }
}
