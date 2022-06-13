using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTExample.API.Models
{
    public class Customer
    {
        public int customerId { get; set; }
        public string email { get; set; }
        public string name { get; set; }
    }
}
