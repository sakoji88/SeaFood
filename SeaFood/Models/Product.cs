namespace SeaFood.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal StockKg { get; set; }
    public string UnitType { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string StorageTemperature { get; set; } = string.Empty;
    public int ShelfLifeDays { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public Category? Category { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
