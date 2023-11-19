using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            if (id <= 0) 
            {
                return BadRequest();
            }
            Product product =_context.Products.Include(x=>x.Category).Include(x=>x.ProductImages).FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }


            ProductVM vm = new ProductVM 
            {
                Product=product,
                RelatedProducts=_context.Products.Where(p=>p.Category.Id == product.CategoryId && p.Id!=product.Id).Include(x => x.ProductImages).ToList(),
            };
            
            
            return View(vm);
        }
    }
}
