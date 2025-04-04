using Microsoft.Extensions.Options;
using MicroWarehouse.Data.Abstractions.DatabaseSettings;
using MicroWarehouse.Data.Abstractions.DTOs;
using MongoDB.Driver;

namespace MicroWarehouse.Data.Repositories
{
    public class OrderRepository(IOptions<WarehouseDatabaseSettings> warehouseDatabaseSettings)
    {
        private readonly IMongoCollection<OrderDto> _ordersCollection = InitializeMongoCollection(warehouseDatabaseSettings.Value);

        public async Task<List<OrderDto>> GetAsync() => await _ordersCollection.Find(_ => true).ToListAsync();

        public async Task<OrderDto?> GetAsync(int orderId) => await _ordersCollection.Find(x => x.OrderId == orderId).FirstOrDefaultAsync();

        public async Task CreateAsync(OrderDto newOrder) => await _ordersCollection.InsertOneAsync(newOrder);

        public async Task UpdateAsync(int orderId, OrderDto updateOrder) => await _ordersCollection.ReplaceOneAsync(x => x.OrderId == orderId, updateOrder);

        private static IMongoCollection<OrderDto> InitializeMongoCollection(WarehouseDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
            return mongoDatabase.GetCollection<OrderDto>(settings.OrdersCollectionName);
        }
    }
}
