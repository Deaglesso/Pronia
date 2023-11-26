using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Admin.ViewModel;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.Utilities.Extensions;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.Include(x=>x.ProductImages).Include(x=>x.Category).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create() 
        {
            CreateProductVM productVM = new CreateProductVM 
            {
                CategoryList = await _context.Categories.ToListAsync(),

            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.CategoryList = await _context.Categories.ToListAsync();
                return View(productVM);
            }

            Product product = new Product
            {
                Name = productVM.Name,
                Description = productVM.Description,
                Price = productVM.Price,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId,
                Category = productVM.Category,
                
                
            };


            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.FirstOrDefaultAsync(x=>x.Id == id);
            if (existed is null) return NotFound();
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = existed.Name,
                Description = existed.Description,
                Price = existed.Price,
                SKU = existed.SKU,
                CategoryId = existed.CategoryId,
                Category = existed.Category,
                CategoryList = await _context.Categories.ToListAsync(),             
            };

            return View(productVM);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateProductVM productVM) 
        {
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            Product existed = await _context.Products.FirstOrDefaultAsync(x=>x.Id == id);
            if (existed is null) return NotFound();

            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price;
            existed.SKU = productVM.SKU;
            existed.CategoryId = productVM.CategoryId;
            existed.Category = productVM.Category;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product is null) return NotFound();


            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _context.Products
                .Include(x=>x.Category)
                .Include(x=>x.ProductImages)
                .Include(x=>x.ProductTags)
                    .ThenInclude(x=>x.Tag)
                .Include(x=>x.ProductPlatforms)
                    .ThenInclude(x=>x.Platform)
                .Include(x=>x.ProductEditions)
                    .ThenInclude(x=>x.Edition)
                .FirstOrDefaultAsync(x => x.Id == id);
            return View(product);
        }



    }
}
