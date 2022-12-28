using System;
using System.Collections.Generic;
using Backend.Models;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class BugsModel
    {
        public IEnumerable<Bug> Bugs { get; set; }

        public string Title { get; set; }

        public string Area { get; set; }

        public string Category { get; set; }

        public string Product { get; set; }

        public string Release { get; set; }

        public string Developers { get; set; }

        public string Customers { get; set; }

        public string Resolution { get; set; }

        public bool AllResolutions { get; set; }

        public bool NotClosedResolutions { get; set; }

        public bool ClosedResolutions { get; set; }

        public string Priority { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime CreatedBefore { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime CreatedAfter { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }
    }
}
