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
    }
}