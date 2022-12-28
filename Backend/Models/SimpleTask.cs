using System;
namespace Backend.Models
{
    public class SimpleTask
    {
        public int? Id { get; set; }

        public int? ProductId { get; set; }

        public string Title { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string TaskBugs { get; set; }
    }
}
