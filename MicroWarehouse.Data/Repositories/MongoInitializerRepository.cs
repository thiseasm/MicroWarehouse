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
        public MongoInitializerRepository(IOptions<WarehouseDatabaseSettings> warehouseDatabaseSettings)
        {
            var mongoClient = new MongoClient(warehouseDatabaseSettings.Value.ConnectionString);
            _database = mongoClient.GetDatabase(warehouseDatabaseSettings.Value.DatabaseName);
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
                    new BsonDocument { { "_id", "products" }, { "sequence_value", 0 } },
                    new BsonDocument { { "_id", "categories" }, { "sequence_value", 0 } },
                    new BsonDocument { { "_id", "orders" }, { "sequence_value", 0 } }
                };

                await counters.InsertManyAsync(initialCounters, cancellationToken: cancellationToken);
            }
        }
    }
}
