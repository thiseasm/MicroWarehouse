using Microsoft.Extensions.Options;
using MicroWarehouse.Infrastructure.Abstractions.DatabaseSettings;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using MongoDB.Driver;

namespace MicroWarehouse.Infrastructure.Repositories
{
    public class CounterRepository(IOptions<WarehouseDatabaseSettings> warehouseDatabaseSettings) : ICounterRepository
    {
        private readonly IMongoCollection<CounterDto> _countersCollection = InitializeMongoCollection(warehouseDatabaseSettings.Value);

        public async Task<int> IncrementCounterAsync(string entityName, CancellationToken cancellationToken)
        {
            var filter = Builders<CounterDto>.Filter.Eq(x => x.Id, entityName);
            var update = Builders<CounterDto>.Update.Inc(nameof(CounterDto.Sequence), 1);

            var options = new FindOneAndUpdateOptions<CounterDto>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var result = await _countersCollection.FindOneAndUpdateAsync(filter, update, options, cancellationToken);
            return result.Sequence;
        }

        private static IMongoCollection<CounterDto> InitializeMongoCollection(WarehouseDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
            return mongoDatabase.GetCollection<CounterDto>(settings.CountersCollectionName);
        }
    }
}
