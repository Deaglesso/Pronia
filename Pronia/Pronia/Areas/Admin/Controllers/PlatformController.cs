using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;

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
    }
}
