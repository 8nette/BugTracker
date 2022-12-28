using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class EditTaskModel
    {
        public string Title { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime Start { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime End { get; set; }

        public string Bugs { get; set; }

        public int? ProductId { get; set; }
    }
}
