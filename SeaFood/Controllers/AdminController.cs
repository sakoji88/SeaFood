using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaFood.Data;
using SeaFood.ViewModels;

namespace SeaFood.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Dashboard()
    {
        ViewBag.ProductsCount = await db.Products.CountAsync();
        ViewBag.UsersCount = await db.Users.CountAsync();
        ViewBag.ReviewsCount = await db.Reviews.CountAsync();
        return View();
    }

    public async Task<IActionResult> Users()
    {
        var users = await db.Users.Include(x => x.Role).OrderByDescending(x => x.CreatedAt)
            .Select(x => new UserManagementViewModel
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                Phone = x.Phone,
                RoleName = x.Role != null ? x.Role.Name : "-",
                CreatedAt = x.CreatedAt,
                IsActive = x.IsActive
            }).ToListAsync();

        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlockUser(int id)
    {
        // Блокировка реализована через поле IsActive = false.
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.IsActive = false;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnblockUser(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.IsActive = true;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PromoteToAdmin(int id)
    {
        // Проверка роли происходит через claims и атрибут [Authorize(Roles = "Admin")].
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();

        var adminRole = await db.Roles.FirstOrDefaultAsync(x => x.Name == "Admin");
        if (adminRole is null)
        {
            TempData["Error"] = "Роль Admin не найдена в базе";
            return RedirectToAction(nameof(Users));
        }

        user.RoleId = adminRole.Id;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Reviews()
    {
        var reviews = await db.Reviews
            .Include(x => x.User)
            .Include(x => x.Product)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        return View(reviews);
    }
}
