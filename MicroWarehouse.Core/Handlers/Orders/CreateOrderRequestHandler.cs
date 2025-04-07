using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Handlers.Orders
{
    public class CreateOrderRequestHandler(ILogger<CreateOrderRequestHandler> logger) : IRequestHandler<CreateOrderRequest, ApiResponse<int>>
    {
        public async Task<ApiResponse<int>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return ApiResponse<int>.Ok(0);


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{HandlerName} : Handle failed with Error: {Message}", nameof(CreateOrderRequestHandler), ex.Message);
                var error = new Error
                {
                    Message = ex.Message
                };

                return ApiResponse<int>.InternalError(error);
            }
        }
    }
}
