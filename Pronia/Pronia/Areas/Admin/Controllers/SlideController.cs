using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Admin.ViewModel;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.Utilities.Extensions;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context, IWebHostEnvironment env)
        {

            _context = context;
            _env = env;
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
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            
            if (!slideVM.File.CheckFileSize(2))
            {
                ModelState.AddModelError("File", "Max file size is 2MB.");
                return View();
            }
            if(!slideVM.File.CheckFileType("image"))
            {
                ModelState.AddModelError("File", "Only image files supported.");
                return View();
            }

            Slide slide = new Slide
            { 
                Name = slideVM.Name,
                Subname = slideVM.Subname,
                Description = slideVM.Description,
                Order = slideVM.Order,
                Img = await slideVM.File.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")
            };
            
            
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int id)
        {
            Slide slide = await _context.Slides.FirstOrDefaultAsync(x => x.Id == id);
            return View(slide);
        }

        public async Task<ActionResult> Delete(int id) 
        {
            
            Slide slide = await _context.Slides.FirstOrDefaultAsync(x => x.Id == id);
            if(slide is null) return NotFound();

            slide.Img.Delete(_env.WebRootPath, "assets", "images", "website-images");

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            UpdateSlideVM slideVM = new UpdateSlideVM 
            {
                Name = existed.Name,
                Subname = existed.Subname,
                Description = existed.Description,
                Order = existed.Order,
                Img = existed.Img,
                
            };

            return View(slideVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVM slideVM)
        {

            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(existed);
            }

            if (slideVM.File is not null)
            {
                bool result = _context.Slides.Any(s => s.Order < 0);
                if (result)
                {
                    ModelState.AddModelError("Order", "Order can't be smaller than 0.");
                    return View(existed);
                }

                if (!slideVM.File.CheckFileType("image"))
                {
                    ModelState.AddModelError("Photo", "You need to choose image file.");
                    return View(existed);
                }
                if (!slideVM.File.CheckFileSize(2))
                {
                    ModelState.AddModelError("Photo", "You need to choose up to 2MB.");
                    return View(existed);
                }
                string newimage = await slideVM.File.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                existed.Img.Delete(_env.WebRootPath, "assets", "images", "website-images");
                existed.Img = newimage;
            }

            existed.Name = slideVM.Name;
            existed.Subname = slideVM.Subname;
            existed.Description = slideVM.Description;
            existed.Order = slideVM.Order;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }

}
