using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EditionController : Controller
    {
        private readonly AppDbContext _context;

        public EditionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Edition> editionList = await _context.Editions.Include(x => x.ProductEditions).ToListAsync();

            return View(editionList);
        }
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Edition edition)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = await _context.Editions.AnyAsync(x => x.Name == edition.Name);
            if (result)
            {
                ModelState.AddModelError("Name","Already exists.");
                return View();
            }
            await _context.Editions.AddAsync(edition);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
    }
}
