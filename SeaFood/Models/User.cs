namespace SeaFood.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public Role? Role { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
