using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Entities
{
    public class Slide
    {
        public int Id { get; set; }
        
        public string Subname { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string? Img { get; set; }

        public int Order { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
