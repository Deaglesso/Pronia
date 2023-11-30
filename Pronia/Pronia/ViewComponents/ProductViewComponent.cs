using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;

namespace Pronia.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public ProductViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int key=1)
        {
            List<Product> products = new List<Product>();

            switch (key)
            {
                case 1:
                    products = await _context.Products.OrderBy(x=>x.Name).Take(8).Include(x=>x.ProductImages.Where(x=>x.IsPrimary!=null)).ToListAsync();
                    break;
                case 2:
                    products = await _context.Products.OrderByDescending(x => x.Price).Take(8).Include(x => x.ProductImages.Where(x => x.IsPrimary != null)).ToListAsync();

                    break;
                case 3:
                    products = await _context.Products.OrderByDescending(x => x.Id).Take(8).Include(x => x.ProductImages.Where(x => x.IsPrimary != null)).ToListAsync();

                    break;
                default:
                    break;
            }

            return View(products);
        }
    }
}
