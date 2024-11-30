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
        public DbSet<together_culture_cambridge.Models.Discount> Discount { get; set; } = default!;
        public DbSet<together_culture_cambridge.Models.DiscountUser> DiscountUser { get; set; } = default!;
        public DbSet<together_culture_cambridge.Models.Admin> Admin { get; set; } = default!;
        public DbSet<together_culture_cambridge.Models.Event> Event { get; set; } = default!;
        public DbSet<together_culture_cambridge.Models.EventBooking> EventBooking { get; set; } = default!;
        public DbSet<together_culture_cambridge.Models.Space> Space { get; set; } = default!;
        public DbSet<together_culture_cambridge.Models.SpaceBooking> SpaceBooking { get; set; } = default!;
    }
}
