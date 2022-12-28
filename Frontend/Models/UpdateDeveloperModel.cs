using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class UpdateDeveloperModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
