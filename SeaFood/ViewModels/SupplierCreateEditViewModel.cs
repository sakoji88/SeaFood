using System.ComponentModel.DataAnnotations;

namespace SeaFood.ViewModels;

public class SupplierCreateEditViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Длина должна быть от 2 до 150 символов")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(100, ErrorMessage = "Максимальная длина — 100 символов")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(30, ErrorMessage = "Максимальная длина — 30 символов")]
    [Phone(ErrorMessage = "Введите корректный номер телефона")]
    public string ContactPhone { get; set; } = string.Empty;
}
