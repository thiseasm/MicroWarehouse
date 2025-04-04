using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Products
{
    public class CreateProduct : IRequest<ApiResponse<Product>>
    {
        public required string Name { get; set; }
        public required int CategoryId { get; set; }
    }
}
