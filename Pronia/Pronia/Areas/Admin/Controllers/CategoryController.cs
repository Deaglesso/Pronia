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
    }
}
