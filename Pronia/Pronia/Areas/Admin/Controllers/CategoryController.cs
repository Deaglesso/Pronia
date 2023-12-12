using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Admin.ViewModel;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.Migrations;
using Pronia.ViewModels;

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
        public async Task<IActionResult> Index(int page = 1)
        {
            int limit = 4;
            double count = await _context.Categories.CountAsync();
            if (page > (int)Math.Ceiling(count / limit) || page <= 0)
            {
                return BadRequest();
            }
            List<Category> CategoryList = await _context.Categories.Skip((page - 1) * limit).Take(limit).Include(x => x.Products).ToListAsync();
            PaginationVM<Category> paginationVM = new PaginationVM<Category>
            {
                Items = CategoryList,
                TotalPage = (int)Math.Ceiling(count / limit),
                CurrentPage = page,
                Limit = limit
            };
            return View(paginationVM);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = await _context.Categories.AnyAsync(x => x.Name == categoryVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Already exists.");
                return View();
            }
            Category category = new Category { Name = categoryVM.Name };
            await _context.Categories.AddAsync(category);
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
            UpdateCategoryVM categoryVM = new UpdateCategoryVM 
            {
                Name = category.Name,
                Products = category.Products,
                
            };
            return View(categoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateCategoryVM newCategory)
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
