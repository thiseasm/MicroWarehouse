using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Data.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Validators.Products
{
    public class UpdateProductStockAmountRequestValidator : AbstractValidator<UpdateProductStockAmountRequest>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductStockAmountRequestValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("Amount must be greater than or equal to 0.");
        }
    }
}
