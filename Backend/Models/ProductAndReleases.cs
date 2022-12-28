using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class ProductAndReleases
    {
        public Product Product { get; set; }

        public IEnumerable<string> Releases { get; set; }
    }
}
