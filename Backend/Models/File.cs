using System;
namespace Backend.Models
{
    public class File
    {
        public int? Id { get; set; }

        public byte[] Contents { get; set; }

        public int? BugId { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }
    }
}
