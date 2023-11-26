using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Areas.Admin.ViewModel
{
    public class CreateSlideVM
    {

        public string Subname { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }

        public IFormFile File { get; set; }
    }
}
