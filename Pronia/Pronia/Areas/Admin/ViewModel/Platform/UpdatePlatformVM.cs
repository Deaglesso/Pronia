﻿using Pronia.Entities;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.Admin.ViewModel
{
    public class UpdatePlatformVM
    {

        [Required]
        [MaxLength(25, ErrorMessage = "Max length is 25")]
        [MinLength(2, ErrorMessage = "Min length is 2")]
        public string Name { get; set; }
        public List<ProductPlatform>? ProductPlatforms { get; set; }
    }
}
