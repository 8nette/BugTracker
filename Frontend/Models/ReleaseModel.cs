using System;
using System.Collections.Generic;
using Backend.Models;

namespace Frontend.Models
{
    public class ReleaseModel
    {
        public Product product { get; set; }

        public string release { get; set; }

        public ReleasePlan releasePlan { get; set; }

        public IEnumerable<Bug> features { get; set; }

        public IEnumerable<Bug> bugs { get; set; }

        public IEnumerable<Task> tasks { get; set; }
    }
}
