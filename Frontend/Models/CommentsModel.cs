using System;
using Backend.Models;
using System.Collections.Generic;

namespace Frontend.Models
{
    public class CommentsModel
    {
        public string BugTitle { get; set; }

        public Comment Comment { get; set; }

        public IEnumerable<(string, Comment)> Comments { get; set; }
    }
}
