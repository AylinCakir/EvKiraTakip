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
      modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
      
      modelBuilder.Entity<House>()
         .HasQueryFilter(h => !h.IsDeleted)
         .HasOne(h => h.User)
         .WithMany(u => u.Houses)
         .HasForeignKey(h => h.UserId)
         .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<Tenant>()
         .HasQueryFilter(t=>!t.IsDeleted)
         .HasOne(t => t.House)
         .WithMany(h  => h.Tenants)
         .HasForeignKey(t => t.HouseId);
      
      modelBuilder.Entity<RentPayment>()
         .HasQueryFilter(r=>!r.IsDeleted)
         .HasOne(r => r.Tenant)
         .WithMany(t => t.RentPayments )
         .HasForeignKey(r => r.TenantId);
      
      base.OnModelCreating(modelBuilder);
   }
}