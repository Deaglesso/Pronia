using System.ComponentModel.DataAnnotations;

namespace Pronia.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25, ErrorMessage = "Max length is 25")]
        [MinLength(2, ErrorMessage = "Min length is 2")]
        public string Name { get; set; }

        public List<ProductTag>? ProductTags { get; set; }
    }
}
