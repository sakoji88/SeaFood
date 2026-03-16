using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SeaFood.ViewModels;

public class ProductCreateEditViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Длина должна быть от 2 до 200 символов")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(2000, MinimumLength = 5, ErrorMessage = "Длина должна быть от 5 до 2000 символов")]
    public string Description { get; set; } = string.Empty;

    [Range(0, 999999, ErrorMessage = "Цена должна быть больше либо равна 0")]
    public decimal Price { get; set; }

    [Range(0, 999999, ErrorMessage = "Количество не может быть отрицательным")]
    public decimal StockKg { get; set; }

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(30, ErrorMessage = "Максимальная длина — 30 символов")]
    public string UnitType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(100, ErrorMessage = "Максимальная длина — 100 символов")]
    public string Origin { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(50, ErrorMessage = "Максимальная длина — 50 символов")]
    public string StorageTemperature { get; set; } = string.Empty;

    [Range(0, 3650, ErrorMessage = "Срок хранения не может быть отрицательным")]
    public int ShelfLifeDays { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Выберите категорию")]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Выберите поставщика")]
    public int SupplierId { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(500, ErrorMessage = "Максимальная длина — 500 символов")]
    public string? MainImageUrl { get; set; }

    public List<SelectListItem> Categories { get; set; } = [];
    public List<SelectListItem> Suppliers { get; set; } = [];
}
