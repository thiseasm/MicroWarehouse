﻿using Microsoft.Extensions.Options;
using MicroWarehouse.Core.Abstractions.Enumerations;
using MicroWarehouse.Infrastructure.Abstractions.DatabaseSettings;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using MongoDB.Driver;

namespace MicroWarehouse.Infrastructure.Repositories
{
    public class OrderRepository(IOptions<WarehouseDatabaseSettings> warehouseDatabaseSettings) : IOrderRepository
    {
        private readonly IMongoCollection<OrderDto> _ordersCollection = InitializeMongoCollection(warehouseDatabaseSettings.Value);

        public async Task<List<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken) => await _ordersCollection.Find(_ => true).ToListAsync(cancellationToken);

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken) => await _ordersCollection.Find(x => x.OrderId == orderId).FirstOrDefaultAsync(cancellationToken);

        public async Task CreateOrderAsync(OrderDto newOrder, CancellationToken cancellationToken) => await _ordersCollection.InsertOneAsync(newOrder, cancellationToken: cancellationToken);

        public async Task<bool> UpdateStatusAsync(int orderId, int newStatus, CancellationToken cancellationToken)
        {
            var update = Builders<OrderDto>.Update.Set(p => p.StatusId, newStatus);
            var result = await _ordersCollection.UpdateOneAsync(p => p.OrderId == orderId, update, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        public Task<List<OrderDto>> GetPendingStockOrdersByProductAsync(int productId, CancellationToken cancellationToken = default)
            => _ordersCollection
                .Find(x => x.ProductId == productId && x.StatusId == (int)OrderStatus.AwaitingStock)
                .SortBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

        private static IMongoCollection<OrderDto> InitializeMongoCollection(WarehouseDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
            return mongoDatabase.GetCollection<OrderDto>(settings.OrdersCollectionName);
        }
    }
}
