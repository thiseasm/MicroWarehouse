using MongoDB.Bson.Serialization.Attributes;

namespace MicroWarehouse.Data.Abstractions.DTOs
{
    public class OrderDto
    {
        [BsonId]
        public int OrderId { get; set; }
        public int Status { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
