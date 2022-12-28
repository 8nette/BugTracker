using System;

namespace Backend.Models
{
    public class Bug
    {
        public int? Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Area { get; set; }

        public string Category { get; set; }

        public string Product { get; set; }

        public string Release { get; set; }

        public string Developers { get; set; }

        public string Customers { get; set; }

        public string Resolution { get; set; }

        public string Priority { get; set; }

        public DateTime Created { get; set; }
    }
}
