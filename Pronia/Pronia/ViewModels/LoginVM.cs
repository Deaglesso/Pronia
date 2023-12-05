using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class LoginVM
    {
        [Required]
        [MinLength(4)]
        public string UsernameorEmail { get; set; }
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool isRemembered { get; set; }
    }
}
