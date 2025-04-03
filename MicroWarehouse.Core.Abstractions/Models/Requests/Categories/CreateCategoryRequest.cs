﻿using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Abstractions.Models.Requests.Categories
{
    public class CreateCategoryRequest : IRequest<ApiResponse<Category>>
    {
        public required string Name { get; set; }
        public required int LowStockThreshold { get; set; }
        public required int OutOfStockThreshold { get; set; }
    }
}
