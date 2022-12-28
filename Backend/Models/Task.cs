using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Task
    {
        public int? Id { get; set; }

        public int? ProductId { get; set; }

        public string Title { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public IEnumerable<Bug> TaskBugs { get; set; }
    }
}
