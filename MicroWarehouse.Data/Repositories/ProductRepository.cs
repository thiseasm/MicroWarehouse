using Microsoft.Extensions.Options;
using MicroWarehouse.Infrastructure.Abstractions.DatabaseSettings;
using MicroWarehouse.Infrastructure.Abstractions.DTOs;
using MicroWarehouse.Infrastructure.Abstractions.Interfaces;
using MongoDB.Driver;

namespace MicroWarehouse.Infrastructure.Repositories
{
    public class ProductRepository(IOptions<WarehouseDatabaseSettings> warehouseDatabaseSettings) : IProductRepository
    {
        private readonly IMongoCollection<ProductDto> _productsCollection = InitializeMongoCollection(warehouseDatabaseSettings.Value);

        public async Task<List<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken) => await _productsCollection.Find(_ => true).ToListAsync(cancellationToken);

        public async Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken) 
            => await _productsCollection.Find(x => x.ProductId == productId).FirstOrDefaultAsync(cancellationToken);

        public async Task CreateAsync(ProductDto newProduct, CancellationToken cancellationToken) => await _productsCollection.InsertOneAsync(newProduct, cancellationToken: cancellationToken);

        public async Task<bool> UpdateAsync(ProductDto updatedProduct, CancellationToken cancellationToken)
        {
            var update = Builders<ProductDto>.Update
                .Set(p => p.Name, updatedProduct.Name)
                .Set(p => p.CategoryId, updatedProduct.CategoryId);

            var result = await _productsCollection.UpdateOneAsync(x => x.ProductId == updatedProduct.ProductId, update, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateStockAsync(int productId, int newStockAmount, CancellationToken cancellationToken)
        {
            var update = Builders<ProductDto>.Update.Set(p => p.StockAmount, newStockAmount);
            var result = await _productsCollection.UpdateOneAsync(p => p.ProductId == productId, update, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ReserveStockAsync(int productId, int reservationQuantity, CancellationToken cancellationToken)
        {
            var update = Builders<ProductDto>.Update.Inc(p => p.StockAmount, -reservationQuantity);
            var result = await _productsCollection.UpdateOneAsync(p => p.ProductId == productId, update, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ProductsWithCategoryExistAsync(int categoryId, CancellationToken cancellationToken)
        {
            var count = await _productsCollection.CountDocumentsAsync(x => x.CategoryId == categoryId, cancellationToken: cancellationToken);
            return count > 0;
        }

        private static IMongoCollection<ProductDto> InitializeMongoCollection(WarehouseDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
            return mongoDatabase.GetCollection<ProductDto>(settings.ProductsCollectionName);
        }
    }
}
