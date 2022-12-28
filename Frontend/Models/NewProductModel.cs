using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class NewProductModel
    {
        [Required]
        public string Name { get; set; }
    }
}
