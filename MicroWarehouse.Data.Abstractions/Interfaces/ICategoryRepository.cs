using MicroWarehouse.Data.Abstractions.DTOs;

namespace MicroWarehouse.Data.Abstractions.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default); 
        Task<CategoryDto?> GetAsync(int categoryId, CancellationToken cancellationToken = default);
        Task CreateAsync(CategoryDto newCategory, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(CategoryDto updatedCategory, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}
