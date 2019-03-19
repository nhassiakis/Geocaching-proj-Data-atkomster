using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Geocaching.Models;

namespace Geocaching
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> Person { get; set; }
        public DbSet<Geocache> Geocache { get; set; }
        public DbSet<FoundGeocache> FoundGeocache { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(local)\SQLEXPRESS;Initial Catalog=Geocaching;Integrated Security=True");
        }
    }
}
