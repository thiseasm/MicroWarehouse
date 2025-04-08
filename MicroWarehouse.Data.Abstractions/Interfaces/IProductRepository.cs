using MicroWarehouse.Infrastructure.Abstractions.DTOs;

namespace MicroWarehouse.Infrastructure.Abstractions.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);

        Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);
        Task CreateAsync(ProductDto newProduct, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(ProductDto updatedProduct, CancellationToken cancellationToken = default);
        Task<bool> UpdateStockAsync(int productId, int newStockAmount, CancellationToken cancellationToken = default);
        Task<bool> ReleaseStockAsync(int productId, int releasedQuantity, CancellationToken cancellationToken = default);
        Task<bool> ReserveStockAsync(int productId, int reservationQuantity, CancellationToken cancellationToken = default);
        Task<bool> ProductsWithCategoryExistAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}
