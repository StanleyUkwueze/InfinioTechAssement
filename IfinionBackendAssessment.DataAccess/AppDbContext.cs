using IfinionBackendAssessment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
namespace IfinionBackendAssessment.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
