using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;

namespace MicroWarehouse.Core.Validators.Products
{
    public class CreateProductRequestValidator : ProductRequestBaseValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.StockAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Starting stock amount must be greater than or equal to 0.");
        }
    }
}
