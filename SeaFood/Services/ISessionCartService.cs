using SeaFood.ViewModels;

namespace SeaFood.Services;

public interface ISessionCartService
{
    CartViewModel GetCart();
    void SaveCart(CartViewModel cart);
    void AddOrUpdateItem(int productId, string productName, decimal pricePerKg, decimal quantityKg, decimal availableKg);
    void RemoveItem(int productId);
    void Clear();
    bool HasPurchasedProduct(int productId);
    void MarkPurchasedProducts(IEnumerable<int> productIds);
}
