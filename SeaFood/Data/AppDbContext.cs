using Microsoft.EntityFrameworkCore;
using SeaFood.Models;

namespace SeaFood.Data;

// DbContext описывает, как C#-сущности связаны с таблицами существующей БД SeafoodStore.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(30).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("datetime2");
            entity.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Suppliers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Country).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ContactPhone).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).IsRequired();
            entity.Property(x => x.Price).HasColumnType("decimal(18,2)");
            entity.Property(x => x.StockKg).HasColumnType("decimal(18,2)");
            entity.Property(x => x.UnitType).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Origin).HasMaxLength(100).IsRequired();
            entity.Property(x => x.StorageTemperature).HasMaxLength(50).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("datetime2");
            entity.HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId);
            entity.HasOne(x => x.Supplier).WithMany(x => x.Products).HasForeignKey(x => x.SupplierId);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("ProductImages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            entity.HasOne(x => x.Product).WithMany(x => x.ProductImages).HasForeignKey(x => x.ProductId);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Reviews");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Comment).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("datetime2");
            entity.HasOne(x => x.User).WithMany(x => x.Reviews).HasForeignKey(x => x.UserId);
            entity.HasOne(x => x.Product).WithMany(x => x.Reviews).HasForeignKey(x => x.ProductId);
        });
    }
}
