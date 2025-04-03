using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController(ILogger<ProductController> logger, IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Product>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductsAsync(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllCategoriesRequest(), cancellationToken);
            return result.Success
                ? Ok(result.Data)
                : StatusCode(result.Code, result.Error);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.Success
                ? Ok(result.Data)
                : StatusCode(result.Code, result.Error);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.Success
                ? Ok(result.Data)
                : StatusCode(result.Code, result.Error);
        }

        [HttpPut("stock")]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> UpdateProcuctStockAmountAsync([FromBody] DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.Success
                ? Ok(result.Data)
                : StatusCode(result.Code, result.Error);
        }
    }
}
