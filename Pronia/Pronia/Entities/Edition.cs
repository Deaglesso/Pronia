namespace Pronia.Entities
{
    public class Edition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductEdition> ProductEditions { get; set; }


    }
}
