using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController(IMediator mediator) : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Order>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllOrdersRequest(), cancellationToken);
            return HandleResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return HandleResponse(result);
        }
    }
}
