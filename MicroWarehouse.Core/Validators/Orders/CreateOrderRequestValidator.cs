using FluentValidation;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Abstractions.Models;

namespace MicroWarehouse.Core.Validators.Orders
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator(IValidator<OrderItem> orderItemValidator)
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item.")
                .ForEach(x => x.SetValidator(orderItemValidator))
                .Custom(ValidateNoDuplicateProductIdsExist);
        }

        private static void ValidateNoDuplicateProductIdsExist(IEnumerable<OrderItem> items, ValidationContext<CreateOrderRequest> context)
        {
            var duplicateProductIds = items
                .GroupBy(i => i.ProductId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateProductIds.Any())
            {
                context.AddFailure("Items", $"Order contains duplicate product IDs: {string.Join(", ", duplicateProductIds)}");
            }
        }
    }
}
