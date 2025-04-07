using FluentValidation;
using MicroWarehouse.Core.Abstractions.Interfaces;
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
