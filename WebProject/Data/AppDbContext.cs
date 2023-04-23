using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebProject.Models;

namespace WebProject.Data
{
    public class AppDbContext : IdentityDbContext<user>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<order_product>().HasKey(x => new { x.orderid, x.productid });
            modelBuilder.Entity<order_product>().HasOne(m => m.product).WithMany(am => am.order_products).HasForeignKey(m => m.productid);
            modelBuilder.Entity<order_product>().HasOne(m => m.order).WithMany(am => am.order_products).HasForeignKey(m => m.orderid);

            modelBuilder.Entity<order>()
                .HasOne(o => o.users)
                .WithMany(u => u.orders)
                .HasForeignKey(o => o.userid)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<order> orders { get; set; }
        public DbSet<order_product> order_products { get; set; }
        public DbSet<orderDetail> orderDetails { get; set; }
        public DbSet<product> products { get; set; }
        public DbSet<user> users { get; set; }
    }
}
