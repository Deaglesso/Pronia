using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> CategoryList = await _context.Categories.Include(x => x.Products).ToListAsync();

            return View(CategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category Category)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = await _context.Categories.AnyAsync(x => x.Name == Category.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Already exists.");
                return View();
            }
            await _context.Categories.AddAsync(Category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Update(int id)
        {

            if (id <= 0)
            {
                return BadRequest();
            }
            Category category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category oldCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (oldCategory == null) return NotFound();

            bool result = await _context.Categories.AnyAsync(x=>x.Name== newCategory.Name && x.Id != id);
            if(result)
            {
                ModelState.AddModelError("Name", "This name already used in other category");
                return View();

            }
            oldCategory.Name = newCategory.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Category category = await _context.Categories.FirstOrDefaultAsync(x=>x.Id==id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Detail(int id)
        {
            if(id <= 0)
            {
                return BadRequest();
            }
            Category category = await _context.Categories
            .Include(x => x.Products)
                .ThenInclude(x => x.ProductTags)
                    .ThenInclude(x => x.Tag)
            .Include(x => x.Products)
                .ThenInclude(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

    }
}
