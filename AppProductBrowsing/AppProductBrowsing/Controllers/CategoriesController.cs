using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppProductBrowsing.Data;
using AppProductBrowsing.Models;
using System.Collections.Generic;
using AppProductBrowsing.Data.Entities;

namespace AppProductBrowsing.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DatabaseContext _context;

        public CategoriesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            List<Category> categoryEntities = await _context.Category.ToListAsync();
            List<CategoryViewModel> model = new();
            model.AddRange(
                categoryEntities
                    .Select(e => new CategoryViewModel
                    {
                        Id = e.Id,
                        Name = e.Name
                    }));

            return View(model);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryEntity = await _context
                .Category
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoryEntity == null)
            {
                return NotFound();
            }

            CategoryViewModel model = new()
            {
                Id = categoryEntity.Id,
                Name = categoryEntity.Name
            };

            return View(model);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] CategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    ModelState.AddModelError(
                        nameof(CategoryViewModel.Name),
                        "Category name is required");
                    return View(category);
                }

                Category categoryEntity = new()
                {
                    Name = category.Name
                };

                _context.Add(categoryEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryEntity = await _context.Category.FindAsync(id);
            if (categoryEntity == null)
            {
                return NotFound();
            }

            CategoryViewModel model = new()
            {
                Id = categoryEntity.Id,
                Name = categoryEntity.Name
            };

            return View(model);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CategoryViewModel category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    ModelState.AddModelError(
                        nameof(CategoryViewModel.Name),
                        "Category name is required");
                    return View(category);
                }

                try
                {
                    Category categoryEntity = new()
                    {
                        Id = category.Id,
                        Name = category.Name
                    };

                    _context.Update(categoryEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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

            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryEntity = await _context
                .Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryEntity == null)
            {
                return NotFound();
            }

            CategoryViewModel model = new()
            {
                Id = categoryEntity.Id,
                Name = categoryEntity.Name
            };

            return View(model);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryEntity = await _context.Category.FindAsync(id);
            _context.Category.Remove(categoryEntity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
