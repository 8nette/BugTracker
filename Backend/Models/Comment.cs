using System;
namespace Backend.Models
{
    public class Comment
    {
        public int? Id { get; set; }

        public int? BugId { get; set; }

        public DateTime Created { get; set; }

        public int? DeveloperId { get; set; }

        public string _Comment { get; set; }
    }
}
