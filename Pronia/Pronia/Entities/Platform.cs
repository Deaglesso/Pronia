namespace Pronia.Entities
{
    public class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductPlatform> ProductPlatforms { get; set; }
    }
}
