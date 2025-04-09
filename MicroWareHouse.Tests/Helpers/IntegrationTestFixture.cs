using MongoDB.Bson;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace MicroWareHouse.Tests.Helpers;

public class IntegrationTestFixture : IAsyncLifetime
{
    public MongoDbContainer MongoContainer { get; private set; }

    public IMongoClient MongoClient { get; private set; }

    private readonly string _mongoDatabaseName = "TestDatabase";

    public async Task InitializeAsync()
    {
        // Create MongoDB container
        MongoContainer = new MongoDbBuilder()
            .WithImage("mongo:6.0")
            .WithUsername("root")
            .WithPassword("example")
            .WithPortBinding(27017, true)
            .Build();

        await MongoContainer.StartAsync();

        MongoClient = new MongoClient(MongoContainer.GetConnectionString());
        await InitializeMongoDatabase();
    }

    private async Task InitializeMongoDatabase()
    {
        var database = MongoClient.GetDatabase(_mongoDatabaseName);

        var collections = (await database.ListCollectionNamesAsync()).ToList();

        if (!collections.Contains("counters"))
        {
            await database.CreateCollectionAsync("counters");
            var counters = database.GetCollection<BsonDocument>("counters");

            var initialCounters = new[]
            {
                new BsonDocument { { "_id", "products" }, { "sequence_value", 0 } },
                new BsonDocument { { "_id", "categories" }, { "sequence_value", 0 } },
                new BsonDocument { { "_id", "orders" }, { "sequence_value", 0 } }
            };

            await counters.InsertManyAsync(initialCounters);
        }

        if (!collections.Contains("categories"))
        {
            await database.CreateCollectionAsync("categories");
            var categories = database.GetCollection<BsonDocument>("categories");

            var initialCategory = new BsonDocument
            {
                { "_id", 0 },
                { "Name", "Electronics" },
                { "LowStockThreshold", 5 },
                { "OutOfStockThreshold", 0 }
            };

            await categories.InsertOneAsync(initialCategory);
        }

        if (!collections.Contains("products"))
        {
            await database.CreateCollectionAsync("products");
            var products = database.GetCollection<BsonDocument>("products");

            var initialProduct = new BsonDocument
            {
                { "_id", 0 },
                { "Name", "Smartphone" },
                { "StockAmount", 100 },
                { "CategoryId", 0 }
            };

            await products.InsertOneAsync(initialProduct);
        }

        if (!collections.Contains("orders"))
        {
            await database.CreateCollectionAsync("orders");
            var orders = database.GetCollection<BsonDocument>("orders");

            var initialOrder = new BsonDocument
            {
                { "_id", 0 },
                { "StatusId", 1 }, 
                { "ProductId", 0 },
                { "Quantity", 2 },
                { "CreatedAt", DateTime.UtcNow }
            };

            await orders.InsertOneAsync(initialOrder);
        }
    }

    public async Task DisposeAsync()
    {
        if (MongoContainer != null)
            await MongoContainer.DisposeAsync();
    }
}
