using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;

namespace MicroWarehouse.Core.Validators.Products
{
    public class UpdateProductRequestValidator : ProductRequestBaseValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
        }
    }
}
