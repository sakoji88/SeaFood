using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaFood.Data;
using SeaFood.Helpers;
using SeaFood.Models;
using SeaFood.ViewModels;

namespace SeaFood.Controllers;

[Authorize(Roles = "Admin")]
public class SuppliersController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Suppliers.OrderBy(x => x.Name).ToListAsync());

    public IActionResult Create() => View(new SupplierCreateEditViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierCreateEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        db.Suppliers.Add(new Supplier
        {
            Name = StringSanitizer.Clean(model.Name, 150),
            Country = StringSanitizer.Clean(model.Country, 100),
            ContactPhone = StringSanitizer.Clean(model.ContactPhone, 30)
        });
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await db.Suppliers.FindAsync(id);
        if (supplier is null) return NotFound();
        return View(new SupplierCreateEditViewModel
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Country = supplier.Country,
            ContactPhone = supplier.ContactPhone
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SupplierCreateEditViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var supplier = await db.Suppliers.FindAsync(id);
        if (supplier is null) return NotFound();

        supplier.Name = StringSanitizer.Clean(model.Name, 150);
        supplier.Country = StringSanitizer.Clean(model.Country, 100);
        supplier.ContactPhone = StringSanitizer.Clean(model.ContactPhone, 30);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await db.Suppliers.FindAsync(id);
        if (supplier is null) return NotFound();
        return View(supplier);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var supplier = await db.Suppliers.FindAsync(id);
        if (supplier is null) return NotFound();
        db.Suppliers.Remove(supplier);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
