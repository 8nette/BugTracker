using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class NewCustomerModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
