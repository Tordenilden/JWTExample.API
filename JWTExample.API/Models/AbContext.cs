using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTExample.API.Models
{
    public class AbContext : DbContext
    {
        public DbContextOptions<AbContext> test { get; set; }
        public AbContext() { } // når vi skal teste vores repo...
        public AbContext(DbContextOptions<AbContext> flemse) : base(flemse) { test = flemse; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}
