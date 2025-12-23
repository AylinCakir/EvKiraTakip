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
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<House>()
         .HasOne<User>()
         .WithMany()
         .HasForeignKey(x => x.UserId);

      modelBuilder.Entity<Tenant>()
         .HasOne<House>()
         .WithMany()
         .HasForeignKey(t => t.HouseId);
      
      modelBuilder.Entity<RentPayment>()
         .HasOne<Tenant>()
         .WithMany()
         .HasForeignKey(t => t.TenantId);
   }
}