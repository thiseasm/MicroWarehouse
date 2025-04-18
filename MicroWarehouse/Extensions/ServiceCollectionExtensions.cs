﻿using FluentValidation;
using MassTransit;
using MediatR;
using MicroWarehouse.Core.Abstractions.Interfaces;
using MicroWarehouse.Core.Abstractions.Models;
using MicroWarehouse.Core.Abstractions.Models.Requests.Categories;
using MicroWarehouse.Core.Abstractions.Models.Requests.Orders;
using MicroWarehouse.Core.Abstractions.Models.Requests.Products;
using MicroWarehouse.Core.Abstractions.Models.Responses;
using MicroWarehouse.Core.Handlers.Categories;
using MicroWarehouse.Core.Handlers.Orders;
using MicroWarehouse.Core.Handlers.Products;
using MicroWarehouse.Core.Services;
using MicroWarehouse.Core.Validators;
using MicroWarehouse.Core.Validators.Categories;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using MicroWarehouse.Infrastructure.Abstractions.Sagas;
using MicroWarehouse.Infrastructure.Consumers;
using MicroWarehouse.Infrastructure.Repositories;
using MicroWarehouse.Infrastructure.Sagas;

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

            services.AddTransient<IRequestHandler<CreateOrderRequest, ApiResponse<int>>, CreateOrderRequestHandler>();
            services.AddTransient<IRequestHandler<GetAllOrdersRequest, ApiResponse<IEnumerable<Order>>>, GetAllOrdersRequestHandler>();

            //Services
            services.AddSingleton<IIdGeneratorService, IdGeneratorService>();
            services.AddSingleton<IMongoInitializationService, MongoInitializationService>();
            services.AddSingleton<IOrderFinalizationService, OrderFinalizationService>();

            //Repositories
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<ICounterRepository, CounterRepository>();
            services.AddSingleton<IMongoInitializerRepository, MongoInitializerRepository>();
        }

        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateCategoryRequestValidator>(ServiceLifetime.Transient);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        public static void AddMessaging(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<OrderReviewStateMachine, OrderReviewState>()
                    .InMemoryRepository();

                x.AddConsumer<OrderReviewRequestedConsumer>();
                x.AddConsumer<StockUpdatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
