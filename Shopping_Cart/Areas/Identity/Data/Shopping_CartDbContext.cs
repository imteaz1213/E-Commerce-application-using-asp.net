using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart.Models;

namespace Shopping_Cart.Areas.Identity.Data;

public class Shopping_CartDbContext : IdentityDbContext<IdentityUser>
{
    public Shopping_CartDbContext(DbContextOptions<Shopping_CartDbContext> options)
        : base(options)
    {
    }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<CartDetail> CartDetails { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<OrderStatus> orderStatuses { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
