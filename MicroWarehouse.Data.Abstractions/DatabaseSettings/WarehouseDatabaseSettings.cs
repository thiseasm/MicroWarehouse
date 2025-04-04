
namespace MicroWarehouse.Data.Abstractions.DatabaseSettings
{
    public class WarehouseDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string ProductsCollectionName { get; set; } = null!;
        public string CategoriesCollectionName { get; set; } = null!;
        public string OrdersCollectionName { get; set; } = null!;
    }
}
