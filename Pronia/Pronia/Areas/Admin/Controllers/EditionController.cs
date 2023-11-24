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
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Edition Edition = await _context.Editions.FirstOrDefaultAsync(s => s.Id == id);

            if (Edition is null) return NotFound();

            return View(Edition);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, Edition Edition)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Edition existed = await _context.Editions.FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();

            bool result = _context.Editions.Any(c => c.Name == Edition.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Edition already exists");
                return View();
            }


            existed.Name = Edition.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Edition existed = await _context.Editions.FirstOrDefaultAsync(s => s.Id == id);

            if (existed is null) return NotFound();

            _context.Editions.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var Edition = await _context.Editions.Include(s => s.ProductEditions).ThenInclude(p => p.Product).ThenInclude(pi => pi.ProductImages).Include(x=>x.ProductEditions).ThenInclude(x=>x.Product).ThenInclude(x=>x.ProductTags).ThenInclude(x=>x.Tag).FirstOrDefaultAsync(x => x.Id == id);
            if (Edition is null) return NotFound();
            return View(Edition);
        }
    }
}
