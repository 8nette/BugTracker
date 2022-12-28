using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class NewTaskModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime Start { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime End { get; set; }

        public string Bugs { get; set; }

        public int? productId { get; set; }
    }
}
