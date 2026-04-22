using Microsoft.EntityFrameworkCore;
using SalesOrderAPI.Domain.Entities;

namespace SalesOrderAPI.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Force exact table names - change these to match your MySQL table names exactly
            modelBuilder.Entity<Client>(entity => {
                entity.ToTable("client");
                entity.HasKey(e => e.ClientId);
            });

            modelBuilder.Entity<Item>(entity => {
                entity.ToTable("item");
                entity.HasKey(e => e.ItemId);
            });

            modelBuilder.Entity<SalesOrder>(entity => {
                entity.ToTable("salesorder");
                entity.HasKey(e => e.OrderId);
            });

            modelBuilder.Entity<OrderDetail>(entity => {
                entity.ToTable("orderdetail");
                entity.HasKey(e => e.DetailId);
            });
        }
    }
}