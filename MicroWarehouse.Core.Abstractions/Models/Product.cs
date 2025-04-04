namespace MicroWarehouse.Core.Abstractions.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required Category Category { get; set; }
        //TODO: Add stock management properties
    }
}
