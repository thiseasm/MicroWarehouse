using MongoDB.Bson.Serialization.Attributes;

namespace MicroWarehouse.Data.Abstractions.DTOs
{
    public class OrderDto
    {
        [BsonId]
        public int OrderId { get; set; }
        public int Status { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = [];
    }
}
