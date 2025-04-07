using MicroWarehouse.Data.Abstractions.DatabaseSettings;
using MicroWarehouse.Data.Abstractions.DTOs;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MicroWarehouse.Data.Abstractions.Interfaces;

namespace MicroWarehouse.Data.Repositories
{
    public class CategoryRepository(IOptions<WarehouseDatabaseSettings> warehouseDatabaseSettings) : ICategoryRepository
    {
        private readonly IMongoCollection<CategoryDto> _categoriesCollection = InitializeMongoCollection(warehouseDatabaseSettings.Value);

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken) => await _categoriesCollection.Find(_ => true).ToListAsync(cancellationToken);

        public async Task<CategoryDto?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken) 
            => await _categoriesCollection.Find(x => x.CategoryId == categoryId).FirstOrDefaultAsync(cancellationToken);

        public async Task CreateAsync(CategoryDto newCategory, CancellationToken cancellationToken) => await _categoriesCollection.InsertOneAsync(newCategory, cancellationToken: cancellationToken);

        public async Task<bool> UpdateAsync(CategoryDto updatedCategory, CancellationToken cancellationToken)
        {
            var result = await _categoriesCollection.ReplaceOneAsync(x => x.CategoryId == updatedCategory.CategoryId, updatedCategory, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveAsync(int categoryId, CancellationToken cancellationToken)
        {
            var result = await _categoriesCollection.DeleteOneAsync(x => x.CategoryId == categoryId, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken)
        {
            var count = await _categoriesCollection.CountDocumentsAsync(x => x.CategoryId == categoryId, cancellationToken: cancellationToken);
            return count > 0;
        }

        private static IMongoCollection<CategoryDto> InitializeMongoCollection(WarehouseDatabaseSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
            return mongoDatabase.GetCollection<CategoryDto>(settings.CategoriesCollectionName);
        }
    }
}
