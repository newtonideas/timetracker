using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.Models
{
    public class Timelog
    {
        public string Id { get; set; }
        [Required]
        public DateTime TimeStarted { get; set; }
        [Required]
        public DateTime TimeFinished { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
