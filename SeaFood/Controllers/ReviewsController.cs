using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaFood.Data;
using SeaFood.Helpers;
using SeaFood.Models;
using SeaFood.Services;
using SeaFood.ViewModels;

namespace SeaFood.Controllers;

public class ReviewsController(AppDbContext db, ISessionCartService cartService) : Controller
{
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Проверьте корректность данных отзыва";
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }

        if (!cartService.HasPurchasedProduct(model.ProductId))
        {
            TempData["Error"] = "Отзыв можно оставить только после покупки товара";
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId)) return Forbid();

        var review = new Review
        {
            ProductId = model.ProductId,
            UserId = userId,
            Rating = model.Rating,
            // Модерация без изменения схемы: новый отзыв скрыт до одобрения администратором.
            Comment = ReviewModerationHelper.ToPending(StringSanitizer.Clean(model.Comment, 1000)),
            CreatedAt = DateTime.UtcNow
        };

        db.Reviews.Add(review);
        await db.SaveChangesAsync();
        TempData["Success"] = "Отзыв отправлен на модерацию администратору";
        return RedirectToAction("Details", "Products", new { id = model.ProductId });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var review = await db.Reviews.FirstOrDefaultAsync(x => x.Id == id);
        if (review is null) return NotFound();
        review.Comment = ReviewModerationHelper.Approve(review.Comment);
        await db.SaveChangesAsync();
        TempData["Success"] = "Отзыв одобрен";
        return RedirectToAction("Reviews", "Admin");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int productId)
    {
        var review = await db.Reviews.FirstOrDefaultAsync(x => x.Id == id);
        if (review is null) return NotFound();
        db.Reviews.Remove(review);
        await db.SaveChangesAsync();

        if (Request.Headers.Referer.ToString().Contains("/Admin/Reviews", StringComparison.OrdinalIgnoreCase))
            return RedirectToAction("Reviews", "Admin");

        return RedirectToAction("Details", "Products", new { id = productId });
    }
}
