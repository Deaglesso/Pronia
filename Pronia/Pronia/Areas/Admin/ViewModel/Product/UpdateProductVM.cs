using Pronia.Entities;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.Admin.ViewModel
{
    public class UpdateProductVM
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Max length is 25")]
        [MinLength(2, ErrorMessage = "Min length is 2")]
        public string Name { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Min length is 2")]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<Category>? CategoryList { get; set; }
    }
}
