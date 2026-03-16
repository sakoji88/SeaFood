using System.ComponentModel.DataAnnotations;

namespace SeaFood.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты")]
    [StringLength(150, ErrorMessage = "Максимальная длина — 150 символов")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть длиной от 6 до 100 символов")]
    public string Password { get; set; } = string.Empty;
}
