using Microsoft.Extensions.Options;
using MicroWarehouse.Data.Abstractions.DatabaseSettings;
using MicroWarehouse.Data.Abstractions.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MicroWarehouse.Data.Repositories
{
    public class MongoInitializerRepository : IMongoInitializerRepository
    {
        private readonly IMongoDatabase _database;

        private readonly string _productsCollectionName;
        private readonly string _categoriesCollectionName;
        private readonly string _ordersCollectionName;

        public MongoInitializerRepository(IOptions<WarehouseDatabaseSettings> warehouseDatabaseSettings)
        {
            var mongoClient = new MongoClient(warehouseDatabaseSettings.Value.ConnectionString);
            _database = mongoClient.GetDatabase(warehouseDatabaseSettings.Value.DatabaseName);

            _productsCollectionName = warehouseDatabaseSettings.Value.ProductsCollectionName;
            _categoriesCollectionName = warehouseDatabaseSettings.Value.CategoriesCollectionName;
            _ordersCollectionName = warehouseDatabaseSettings.Value.OrdersCollectionName;
        }
    
        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            var collections = (await _database.ListCollectionNamesAsync(cancellationToken: cancellationToken)).ToList();

            if (!collections.Contains("counters"))
            {
                await _database.CreateCollectionAsync("counters", cancellationToken: cancellationToken);
                var counters = _database.GetCollection<BsonDocument>("counters");

                var initialCounters = new[]
                {
                    new BsonDocument { { "_id", _productsCollectionName }, { "sequence_value", 0 } },
                    new BsonDocument { { "_id", _categoriesCollectionName }, { "sequence_value", 0 } },
                    new BsonDocument { { "_id", _ordersCollectionName }, { "sequence_value", 0 } }
                };

                await counters.InsertManyAsync(initialCounters, cancellationToken: cancellationToken);
            }
        }
    }
}
