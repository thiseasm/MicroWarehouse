using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController(ILogger<CategoryController> logger, IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Category>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllCategoriesRequest(), cancellationToken);
            return result.Success
                ? Ok(result.Data)
                : StatusCode(result.Code, result.Error);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Category>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.Success
                ? Ok(result.Data)
                : StatusCode(result.Code, result.Error);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<Category>), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.Success
                ? Ok(result.Data)
                : StatusCode(result.Code, result.Error);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteCategoryAsync([FromBody] DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.Success
                ? Ok(result.Data)
                : StatusCode(result.Code, result.Error);
        }
    }
}
