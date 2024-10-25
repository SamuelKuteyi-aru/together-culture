using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using together_culture_cambridge.Models;

namespace together_culture_cambridge.Data
{
    public class  ApplicationDatabaseContext : DbContext
    {
        public ApplicationDatabaseContext (DbContextOptions<ApplicationDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<together_culture_cambridge.Models.Membership> Membership { get; set; } = default!;
        public DbSet<together_culture_cambridge.Models.EndUser> EndUser { get; set; } = default!;
    }
}
