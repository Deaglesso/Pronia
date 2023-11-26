using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Admin.ViewModel;
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
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool res = await _context.Tags.AnyAsync(x => x.Name == tagVM.Name);
            if (res) 
            {
                ModelState.AddModelError("Name", "Already exists.");
                return View();
            }
            Tag tag = new Tag
            {
                Name = tagVM.Name,
            };
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            TempData["ToasterMessage"] = $"{tag.Name} tag created successfully!";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int id)
        {

            if (id <= 0)
            {
                return BadRequest();
            }
            Tag tag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (tag == null)
            {
                return NotFound();
            }
            UpdateTagVM tagVM = new UpdateTagVM 
            {
                Name=tag.Name,
                ProductTags = tag.ProductTags,

            };
            
            return View(tagVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM newTag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Tag oldTag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (oldTag == null) return NotFound();

            bool result = await _context.Tags.AnyAsync(x => x.Name == newTag.Name && x.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This name already used in other Tag");
                return View();

            }
            oldTag.Name = newTag.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Tag Tag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (Tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(Tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int id)
        {
            Tag tag = await _context.Tags.Include(c => c.ProductTags).ThenInclude(p => p.Product).ThenInclude(i => i.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
            if (tag is null) return NotFound();
            return View(tag);
        }

    }
}
