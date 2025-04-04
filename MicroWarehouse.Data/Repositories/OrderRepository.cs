using Microsoft.Extensions.Options;
using MicroWarehouse.Data.Abstractions.DatabaseSettings;
using MicroWarehouse.Data.Abstractions.DTOs;
using MicroWarehouse.Data.Abstractions.Interfaces;
using MongoDB.Driver;

namespace MicroWarehouse.Data.Repositories
{
    public class OrderRepository(IOptions<WarehouseDatabaseSettings> warehouseDatabaseSettings) : IOrderRepository
    {
        private readonly IMongoCollection<OrderDto> _ordersCollection = InitializeMongoCollection(warehouseDatabaseSettings.Value);

        public async Task<List<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken) => await _ordersCollection.Find(_ => true).ToListAsync(cancellationToken);

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken) => await _ordersCollection.Find(x => x.OrderId == orderId).FirstOrDefaultAsync(cancellationToken);

        public async Task CreateOrderAsync(OrderDto newOrder, CancellationToken cancellationToken) => await _ordersCollection.InsertOneAsync(newOrder, cancellationToken: cancellationToken);

        public async Task<bool> UpdateStatusAsync(int orderId, int newStatus, CancellationToken cancellationToken)
        {
            var update = Builders<OrderDto>.Update.Set(p => p.Status, newStatus);
            var result = await _ordersCollection.UpdateOneAsync(p => p.OrderId == orderId, update, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        private static IMongoCollection<OrderDto> InitializeMongoCollection(WarehouseDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
            return mongoDatabase.GetCollection<OrderDto>(settings.OrdersCollectionName);
        }
    }
}
