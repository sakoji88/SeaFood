namespace SeaFood.ViewModels;

public class CartItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal PricePerKg { get; set; }
    public decimal QuantityKg { get; set; }
    public decimal AvailableKg { get; set; }
    public decimal LineTotal => PricePerKg * QuantityKg;
}
