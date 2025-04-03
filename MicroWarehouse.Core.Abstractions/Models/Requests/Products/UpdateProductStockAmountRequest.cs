﻿using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Products
{
    public class DeleteCategoryRequest : IRequest<ApiResponse<bool>>
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
    }
}
