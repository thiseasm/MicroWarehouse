﻿using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Products
{
    public class CreateProductRequest : ProductRequestBase, IRequest<ApiResponse<Product>>
    {
        public required int StockAmount { get; set; }
    }
}
