namespace MicroWarehouse.Core.Abstractions.Models.Requests.Products
{
    public abstract class ProductRequestBase
    {
        public required string Name { get; set; }
        public required int CategoryId { get; set; }
    }
}
