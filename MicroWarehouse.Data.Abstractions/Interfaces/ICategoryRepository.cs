using MicroWarehouse.Infrastructure.Abstractions.DTOs;

namespace MicroWarehouse.Infrastructure.Abstractions.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default); 
        Task<CategoryDto?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default);
        Task CreateAsync(CategoryDto newCategory, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(CategoryDto updatedCategory, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}
