using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MicroWarehouse;

namespace MicroWareHouse.Tests.Helpers;

public class TestApiFactory(IntegrationTestFixture fixture) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var overrides = new Dictionary<string, string>
            {
                ["WarehouseDatabase:ConnectionString"] = fixture.MongoContainer.GetConnectionString(),
                ["WarehouseDatabase:DatabaseName"] = "TestDatabase",
                ["WarehouseDatabase:ProductsCollectionName"] = "products",
                ["WarehouseDatabase:CategoriesCollectionName"] = "categories",
                ["WarehouseDatabase:OrdersCollectionName"] = "orders",
                ["WarehouseDatabase:CountersCollectionName"] = "counters"
            };

            config.AddInMemoryCollection(overrides);
        });
    }
}