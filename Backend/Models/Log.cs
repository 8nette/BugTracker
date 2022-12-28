using System;
namespace Backend.Models
{
    public class Log
    {
        public int? Id { get; set; }

        public DateTime DateAndTime { get; set; }

        public int BugId { get; set; }

        public string Resolution { get; set; }
    }
}
