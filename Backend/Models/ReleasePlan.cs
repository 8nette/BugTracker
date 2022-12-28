using System;
namespace Backend.Models
{
    public class ReleasePlan
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Objectives { get; set; }

        public string Workload { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public string ProductName { get; set; }
    }
}
