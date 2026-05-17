using All_Baby_Essentials.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using All_Baby_Essentials.Areas.Admin.ViewModels;

namespace All_Baby_Essentials.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<WishlistItem> WishlistItems { get; set; }

        public DbSet<ProductReview> ProductReviews { get; set; }

        public DbSet<Testimonial> Testimonials { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<ProductColor> ProductColors { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Unique Index (CartItem - WishListItem - OrderItem)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartItem>().HasIndex(c => new { c.UserId, c.SessionId, c.ProductId }).IsUnique();

            modelBuilder.Entity<WishlistItem>().HasIndex(w => new { w.UserId, w.SessionId, w.ProductId }).IsUnique();

            modelBuilder.Entity<OrderItem>().HasIndex(o => new { o.OrderId, o.ProductId }).IsUnique();
        }


    }
}
