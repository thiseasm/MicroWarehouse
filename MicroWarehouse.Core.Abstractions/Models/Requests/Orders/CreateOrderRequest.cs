using MediatR;
using MicroWarehouse.Core.Abstractions.Enumerations;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Orders
{
    public class CreateOrderRequest : IRequest<ApiResponse<int>>
    {
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
        public ReserveType ReserveType { get; set; } = ReserveType.None;
    }
}
