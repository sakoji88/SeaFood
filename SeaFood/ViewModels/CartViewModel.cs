namespace SeaFood.ViewModels;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = [];
    public decimal Total => Items.Sum(x => x.LineTotal);
}
