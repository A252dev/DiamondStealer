using DiamondStealer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondStealer
{
    internal class DiamondDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<UserAccess> UserAccess { get; set; }
        public DbSet<Promo> Promo { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=AGENT\DATABASE;Initial Catalog=diamond_db;Integrated Security=True;TrustServerCertificate=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
