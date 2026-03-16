using System.ComponentModel.DataAnnotations;

namespace SeaFood.ViewModels;

public class CategoryCreateEditViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Длина должна быть от 2 до 100 символов")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Максимальная длина — 500 символов")]
    public string? Description { get; set; }
}
