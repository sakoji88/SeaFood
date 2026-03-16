using System.ComponentModel.DataAnnotations;

namespace SeaFood.ViewModels;

public class AddToCartViewModel
{
    public int ProductId { get; set; }

    [Range(typeof(decimal), "0.1", "999999", ErrorMessage = "Количество должно быть больше 0")]
    public decimal QuantityKg { get; set; }
}
