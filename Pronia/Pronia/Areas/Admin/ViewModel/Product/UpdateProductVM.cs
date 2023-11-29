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
        public List<ProductImage>? ProductImages { get; set; }

        public List<int>? TagIds { get; set; }
        public List<int>? PlatformIds { get; set; }
        public List<int>? EditionIds { get; set; }
        public List<int>? ImageIds { get; set; }

        public IFormFile? MainImage { get; set; }
        public IFormFile? HoverImage { get; set; }
        public List<IFormFile>? AddImages { get; set; }

        public Category? Category { get; set; }

        public List<Category>? CategoryList { get; set; }

        public List<Tag>? TagList { get; set; }
        public List<Edition>? EditionList { get; set; }
        public List<Platform>? PlatformList { get; set; }

    }
}
