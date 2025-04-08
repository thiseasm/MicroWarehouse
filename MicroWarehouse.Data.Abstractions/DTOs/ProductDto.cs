using MongoDB.Bson.Serialization.Attributes;

namespace MicroWarehouse.Infrastructure.Abstractions.DTOs
{
    public class ProductDto
    {
        [BsonId]
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public int StockAmount { get; set; }
        public int CategoryId { get; set; }
    }
}
