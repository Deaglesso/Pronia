using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Tag> tagList = await _context.Tags.Include(x=>x.ProductTags).ToListAsync();
            return View(tagList);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool res = await _context.Tags.AnyAsync(x => x.Name == tag.Name);
            if (res) 
            {
                ModelState.AddModelError("Name", "Already exists.");
                return View();
            }
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            TempData["ToasterMessage"] = $"{tag.Name} tag created successfully!";
            return RedirectToAction("Index");
        }
    }
}
