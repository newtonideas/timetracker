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
        public string Project_id { get; set; }
        [Required]
        public string User_id { get; set; }
        [Required]
        public string Task_id { get; set; }
        [Required]
        public DateTime Start_on { get; set; }
        [Required]
        public DateTime Finish_on { get; set; }
        [Required]
        public string Title { get; set; }
    }
}
