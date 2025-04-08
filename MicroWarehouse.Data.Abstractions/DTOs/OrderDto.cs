using MongoDB.Bson.Serialization.Attributes;

namespace MicroWarehouse.Infrastructure.Abstractions.DTOs
{
    public class OrderDto
    {
        [BsonId]
        public int OrderId { get; set; }
        public int StatusId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
