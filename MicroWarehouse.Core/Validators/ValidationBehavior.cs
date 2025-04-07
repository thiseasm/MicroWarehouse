using FluentValidation;
using MediatR;
using MicroWarehouse.Core.Abstractions.Models.Responses;

namespace MicroWarehouse.Core.Validators
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var errorDetails = string.Join(", ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
                var error = new Error
                {
                    Message = "Validation failed",
                    Details = errorDetails
                };

                var apiResponse = CreateBadRequestResponse(error);
                return (TResponse)apiResponse!;
            }

            return await next();
        }

        private static object? CreateBadRequestResponse(Error error)
        {
            var responseType = typeof(TResponse);

            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                var innerType = responseType.GetGenericArguments()[0];
                var method = typeof(ApiResponseHelper).GetMethod(nameof(ApiResponseHelper.CreateBadRequest))!;
                var genericMethod = method.MakeGenericMethod(innerType);
                return genericMethod.Invoke(null, [error]);
            }

            return default;
        }
    }

    public static class ApiResponseHelper
    {
        public static ApiResponse<T> CreateBadRequest<T>(Error error) => ApiResponse<T>.BadRequest(error);
    }
}
