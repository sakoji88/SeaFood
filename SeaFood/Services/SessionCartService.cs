using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SeaFood.ViewModels;

namespace SeaFood.Services;

public class SessionCartService(IHttpContextAccessor httpContextAccessor) : ISessionCartService
{
    private const string CartKey = "KacerFish.Cart";
    private const string PurchasedKey = "KacerFish.Purchased";

    public CartViewModel GetCart()
    {
        var session = httpContextAccessor.HttpContext?.Session;
        var json = session?.GetString(CartKey);
        return string.IsNullOrWhiteSpace(json) ? new CartViewModel() : JsonSerializer.Deserialize<CartViewModel>(json) ?? new CartViewModel();
    }

    public void SaveCart(CartViewModel cart)
    {
        httpContextAccessor.HttpContext?.Session.SetString(CartKey, JsonSerializer.Serialize(cart));
    }

    public void AddOrUpdateItem(int productId, string productName, decimal pricePerKg, decimal quantityKg, decimal availableKg)
    {
        var cart = GetCart();
        var existing = cart.Items.FirstOrDefault(x => x.ProductId == productId);

        if (quantityKg <= 0)
        {
            if (existing is not null) cart.Items.Remove(existing);
        }
        else
        {
            if (existing is null)
            {
                var clamped = Math.Min(quantityKg, availableKg);
                cart.Items.Add(new CartItemViewModel
                {
                    ProductId = productId,
                    ProductName = productName,
                    PricePerKg = pricePerKg,
                    QuantityKg = clamped,
                    AvailableKg = availableKg
                });
            }
            else
            {
                // Добавляем вес к уже выбранному товару, а не перезаписываем.
                existing.QuantityKg = Math.Min(existing.QuantityKg + quantityKg, availableKg);
                existing.AvailableKg = availableKg;
                existing.PricePerKg = pricePerKg;
                existing.ProductName = productName;
            }
        }

        SaveCart(cart);
    }

    public void RemoveItem(int productId)
    {
        var cart = GetCart();
        cart.Items.RemoveAll(x => x.ProductId == productId);
        SaveCart(cart);
    }

    public void Clear() => SaveCart(new CartViewModel());

    public bool HasPurchasedProduct(int productId)
    {
        var purchased = GetPurchasedSet();
        return purchased.Contains(productId);
    }

    public void MarkPurchasedProducts(IEnumerable<int> productIds)
    {
        var purchased = GetPurchasedSet();
        foreach (var id in productIds) purchased.Add(id);
        httpContextAccessor.HttpContext?.Session.SetString(PurchasedKey, JsonSerializer.Serialize(purchased));
    }

    private HashSet<int> GetPurchasedSet()
    {
        var json = httpContextAccessor.HttpContext?.Session.GetString(PurchasedKey);
        return string.IsNullOrWhiteSpace(json)
            ? []
            : JsonSerializer.Deserialize<HashSet<int>>(json) ?? [];
    }
}
