using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;

namespace MicroWarehouse.Core.Validators.Categories
{
    public abstract class CategoryRequestBaseValidator<T> : AbstractValidator<T> where T : CategoryRequestBase
    {
        protected CategoryRequestBaseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.LowStockThreshold)
                .GreaterThanOrEqualTo(0).WithMessage("LowStockThreshold must be greater than or equal to 0.");

            RuleFor(x => x.OutOfStockThreshold)
                .GreaterThanOrEqualTo(0).WithMessage("OutOfStockThreshold must be greater than or equal to 0.")
                .GreaterThan(x => x.LowStockThreshold).WithMessage("OutOfStockThreshold must be greater than LowStockThreshold.");
        }
    }
}
