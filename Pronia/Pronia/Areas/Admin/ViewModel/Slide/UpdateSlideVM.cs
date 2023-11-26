namespace Pronia.Areas.Admin.ViewModel
{
    public class UpdateSlideVM
    {
        public string Subname { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public string? Img { get; set; }

        public int Order { get; set; }

        public IFormFile? File { get; set; }
    }
}
