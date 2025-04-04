using MicroWarehouse.Data.Abstractions.DTOs;

namespace MicroWarehouse.Data.Abstractions.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
        public Task<CategoryDto?> GetAsync(int categoryId, CancellationToken cancellationToken = default);
        public Task CreateAsync(CategoryDto newCategory, CancellationToken cancellationToken = default);
        public Task<bool> UpdateAsync(int categoryId, CategoryDto updatedCategory, CancellationToken cancellationToken = default);
        public Task<bool> RemoveAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}
