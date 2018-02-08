using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.Models
{
    public class Task
    {
        public string Id { get; set; }
        [Required]
        public string Project_id { get; set; }
        public int SeqNumber { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Priority { get; set; }
    }
}
