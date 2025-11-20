using Microsoft.EntityFrameworkCore;
using EvKiraTakip.Models;

namespace EvKiraTakip;

public class AppDbContext : DbContext
{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Payment> Payments { get; set; }
        

}