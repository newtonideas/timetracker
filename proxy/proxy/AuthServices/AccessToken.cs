using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.AuthServices
{
    public class AccessToken
    {
        [Key]
        public string Token { get; set; }
        [Required]
        public string SessionId { get; set; }
        [Required]
        public string FedAuth { get; set; }
        [Required]
        public string FedAuth1 { get; set; }

    }
}
