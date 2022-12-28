using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class DateAndTasks
    {
        public int? ProductId { get; set; }

        public DateTime Date { get; set; }

        public string simpleDate { get; set; }

        public List<Task> Tasks { get; set; }
    }
}
