using MongoDB.Bson.Serialization.Attributes;

namespace MicroWarehouse.Infrastructure.Abstractions.DTOs
{
    public class CounterDto
    {
        [BsonId]
        [BsonElement("_id")]
        public string Id { get; set; } = null!;

        [BsonElement("sequence_value")]
        public int Sequence { get; set; }
    }
}
