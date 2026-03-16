namespace SeaFood.Models;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Product? Product { get; set; }
}
