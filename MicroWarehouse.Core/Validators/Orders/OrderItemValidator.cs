using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Data.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Validators.Orders
{
    public class OrderItemValidator : AbstractValidator<OrderItem>
    {
        private readonly IProductRepository _productRepository;

        public OrderItemValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}
