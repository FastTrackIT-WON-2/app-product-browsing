using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppProductBrowsing.Data;
using AppProductBrowsing.Models;
using AppProductBrowsing.Data.Entities;

namespace AppProductBrowsing.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DatabaseContext _context;

        public ProductsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            List<Product> productEntities = await _context
                .Product
                .Include(e => e.Category)
                .ToListAsync();

            List<ProductViewModel> models = new();
            models.AddRange(
                productEntities.Select(e => new ProductViewModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Price = e.Price,
                    Category = new()
                    {
                        Id = e.Category.Id,
                        Name = e.Category.Name
                    }
                }));

            return View(models);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productEntity = await _context
                .Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productEntity == null)
            {
                return NotFound();
            }

            ProductViewModel model = new()
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Price = productEntity.Price,
                Category = new()
                {
                    Id = productEntity.Category.Id,
                    Name = productEntity.Category.Name
                }
            };

            return View(model);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ProductViewModel model = new();
            await EnsureAvailableCategories(model);

            return View(model);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SelectedCategoryId,Name,Price")] ProductViewModel product)
        {
            if (!ModelState.IsValid)
            {
                await EnsureAvailableCategories(product);
                return View(product);
            }

            Category categoryEntity = await _context
                    .Category
                    .FirstOrDefaultAsync(c => c.Id == product.SelectedCategoryId);

            if (categoryEntity is null)
            {
                ModelState.AddModelError(
                    nameof(ProductViewModel.SelectedCategoryId),
                    "Category not found");

                await EnsureAvailableCategories(product);
                return View(product);
            }

            Product productEntity = new()
            {
                Name = product.Name,
                Price = product.Price,
                Category = categoryEntity
            };

            _context.Add(productEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Navigation property is not loading
            var productEntity = await _context
                .Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productEntity == null)
            {
                return NotFound();
            }

            ProductViewModel model = new()
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Price = productEntity.Price,
                Category = new()
                {
                    Id = productEntity.Category.Id,
                    Name = productEntity.Category.Name
                }
            };

            await EnsureAvailableCategories(model, productEntity.Category.Id);
            return View(model);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SelectedCategoryId,Name,Price")] ProductViewModel product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            Product originalProductEntity = await _context
                    .Product
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

            Category categoryEntity = await _context
                    .Category
                    .FirstOrDefaultAsync(c => c.Id == product.SelectedCategoryId);

            if (!ModelState.IsValid)
            {
                await EnsureAvailableCategories(
                    product,
                    categoryEntity?.Id ?? originalProductEntity.Category.Id);

                return View(product);
            }

            if (categoryEntity is null)
            {
                ModelState.AddModelError(
                    nameof(ProductViewModel.SelectedCategoryId),
                    "Category not found");

                await EnsureAvailableCategories(product, originalProductEntity.Category.Id);
                return View(product);
            }

            try
            {
                Product updatedProductEntity = originalProductEntity;
                if (!string.Equals(product.Name, updatedProductEntity.Name, StringComparison.OrdinalIgnoreCase))
                {
                    updatedProductEntity.Name = product.Name;
                }

                if (updatedProductEntity.Price != product.Price)
                {
                    updatedProductEntity.Price = product.Price;
                }

                if (updatedProductEntity.Category.Id != product.SelectedCategoryId)
                {
                    updatedProductEntity.Category = categoryEntity;
                }

                _context.Update(updatedProductEntity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productEntity = await _context
                .Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productEntity == null)
            {
                return NotFound();
            }

            ProductViewModel model = new()
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Price = productEntity.Price,
                Category = new()
                {
                    Id = productEntity.Category.Id,
                    Name = productEntity.Category.Name
                }
            };

            return View(model);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productEntity = await _context.Product.FindAsync(id);
            _context.Product.Remove(productEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        private async Task EnsureAvailableCategories(
            ProductViewModel viewModel,
            int? selectedCategoryId = null)
        {
            if (viewModel is not null)
            {
                if (selectedCategoryId.HasValue)
                {
                    viewModel.SelectedCategoryId = selectedCategoryId.GetValueOrDefault();
                }

                List<Category> categoryEntities = await _context.Category.ToListAsync();
                viewModel.AvailableCategories.AddRange(categoryEntities.Select(
                    e => new SelectListItem
                    {
                        Text = e.Name,
                        Value = e.Id.ToString(),
                        Selected = selectedCategoryId.HasValue &&
                                    selectedCategoryId.GetValueOrDefault() == e.Id
                    }));
            }
        }
    }
}
