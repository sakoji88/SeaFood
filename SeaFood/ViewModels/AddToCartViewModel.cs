using System.ComponentModel.DataAnnotations;

namespace SeaFood.ViewModels;

public class AddToCartViewModel
{
    public int ProductId { get; set; }

    // Используем double-константы в Range, чтобы не ловить FormatException на разных культурах.
    [Range(0.1, 999999, ErrorMessage = "Количество должно быть больше 0")]
    public decimal QuantityKg { get; set; }
}
