using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;

namespace MicroWarehouse.Core.Validators.Categories
{
    public class UpdateCategoryRequestValidator : CategoryRequestBaseValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
        }
    }
}
