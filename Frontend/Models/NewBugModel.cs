using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Frontend.Models
{
    public class NewBugModel
    {
        public int? Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Area { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Product { get; set; }

        public string Release { get; set; }

        public string Developers { get; set; }

        public string Customers { get; set; }

        [Required]
        public string Priority { get; set; }
    }
}
