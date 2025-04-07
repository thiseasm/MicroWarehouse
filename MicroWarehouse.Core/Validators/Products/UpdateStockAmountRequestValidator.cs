using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;

namespace MicroWarehouse.Core.Validators.Products
{
    public class UpdateProductStockAmountRequestValidator : AbstractValidator<UpdateProductStockAmountRequest>
    {
        public UpdateProductStockAmountRequestValidator()
        {

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.StockAmount)
                .GreaterThanOrEqualTo(0).WithMessage("StockAmount must be greater than or equal to 0.");
        }
    }
}
