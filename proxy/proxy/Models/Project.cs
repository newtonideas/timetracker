using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.Models
{
    public class Project
    {
        public string Id { get; set; }
        [Required]
        public string Project_title { get; set; }
    }
}
