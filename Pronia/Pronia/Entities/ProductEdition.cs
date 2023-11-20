namespace Pronia.Entities
{
    public class ProductEdition
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int EditionId { get; set; }
        public Edition Edition { get; set; }
    }
}
