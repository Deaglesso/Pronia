using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;
using System.Drawing;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PlatformController : Controller
    {
        private readonly AppDbContext _context;

        public PlatformController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Platform> PlatformList = await _context.Platforms.Include(x => x.ProductPlatforms).ToListAsync();

            return View(PlatformList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Platform platform)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = await _context.Platforms.AnyAsync(x => x.Name == platform.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Already exists.");
                return View();
            }
            await _context.Platforms.AddAsync(platform);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Platform Platform = await _context.Platforms.FirstOrDefaultAsync(c => c.Id == id);

            if (Platform is null) return NotFound();

            return View(Platform);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, Platform Platform)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Platform existed = await _context.Platforms.FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();

            bool result = _context.Platforms.Any(c => c.Name == Platform.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Platform already exists");
                return View();
            }


            existed.Name = Platform.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Platform existed = await _context.Platforms.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            _context.Platforms.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            Platform Platform = await _context.Platforms.Include(c => c.ProductPlatforms).ThenInclude(p => p.Product).ThenInclude(pi => pi.ProductImages).Include(x => x.ProductPlatforms).ThenInclude(x => x.Product).ThenInclude(x => x.ProductTags).ThenInclude(x => x.Tag).FirstOrDefaultAsync(x => x.Id == id);
            if (Platform is null) return NotFound();
            return View(Platform);
        }

    }
}
