using MongoDB.Bson.Serialization.Attributes;

namespace MicroWarehouse.Data.Abstractions.DTOs
{
    public class CounterDto
    {
        [BsonId]
        public string Id { get; set; } = null!;

        public int Sequence { get; set; }
    }
}
