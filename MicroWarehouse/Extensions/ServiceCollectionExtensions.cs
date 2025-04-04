using Microsoft.Extensions.Options;
using MicroWarehouse.Data.Abstractions.DatabaseSettings;
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

            //Repositories
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
        }
    }
}
