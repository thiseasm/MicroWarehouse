using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController(ILogger<OrderController> logger, IMediator mediator) : ApiControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return HandleResponse(result);
        }
    }
}
