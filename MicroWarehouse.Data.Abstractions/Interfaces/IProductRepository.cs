using MicroWarehouse.Data.Abstractions.DTOs;

namespace MicroWarehouse.Data.Abstractions.Interfaces
{
    public interface IProductRepository
    {
        public Task<List<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);

        public Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);

        public Task CreateAsync(ProductDto newProduct, CancellationToken cancellationToken = default);

        public Task<bool> UpdateAsync(int productId, ProductDto updatedProduct, CancellationToken cancellationToken = default);

        public Task<bool> UpdateStockAsync(int productId, int newQuantity, CancellationToken cancellationToken = default);
    }
}
