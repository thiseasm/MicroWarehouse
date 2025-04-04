using MongoDB.Bson.Serialization.Attributes;

namespace MicroWarehouse.Data.Abstractions.DTOs
{
    public class CategoryDto
    {
        [BsonId]
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public int LowStockThreshold { get; set; }
        public int OutOfStockThreshold { get; set; }
    }
}
