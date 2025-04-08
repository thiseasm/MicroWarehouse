using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController(IMediator mediator) : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Product>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductsAsync(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllProductsRequest(), cancellationToken);
            return HandleResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return HandleResponse(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return HandleResponse(result);
        }

        [HttpPut("stock")]
        [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> UpdateProductStockAmountAsync([FromBody] UpdateProductStockAmountRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return HandleResponse(result);
        }
    }
}
