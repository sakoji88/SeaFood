using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaFood.Data;
using SeaFood.Helpers;
using SeaFood.Models;
using SeaFood.ViewModels;

namespace SeaFood.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriesController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Categories.OrderBy(x => x.Name).ToListAsync());

    public IActionResult Create() => View(new CategoryCreateEditViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryCreateEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        db.Categories.Add(new Category { Name = StringSanitizer.Clean(model.Name, 100), Description = StringSanitizer.Clean(model.Description, 500) });
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null) return NotFound();
        return View(new CategoryCreateEditViewModel { Id = category.Id, Name = category.Name, Description = category.Description });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryCreateEditViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        var category = await db.Categories.FindAsync(id);
        if (category is null) return NotFound();
        category.Name = StringSanitizer.Clean(model.Name, 100);
        category.Description = StringSanitizer.Clean(model.Description, 500);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null) return NotFound();
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null) return NotFound();
        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
