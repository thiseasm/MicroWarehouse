using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Data.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Validators.Products
{
    public class UpdateProductRequestValidator : ProductRequestBaseValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator(ICategoryRepository categoryRepository) : base(categoryRepository)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
        }
    }
}
