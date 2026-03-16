using System.ComponentModel.DataAnnotations;

namespace SeaFood.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Длина ФИО должна быть от 2 до 150 символов")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(150, ErrorMessage = "Максимальная длина — 150 символов")]
    [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(30, ErrorMessage = "Максимальная длина — 30 символов")]
    [Phone(ErrorMessage = "Введите корректный номер телефона")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть длиной от 6 до 100 символов")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
