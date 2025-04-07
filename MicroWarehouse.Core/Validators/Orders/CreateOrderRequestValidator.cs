using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;

namespace MicroWarehouse.Core.Validators.Orders
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }

    }
}
