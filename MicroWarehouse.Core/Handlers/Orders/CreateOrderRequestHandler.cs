using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MicroWarehouse.Contracts.Messaging;
using MicroWarehouse.Core.Abstractions.Enumerations;
using MicroWarehouse.Core.Abstractions.Interfaces;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Mappings;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Handlers.Orders
{
    public class CreateOrderRequestHandler(ILogger<CreateOrderRequestHandler> logger,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IIdGeneratorService idGeneratorService,
        IPublishEndpoint publishEndpoint) : IRequestHandler<CreateOrderRequest, ApiResponse<int>>
    {
        public async Task<ApiResponse<int>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var productDto = await productRepository.GetProductByIdAsync(request.ProductId, cancellationToken);
                if (productDto is null)
                {
                    var error = new Error
                    {
                        Message = $"Product with ID: {request.ProductId} not found"
                    };
                    return ApiResponse<int>.BadRequest(error);
                }

                var categoryDto = await categoryRepository.GetCategoryByIdAsync(productDto.CategoryId, cancellationToken);
                var product = productDto.ToDomain(categoryDto!);

                if (product.IsOutOfStock || request.Quantity > product.AvailableStock)
                {
                    return await HandleInsufficientStockOrderAsync(request, cancellationToken);
                }

                if (product.IsLowOnStock)
                {
                    return await HandleLowStockOrderAsync(request, product, cancellationToken);
                }

                return await ReserveStockAndCreateOrderAsync(request, product, cancellationToken);
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

        private async Task<ApiResponse<int>> HandleLowStockOrderAsync(CreateOrderRequest request, Product product, CancellationToken cancellationToken)
        {
            var order = request.ToDto(OrderStatus.UnderReview);
            var reservationResult = await productRepository.ReserveStockAsync(product.Id, request.Quantity, cancellationToken);
            if (!reservationResult)
            {
                var error = new Error
                {
                    Message = $"Could not reserve {request.Quantity} units of Product with ID: {request.ProductId}"
                };
                return ApiResponse<int>.Conflict(error);
            }

            var newOrderId = await idGeneratorService.GetNextSequenceValueAsync("orders", cancellationToken);
            order.OrderId = newOrderId;
            await orderRepository.CreateOrderAsync(order, cancellationToken);

            await publishEndpoint.Publish<IOrderReviewRequested>(new
            {
                CorrelationId = Guid.NewGuid(),
                OrderId = order.OrderId,
                Quantity = order.Quantity
            }, cancellationToken);

            return ApiResponse<int>.Ok(newOrderId);
        }

        private async Task<ApiResponse<int>> ReserveStockAndCreateOrderAsync(CreateOrderRequest request, Product product, CancellationToken cancellationToken)
        {
            var order = request.ToDto(OrderStatus.Approved);
            var reservationResult = await productRepository.ReserveStockAsync(product.Id, request.Quantity, cancellationToken);

            if (!reservationResult)
            {
                var error = new Error
                {
                    Message = $"Could not reserve {request.Quantity} units of Product with ID: {request.ProductId}"
                };
                return ApiResponse<int>.Conflict(error);
            }

            var newOrderId = await idGeneratorService.GetNextSequenceValueAsync("orders", cancellationToken);
            order.OrderId = newOrderId;
            await orderRepository.CreateOrderAsync(order, cancellationToken);
            return ApiResponse<int>.Created(newOrderId);
        }

        private async Task<ApiResponse<int>> HandleInsufficientStockOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            if (request.ReserveType == ReserveType.None)
            {
                return ApiResponse<int>.Conflict(new Error
                {
                    Message = $"Product with ID: {request.ProductId} is out of stock or insufficient quantity available"
                });
            }

            var pendingOrder = request.ToDto(OrderStatus.AwaitingStock);
            var newOrderId = await idGeneratorService.GetNextSequenceValueAsync("orders", cancellationToken);
            pendingOrder.OrderId = newOrderId;
            await orderRepository.CreateOrderAsync(pendingOrder, cancellationToken);
            return ApiResponse<int>.Created(newOrderId);
        }
    }
}
