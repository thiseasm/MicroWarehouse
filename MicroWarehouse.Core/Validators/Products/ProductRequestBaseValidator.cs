using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;

namespace MicroWarehouse.Core.Validators.Products
{
    public abstract class ProductRequestBaseValidator<T> : AbstractValidator<T> where T : ProductRequestBase
    {
        protected ProductRequestBaseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
        }
    }
}
