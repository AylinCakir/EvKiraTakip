using Microsoft.EntityFrameworkCore;
using EvKiraTakip.Models;

namespace EvKiraTakip;

public class AppDbContext : DbContext
{
   public AppDbContext (DbContextOptions<AppDbContext> options) : base(options){}
   
   public DbSet<User> Users { get; set; }
   public DbSet<House>  Houses { get; set; }
   public DbSet<Tenant> Tenants { get; set; }
   public DbSet<RentPayment>  RentPayments { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      
      modelBuilder.Entity<House>()
         .HasOne(h => h.User)
         .WithMany(u => u.Houses)
         .HasForeignKey(h => h.UserId)
         .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<Tenant>()
         .HasOne(t => t.House)
         .WithMany()
         .HasForeignKey(t => t.HouseId);
      
      modelBuilder.Entity<RentPayment>()
         .HasOne(r => r.Tenant)
         .WithMany()
         .HasForeignKey(r => r.TenantId);
      
      base.OnModelCreating(modelBuilder);
   }
}