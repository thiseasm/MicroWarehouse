using FluentValidation;
using MediatR;
using MicroWarehouse.Core.Abstractions.Interfaces;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Handlers.Categories;
using MicroWarehouse.Core.Handlers.Products;
using MicroWarehouse.Core.Services;
using MicroWarehouse.Core.Validators.Orders;
using MicroWarehouse.Data.Abstractions.Interfaces;
using MicroWarehouse.Data.Repositories;

namespace MicroWarehouse.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRegistrations(this IServiceCollection services)
        {
            //Mediator
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

            //Handlers
            services.AddTransient<IRequestHandler<GetAllCategoriesRequest, ApiResponse<IEnumerable<Category>>>, GetAllCategoriesRequestHandler>();
            services.AddTransient<IRequestHandler<CreateCategoryRequest, ApiResponse<int>>, CreateCategoryRequestHandler>();
            services.AddTransient<IRequestHandler<UpdateCategoryRequest, ApiResponse<Category>>, UpdateCategoryRequestHandler>();
            services.AddTransient<IRequestHandler<DeleteCategoryRequest, ApiResponse<bool>>, DeleteCategoryRequestHandler>();

            services.AddTransient<IRequestHandler<GetAllProductsRequest, ApiResponse<IEnumerable<Product>>>, GetAllProductsRequestHandler>();
            services.AddTransient<IRequestHandler<CreateProductRequest, ApiResponse<int>>, CreateProductRequestHandler>();
            services.AddTransient<IRequestHandler<UpdateProductRequest, ApiResponse<Product>>, UpdateProductRequestHandler>();
            services.AddTransient<IRequestHandler<UpdateProductStockAmountRequest, ApiResponse<bool>>, UpdateProductStockAmountRequestHandler>();

            //Services
            services.AddTransient<IIdGeneratorService, IdGeneratorService>();
            services.AddSingleton<IMongoInitializationService, MongoInitializationService>();

            //Repositories
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<ICounterRepository, CounterRepository>();
            services.AddSingleton<IMongoInitializerRepository, MongoInitializerRepository>();
        }

        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>(ServiceLifetime.Transient);
        }
    }
}
