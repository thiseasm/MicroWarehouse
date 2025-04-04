namespace MicroWarehouse.Core.Abstractions.Models.Responses
{
    public class Error
    {
        public required string Message { get; set; }
        public string? Details { get; set; }
        public string? Code { get; set; }
    }
}
