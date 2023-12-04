using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class RegisterVM
    {
        [System.ComponentModel.DataAnnotations.Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Surname { get; set; }
        public string Gender { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        [MinLength(4)]
        [MaxLength(25)]
        public string Username { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Password { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
