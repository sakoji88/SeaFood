using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaFood.Data;
using SeaFood.Services;
using SeaFood.ViewModels;

namespace SeaFood.Controllers;

[Authorize]
public class CartController(AppDbContext db, ISessionCartService cartService) : Controller
{
    public IActionResult Index()
    {
        var cart = cartService.GetCart();
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddToCartViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Некорректное количество для добавления в корзину";
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }

        var product = await db.Products.FirstOrDefaultAsync(x => x.Id == model.ProductId && x.IsActive);
        if (product is null) return NotFound();

        if (product.StockKg <= 0)
        {
            TempData["Error"] = "Товар закончился";
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }

        cartService.AddOrUpdateItem(product.Id, product.Name, product.Price, model.QuantityKg, product.StockKg);
        TempData["Success"] = "Товар добавлен в демо-корзину";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        cartService.RemoveItem(productId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout()
    {
        var cart = cartService.GetCart();
        if (!cart.Items.Any())
        {
            TempData["Error"] = "Корзина пуста";
            return RedirectToAction(nameof(Index));
        }

        var productIds = cart.Items.Select(x => x.ProductId).ToList();
        var products = await db.Products.Where(x => productIds.Contains(x.Id)).ToListAsync();

        foreach (var item in cart.Items)
        {
            var product = products.FirstOrDefault(x => x.Id == item.ProductId);
            if (product is null)
            {
                TempData["Error"] = "Один из товаров не найден";
                return RedirectToAction(nameof(Index));
            }

            if (item.QuantityKg <= 0 || product.StockKg < item.QuantityKg)
            {
                TempData["Error"] = $"Недостаточно остатка для товара: {product.Name}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Демо-оплата: уменьшаем остаток в кг у купленных товаров.
        foreach (var item in cart.Items)
        {
            var product = products.First(x => x.Id == item.ProductId);
            product.StockKg -= item.QuantityKg;
        }

        await db.SaveChangesAsync();

        // После покупки разрешаем пользователю оставлять отзыв на эти товары.
        cartService.MarkPurchasedProducts(productIds);
        cartService.Clear();

        TempData["Success"] = "Демо-оплата успешна. Теперь вы можете оставить отзыв на купленные товары.";
        return RedirectToAction("Index", "Products");
    }
}
