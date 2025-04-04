using MicroWarehouse.Data.Abstractions.DatabaseSettings;
using MicroWarehouse.Data.Abstractions.DTOs;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace MicroWarehouse.Data.Repositories
{
    public class CategoryRepository(IOptions<WarehouseDatabaseSettings> warehouseDatabaseSettings)
    {
        private readonly IMongoCollection<CategoryDto> _categoriesCollection = InitializeMongoCollection(warehouseDatabaseSettings.Value);

        public async Task<List<CategoryDto>> GetAsync() => await _categoriesCollection.Find(_ => true).ToListAsync();

        public async Task<CategoryDto?> GetAsync(int id) => await _categoriesCollection.Find(x => x.CategoryId == id).FirstOrDefaultAsync();

        public async Task CreateAsync(CategoryDto newCategory) => await _categoriesCollection.InsertOneAsync(newCategory);

        public async Task UpdateAsync(int categoryId, CategoryDto updatedCategory) => await _categoriesCollection.ReplaceOneAsync(x => x.CategoryId == categoryId, updatedCategory);

        public async Task RemoveAsync(int categoryId) => await _categoriesCollection.DeleteOneAsync(x => x.CategoryId == categoryId);

        private static IMongoCollection<CategoryDto> InitializeMongoCollection(WarehouseDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
            return mongoDatabase.GetCollection<CategoryDto>(settings.CategoriesCollectionName);
        }
    }
}
