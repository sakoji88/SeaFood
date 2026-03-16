using System.ComponentModel.DataAnnotations;

namespace SeaFood.ViewModels;

public class ReviewCreateViewModel
{
    [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(1000, MinimumLength = 2, ErrorMessage = "Комментарий должен быть от 2 до 1000 символов")]
    public string Comment { get; set; } = string.Empty;

    public int ProductId { get; set; }
}
