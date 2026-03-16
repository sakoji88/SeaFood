using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaFood.Data;

namespace SeaFood.Controllers;

public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var popularProducts = await db.Products
            .Where(x => x.IsActive)
            .Include(x => x.ProductImages)
            .OrderByDescending(x => x.CreatedAt)
            .Take(4)
            .ToListAsync();

        return View(popularProducts);
    }

    public IActionResult Privacy() => View();

    public IActionResult AccessDenied() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
