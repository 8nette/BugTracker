using System;
using System.Collections.Generic;
using Backend.Models;

namespace Frontend.Models
{
    public class RoadmapModel
    {
        public Product product { get; set; }

        public IEnumerable<Bug> productBugs { get; set; }

        public List<DateAndTasks> tasks { get; set; }
    }
}
