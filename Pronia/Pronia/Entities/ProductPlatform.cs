namespace Pronia.Entities
{
    public class ProductPlatform
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int PlatformId { get; set; }
        public Platform Platform { get; set; }

    }
}
