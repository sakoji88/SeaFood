namespace SeaFood.ViewModels;

public class ProductFilterViewModel
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? SupplierId { get; set; }
    public bool? InStockOnly { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string SortBy { get; set; } = "date_desc";
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
}
