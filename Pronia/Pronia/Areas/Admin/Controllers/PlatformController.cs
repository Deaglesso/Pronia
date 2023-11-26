using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Admin.ViewModel;
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
        public async Task<IActionResult> Create(CreatePlatformVM platformVM)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = await _context.Platforms.AnyAsync(x => x.Name == platformVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Already exists.");
                return View();
            }
            Platform platform = new Platform { Name = platformVM.Name };

            await _context.Platforms.AddAsync(platform);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Platform platform = await _context.Platforms.FirstOrDefaultAsync(c => c.Id == id);

            if (platform is null) return NotFound();

            UpdatePlatformVM platformVM = new UpdatePlatformVM 
            {
                Name = platform.Name,
                ProductPlatforms = platform.ProductPlatforms,
            };

            return View(platformVM);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdatePlatformVM platformVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Platform existed = await _context.Platforms.FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();

            bool result = _context.Platforms.Any(c => c.Name == platformVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Platform already exists");
                return View();
            }


            existed.Name = platformVM.Name;
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
