using Microsoft.Extensions.Options;
using MicroWarehouse.Data.Abstractions.DatabaseSettings;
using MicroWarehouse.Data.Abstractions.DTOs;
using MongoDB.Driver;

namespace MicroWarehouse.Data.Repositories
{
    public class ProductRepository(IOptions<WarehouseDatabaseSettings> warehouseDatabaseSettings)
    {
        private readonly IMongoCollection<ProductDto> _productsCollection = InitializeMongoCollection(warehouseDatabaseSettings.Value);

        public async Task<List<ProductDto>> GetAsync() => await _productsCollection.Find(_ => true).ToListAsync();

        public async Task<ProductDto?> GetAsync(int productId) => await _productsCollection.Find(x => x.ProductId == productId).FirstOrDefaultAsync();

        public async Task CreateAsync(ProductDto newProduct) => await _productsCollection.InsertOneAsync(newProduct);

        public async Task UpdateAsync(int productId, ProductDto updatedProduct) => await _productsCollection.ReplaceOneAsync(x => x.ProductId == productId, updatedProduct);

        public async Task<bool> UpdateStockAsync(int productId, int newQuantity)
        {
            var update = Builders<ProductDto>.Update.Set(p => p.StockAmount, newQuantity);
            var result = await _productsCollection.UpdateOneAsync(p => p.ProductId == productId, update);
            return result.ModifiedCount > 0;
        }

        private static IMongoCollection<ProductDto> InitializeMongoCollection(WarehouseDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
            return mongoDatabase.GetCollection<ProductDto>(settings.ProductsCollectionName);
        }
    }
}
