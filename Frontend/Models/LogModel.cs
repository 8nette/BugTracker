using System;
using System.Collections.Generic;
using Backend.Models;

namespace Frontend.Models
{
    public class LogModel
    {
        public int? bugId { get; set; }

        public string bugTitle { get; set; }

        public IEnumerable<Log> logs { get; set; }
    }
}
