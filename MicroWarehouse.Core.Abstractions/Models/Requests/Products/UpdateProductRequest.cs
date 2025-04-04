using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Products
{
    public class UpdateProductRequest : ProductRequestBase, IRequest<ApiResponse<Product>>
    {
        public required int Id { get; set; }
    }
}
