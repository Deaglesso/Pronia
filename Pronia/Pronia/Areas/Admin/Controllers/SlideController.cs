using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;

        public SlideController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> SlideList = await _context.Slides.ToListAsync();

            return View(SlideList);
        }
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (slide.File.Length > 1024 * 1024 * 2)
            {
                ModelState.AddModelError("File", "Max file size is 2MB.");
                return View();
            }
            if(!slide.File.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("File", "Only image files supported.");
                return View();
            }
            using (FileStream fs = new FileStream(@$"D:\CFF\Pronia\Pronia\Pronia\wwwroot\assets\images\website-images\{slide.File.FileName}", FileMode.Create))
            {
                await slide.File.CopyToAsync(fs);
            }

            
            slide.Img = slide.File.FileName;
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int id)
        {
            Slide slide = await _context.Slides.FirstOrDefaultAsync(x => x.Id == id);
            return View(slide);
        }
    }
}
