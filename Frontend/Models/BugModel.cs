using System;
using System.Collections.Generic;
using Backend.Models;

namespace Frontend.Models
{
    public class BugModel
    {
        public Bug Bug { get; set; }

        public IEnumerable<File> Files { get; set; }
    }
}
