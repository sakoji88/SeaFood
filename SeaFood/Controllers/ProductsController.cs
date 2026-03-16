using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeaFood.Data;
using SeaFood.Helpers;
using SeaFood.Models;
using SeaFood.Services;
using SeaFood.ViewModels;

namespace SeaFood.Controllers;

public class ProductsController(AppDbContext db, ISessionCartService cartService) : Controller
{
    public async Task<IActionResult> Index(ProductFilterViewModel filter)
    {
        const int pageSize = 8;
        var query = db.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .Include(x => x.ProductImages)
            .Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(x => x.Name.Contains(term));
        }

        if (filter.CategoryId.HasValue) query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
        if (filter.SupplierId.HasValue) query = query.Where(x => x.SupplierId == filter.SupplierId.Value);
        if (filter.InStockOnly == true) query = query.Where(x => x.StockKg > 0);
        if (filter.MinPrice.HasValue) query = query.Where(x => x.Price >= filter.MinPrice.Value);
        if (filter.MaxPrice.HasValue) query = query.Where(x => x.Price <= filter.MaxPrice.Value);

        query = filter.SortBy switch
        {
            "name_asc" => query.OrderBy(x => x.Name),
            "name_desc" => query.OrderByDescending(x => x.Name),
            "price_asc" => query.OrderBy(x => x.Price),
            "price_desc" => query.OrderByDescending(x => x.Price),
            "date_asc" => query.OrderBy(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        filter.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        if (filter.Page < 1) filter.Page = 1;

        ViewBag.Filter = filter;
        ViewBag.Categories = await db.Categories.OrderBy(x => x.Name).ToListAsync();
        ViewBag.Suppliers = await db.Suppliers.OrderBy(x => x.Name).ToListAsync();

        var products = await query.Skip((filter.Page - 1) * pageSize).Take(pageSize).ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await db.Products
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .Include(x => x.ProductImages)
            .Include(x => x.Reviews).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product is null) return NotFound();

        product.Reviews = product.Reviews.Where(x => !ReviewModerationHelper.IsPending(x.Comment)).ToList();
        ViewBag.CanReview = User.Identity?.IsAuthenticated == true && cartService.HasPurchasedProduct(id);
        return View(product);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create() => View(await BuildProductVmAsync(new ProductCreateEditViewModel()));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateEditViewModel model)
    {
        ValidateTrimmed(model);
        if (!ModelState.IsValid) return View(await BuildProductVmAsync(model));

        var product = new Product
        {
            Name = StringSanitizer.Clean(model.Name, 200),
            Description = StringSanitizer.Clean(model.Description, 2000),
            Price = model.Price,
            StockKg = model.StockKg,
            UnitType = StringSanitizer.Clean(model.UnitType, 30),
            Origin = StringSanitizer.Clean(model.Origin, 100),
            StorageTemperature = StringSanitizer.Clean(model.StorageTemperature, 50),
            ShelfLifeDays = model.ShelfLifeDays,
            CategoryId = model.CategoryId,
            SupplierId = model.SupplierId,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(model.MainImageUrl))
        {
            db.ProductImages.Add(new ProductImage { ProductId = product.Id, ImageUrl = StringSanitizer.Clean(model.MainImageUrl, 500), IsMain = true });
            await db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await db.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) return NotFound();

        var model = new ProductCreateEditViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockKg = product.StockKg,
            UnitType = product.UnitType,
            Origin = product.Origin,
            StorageTemperature = product.StorageTemperature,
            ShelfLifeDays = product.ShelfLifeDays,
            CategoryId = product.CategoryId,
            SupplierId = product.SupplierId,
            IsActive = product.IsActive,
            MainImageUrl = product.ProductImages.FirstOrDefault(x => x.IsMain)?.ImageUrl
        };

        return View(await BuildProductVmAsync(model));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductCreateEditViewModel model)
    {
        if (id != model.Id) return BadRequest();

        ValidateTrimmed(model);
        if (!ModelState.IsValid) return View(await BuildProductVmAsync(model));

        var product = await db.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) return NotFound();

        product.Name = StringSanitizer.Clean(model.Name, 200);
        product.Description = StringSanitizer.Clean(model.Description, 2000);
        product.Price = model.Price;
        product.StockKg = model.StockKg;
        product.UnitType = StringSanitizer.Clean(model.UnitType, 30);
        product.Origin = StringSanitizer.Clean(model.Origin, 100);
        product.StorageTemperature = StringSanitizer.Clean(model.StorageTemperature, 50);
        product.ShelfLifeDays = model.ShelfLifeDays;
        product.CategoryId = model.CategoryId;
        product.SupplierId = model.SupplierId;
        product.IsActive = model.IsActive;

        var mainImage = product.ProductImages.FirstOrDefault(x => x.IsMain);
        var cleanedUrl = StringSanitizer.Clean(model.MainImageUrl, 500);

        if (string.IsNullOrWhiteSpace(cleanedUrl) && mainImage is not null) db.ProductImages.Remove(mainImage);
        else if (!string.IsNullOrWhiteSpace(cleanedUrl) && mainImage is null) db.ProductImages.Add(new ProductImage { ProductId = product.Id, ImageUrl = cleanedUrl, IsMain = true });
        else if (mainImage is not null) mainImage.ImageUrl = cleanedUrl;

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await db.Products.Include(x => x.Category).Include(x => x.Supplier).FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) return NotFound();
        return View(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return NotFound();
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<ProductCreateEditViewModel> BuildProductVmAsync(ProductCreateEditViewModel vm)
    {
        vm.Categories = await db.Categories.OrderBy(x => x.Name).Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToListAsync();
        vm.Suppliers = await db.Suppliers.OrderBy(x => x.Name).Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToListAsync();
        return vm;
    }

    private void ValidateTrimmed(ProductCreateEditViewModel model)
    {
        if (StringSanitizer.IsBlank(model.Name)) ModelState.AddModelError(nameof(model.Name), "Поле обязательно для заполнения");
        if (StringSanitizer.IsBlank(model.Description)) ModelState.AddModelError(nameof(model.Description), "Поле обязательно для заполнения");
    }
}
