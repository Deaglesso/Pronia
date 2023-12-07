using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Admin.ViewModel;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.Utilities.Extensions;
using Pronia.ViewModels;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.Include(x=>x.ProductImages).Include(x=>x.Category).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create() 
        {
            CreateProductVM productVM = new CreateProductVM 
            {
                CategoryList = await _context.Categories.ToListAsync(),
                TagList = await _context.Tags.ToListAsync(),
                EditionList = await _context.Editions.ToListAsync(),
                PlatformList = await _context.Platforms.ToListAsync(),

            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.CategoryList = await _context.Categories.ToListAsync();

                productVM.TagList = await _context.Tags.ToListAsync();
                productVM.EditionList = await _context.Editions.ToListAsync();
                productVM.PlatformList = await _context.Platforms.ToListAsync();
                return View(productVM);
            }


            if(!(await _context.Categories.AnyAsync(x=>x.Id == productVM.CategoryId)))
            {
                productVM.CategoryList = await _context.Categories.ToListAsync();
                productVM.TagList = await _context.Tags.ToListAsync();
                productVM.EditionList = await _context.Editions.ToListAsync();
                productVM.PlatformList = await _context.Platforms.ToListAsync();
                ModelState.AddModelError("CategoryId", "This category does not exist.");

                return View(productVM);
            }
            if (productVM.TagIds is not null)
            {
                foreach (var item in productVM.TagIds)
                {
                    if (!(await _context.Tags.AnyAsync(x => x.Id == item)))
                    {
                        productVM.CategoryList = await _context.Categories.ToListAsync();
                        productVM.TagList = await _context.Tags.ToListAsync();
                        productVM.EditionList = await _context.Editions.ToListAsync();
                        productVM.PlatformList = await _context.Platforms.ToListAsync();
                        ModelState.AddModelError("TagIds", "These tags does not exist.");

                        return View(productVM);
                    }
                }
            }
            if (!productVM.MainImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("MainImage","Only images allowed.");
                productVM.CategoryList = await _context.Categories.ToListAsync();
                productVM.TagList = await _context.Tags.ToListAsync();
                productVM.EditionList = await _context.Editions.ToListAsync();
                productVM.PlatformList = await _context.Platforms.ToListAsync();
                return View(productVM);
            }
            if (!productVM.MainImage.CheckFileSize(1))
            {
                ModelState.AddModelError("MainImage", "Only images below 1MB allowed.");
                productVM.CategoryList = await _context.Categories.ToListAsync();
                productVM.TagList = await _context.Tags.ToListAsync();
                productVM.EditionList = await _context.Editions.ToListAsync();
                productVM.PlatformList = await _context.Platforms.ToListAsync();
                return View(productVM);
            }

            if (!productVM.HoverImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("HoverImage", "Only images allowed.");
                productVM.CategoryList = await _context.Categories.ToListAsync();
                productVM.TagList = await _context.Tags.ToListAsync();
                productVM.EditionList = await _context.Editions.ToListAsync();
                productVM.PlatformList = await _context.Platforms.ToListAsync();
                return View(productVM);
            }
            if (!productVM.HoverImage.CheckFileSize(1))
            {
                ModelState.AddModelError("HoverImage", "Only images below 1MB allowed.");
                productVM.CategoryList = await _context.Categories.ToListAsync();
                productVM.TagList = await _context.Tags.ToListAsync();
                productVM.EditionList = await _context.Editions.ToListAsync();
                productVM.PlatformList = await _context.Platforms.ToListAsync();
                return View(productVM);
            }

            ProductImage mainImage = new ProductImage
            {
                IsPrimary = true,
                Url = await productVM.MainImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
            };
            ProductImage hoverImage = new ProductImage
            {
                IsPrimary = false,
                Url = await productVM.HoverImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
            };

            Product product = new Product
            {
                Name = productVM.Name,
                Description = productVM.Description,
                Price = productVM.Price,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId,
                Category = productVM.Category,
                ProductTags = new List<ProductTag>(),
                ProductEditions = new List<ProductEdition>(),
                ProductPlatforms = new List<ProductPlatform>(),
                ProductImages = new List<ProductImage> { mainImage,hoverImage }
                
            };
            TempData["ImageMessage"] = "";
            foreach (IFormFile image in productVM.AddImages)
            {
                if (!image.CheckFileType("image/"))
                {
                    TempData["ImageMessage"] += $" <p class=\"btn btn-inverse-danger btn-fw myParagraph\" style=\"display: inline-flex; align-items: center;\" >{image.FileName} file's type is not image.<span style=\"margin-top: -1px; margin-left: 3px\" class=\"close-button text-white\" onclick=\"closeParagraph()\"><i class=\"mdi mdi-close-circle-outline\"></i></span></p>\r\n\r\n                    <script> function closeParagraph() {{ var paragraphs = document.getElementsByClassName(\"myParagraph\");  for (var i = 0; i < paragraphs.length; i++) {{ paragraphs[i].style.display = \"none\"; }} }} setTimeout(closeParagraph, 10000); </script>";
                    continue;
                }
                if (!image.CheckFileSize(1))
                {
                    TempData["ImageMessage"] += $" <p class=\"btn btn-inverse-danger btn-fw myParagraph\" style=\"display: inline-flex; align-items: center;\" >{image.FileName} file's size is larger than 1MB.<span style=\"margin-top: -1px; margin-left: 3px\" class=\"close-button text-white\" onclick=\"closeParagraph()\"><i class=\"mdi mdi-close-circle-outline\"></i></span></p>\r\n\r\n                    <script> function closeParagraph() {{ var paragraphs = document.getElementsByClassName(\"myParagraph\");  for (var i = 0; i < paragraphs.length; i++) {{ paragraphs[i].style.display = \"none\"; }} }} setTimeout(closeParagraph, 10000); </script>";

                    continue;
                }

                product.ProductImages.Add(new ProductImage { IsPrimary = null, Url = await image.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images") });
            }
            if (productVM.TagIds is not null)
            {
                foreach (var item in productVM.TagIds)
                {
                    ProductTag productTag = new ProductTag
                    {
                        TagId = item
                    };
                    product.ProductTags.Add(productTag);
                }
            }
            if (productVM.EditionIds is not null)
            {
                foreach (var item in productVM.EditionIds)
                {
                    ProductEdition productEdition = new ProductEdition
                    {
                        EditionId = item
                    };
                    product.ProductEditions.Add(productEdition);
                }

            }
            if (productVM.PlatformIds is not null)
            {
                foreach (var item in productVM.PlatformIds)
                {
                    ProductPlatform productPlatform = new ProductPlatform
                    {
                        PlatformId = item
                    };
                    product.ProductPlatforms.Add(productPlatform);
                }
            }
            


            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.Include(x=>x.ProductTags).Include(x=>x.ProductEditions).Include(x=>x.ProductPlatforms).Include(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id == id);
            if (existed is null) return NotFound();
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = existed.Name,
                Description = existed.Description,
                Price = existed.Price,
                SKU = existed.SKU,
                CategoryId = existed.CategoryId,
                Category = existed.Category,
                ProductImages = existed.ProductImages,
                TagIds = existed.ProductTags.Select(x=>x.TagId).ToList(),
                EditionIds = existed.ProductEditions.Select(x=>x.EditionId).ToList(),
                PlatformIds = existed.ProductPlatforms.Select(x=>x.PlatformId).ToList(),
                CategoryList = await _context.Categories.ToListAsync(),      
                TagList = await _context.Tags.ToListAsync(),
                EditionList = await _context.Editions.ToListAsync(),
                PlatformList = await _context.Platforms.ToListAsync(),
                
            };

            return View(productVM);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateProductVM productVM) 
        {
            Product existed = await _context.Products.Include(x=>x.ProductTags).Include(x => x.ProductEditions).Include(x => x.ProductPlatforms).Include(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id == id);
            if (!ModelState.IsValid)
            {
                productVM.CategoryList = await _context.Categories.ToListAsync();
                productVM.EditionList = await _context.Editions.ToListAsync();
                productVM.PlatformList = await _context.Platforms.ToListAsync();
                productVM.TagList = await _context.Tags.ToListAsync();
                productVM.ProductImages = existed.ProductImages;
                return View(productVM);
            }
            if (existed is null) return NotFound();
            if (!(await _context.Categories.AnyAsync(x => x.Id == productVM.CategoryId)))
            {
                productVM.CategoryList = await _context.Categories.ToListAsync();
                productVM.EditionList = await _context.Editions.ToListAsync();
                productVM.PlatformList = await _context.Platforms.ToListAsync();
                productVM.TagList = await _context.Tags.ToListAsync();
                productVM.ProductImages = existed.ProductImages;

                ModelState.AddModelError("CategoryId", "This category does not exist.");
                 
                return View(productVM);
            }

            if (productVM.MainImage is not null)
            {
                if (!productVM.MainImage.CheckFileType("image/"))
                {
                    ModelState.AddModelError("MainImage", "Only images allowed.");
                    productVM.CategoryList = await _context.Categories.ToListAsync();
                    productVM.TagList = await _context.Tags.ToListAsync();
                    productVM.EditionList = await _context.Editions.ToListAsync();
                    productVM.PlatformList = await _context.Platforms.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;

                    return View(productVM);
                }
                if (!productVM.MainImage.CheckFileSize(1))
                {
                    ModelState.AddModelError("MainImage", "Only images below 1MB allowed.");
                    productVM.CategoryList = await _context.Categories.ToListAsync();
                    productVM.TagList = await _context.Tags.ToListAsync();
                    productVM.EditionList = await _context.Editions.ToListAsync();
                    productVM.PlatformList = await _context.Platforms.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;

                    return View(productVM);
                }

                
            }
            if (productVM.HoverImage is not null)
            {
                if (!productVM.HoverImage.CheckFileType("image/"))
                {
                    ModelState.AddModelError("HoverImage", "Only images allowed.");
                    productVM.CategoryList = await _context.Categories.ToListAsync();
                    productVM.TagList = await _context.Tags.ToListAsync();
                    productVM.EditionList = await _context.Editions.ToListAsync();
                    productVM.PlatformList = await _context.Platforms.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;

                    return View(productVM);
                }
                if (!productVM.HoverImage.CheckFileSize(1))
                {
                    ModelState.AddModelError("HoverImage", "Only images below 1MB allowed.");
                    productVM.CategoryList = await _context.Categories.ToListAsync();
                    productVM.TagList = await _context.Tags.ToListAsync();
                    productVM.EditionList = await _context.Editions.ToListAsync();
                    productVM.PlatformList = await _context.Platforms.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;

                    return View(productVM);
                }
            }

            





            //tag
            _context.ProductTags.RemoveRange(existed.ProductTags.Where(x => productVM.TagIds == null || !productVM.TagIds.Contains(x.TagId)));



            if (productVM.TagIds is not null)
            {
                foreach (var item in productVM.TagIds)
                {
                    if (!existed.ProductTags.Exists(x => x.TagId == item))
                    {
                        if (!(await _context.Tags.AnyAsync(x => x.Id == item)))
                        {
                            productVM.CategoryList = await _context.Categories.ToListAsync();
                            productVM.EditionList = await _context.Editions.ToListAsync();
                            productVM.PlatformList = await _context.Platforms.ToListAsync();
                            productVM.TagList = await _context.Tags.ToListAsync();
                            productVM.ProductImages = existed.ProductImages;

                            ModelState.AddModelError("TagIds", "These tags does not exist.");

                            return View(productVM);
                        }
                        ProductTag productTag = new ProductTag
                        {
                            TagId = item
                        };
                        existed.ProductTags.Add(productTag);
                    }
                }
            }
            
            //edition
            _context.ProductEditions.RemoveRange(existed.ProductEditions.Where(x => productVM.EditionIds == null || !productVM.EditionIds.Contains(x.EditionId)));


            if (productVM.EditionIds is not null)
            {
                foreach (var item in productVM.EditionIds)
                {
                    if (!existed.ProductEditions.Exists(x => x.EditionId == item))
                    {
                        if (!(await _context.Editions.AnyAsync(x => x.Id == item)))
                        {
                            productVM.CategoryList = await _context.Categories.ToListAsync();
                            productVM.EditionList = await _context.Editions.ToListAsync();
                            productVM.PlatformList = await _context.Platforms.ToListAsync();
                            productVM.TagList = await _context.Tags.ToListAsync();
                            productVM.ProductImages = existed.ProductImages;

                            ModelState.AddModelError("EditionIds", "These editions does not exist.");

                            return View(productVM);
                        }
                        ProductEdition productEdition = new ProductEdition
                        {
                            EditionId = item,
                        };
                        existed.ProductEditions.Add(productEdition);
                    }
                }
            }

            //platform

            _context.ProductPlatforms.RemoveRange(existed.ProductPlatforms.Where(x => productVM.PlatformIds == null || !productVM.PlatformIds.Contains(x.PlatformId)));


            if (productVM.PlatformIds is not null)
            {
                foreach (var item in productVM.PlatformIds)
                {
                    if (!existed.ProductPlatforms.Exists(x => x.PlatformId == item))
                    {
                        if (!(await _context.Platforms.AnyAsync(x => x.Id == item)))
                        {
                            productVM.CategoryList = await _context.Categories.ToListAsync();
                            productVM.EditionList = await _context.Editions.ToListAsync();
                            productVM.PlatformList = await _context.Platforms.ToListAsync();
                            productVM.TagList = await _context.Tags.ToListAsync();
                            productVM.ProductImages = existed.ProductImages;

                            ModelState.AddModelError("PlatformIds", "These editions does not exist.");

                            return View(productVM);
                        }
                        ProductPlatform ProductPlatform = new ProductPlatform
                        {
                            PlatformId = item,
                        };
                        existed.ProductPlatforms.Add(ProductPlatform);
                    }
                }
            }


            if(productVM.MainImage is not null)
            {
                string filename = await productVM.MainImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage existedImg = existed.ProductImages.FirstOrDefault(x=>x.IsPrimary==true);
                existedImg.Url.Delete(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(existedImg);

                existed.ProductImages.Add(new ProductImage { IsPrimary = true, Url = filename });
            }

            if (productVM.HoverImage is not null)
            {
                string filename = await productVM.HoverImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage existedImg = existed.ProductImages.FirstOrDefault(x => x.IsPrimary == false);
                existedImg.Url.Delete(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(existedImg);

                existed.ProductImages.Add(new ProductImage { IsPrimary = false, Url = filename });
            }

            if (productVM.ImageIds is null)
            {
                productVM.ImageIds = new List<int>();
            }
            List<ProductImage> removable =  existed.ProductImages.Where(x=>!productVM.ImageIds.Exists(y=>y==x.Id)&&x.IsPrimary==null).ToList();
            foreach (var item in removable)
            {
                item.Url.Delete(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(item);
            }

            TempData["ImageMessage"] = "";

            foreach (IFormFile image in productVM.AddImages ?? new List<IFormFile>())
            {
                if (!image.CheckFileType("image/"))
                {
                    TempData["ImageMessage"] += $" <p class=\"btn btn-inverse-danger btn-fw myParagraph\" style=\"display: inline-flex; align-items: center;\" >{image.FileName} file's type is not image.<span style=\"margin-top: -1px; margin-left: 3px\" class=\"close-button text-white\" onclick=\"closeParagraph()\"><i class=\"mdi mdi-close-circle-outline\"></i></span></p>\r\n\r\n                    <script> function closeParagraph() {{ var paragraphs = document.getElementsByClassName(\"myParagraph\");  for (var i = 0; i < paragraphs.length; i++) {{ paragraphs[i].style.display = \"none\"; }} }} setTimeout(closeParagraph, 10000); </script>";
                    continue;
                }
                if (!image.CheckFileSize(1))
                {
                    TempData["ImageMessage"] += $" <p class=\"btn btn-inverse-danger btn-fw myParagraph\" style=\"display: inline-flex; align-items: center;\" >{image.FileName} file's size is larger than 1MB.<span style=\"margin-top: -1px; margin-left: 3px\" class=\"close-button text-white\" onclick=\"closeParagraph()\"><i class=\"mdi mdi-close-circle-outline\"></i></span></p>\r\n\r\n                    <script> function closeParagraph() {{ var paragraphs = document.getElementsByClassName(\"myParagraph\");  for (var i = 0; i < paragraphs.length; i++) {{ paragraphs[i].style.display = \"none\"; }} }} setTimeout(closeParagraph, 10000); </script>";

                    continue;
                }

                existed.ProductImages.Add(new ProductImage { IsPrimary = null, Url = await image.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images") });
            }

            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = (decimal)productVM.Price;
            existed.SKU = productVM.SKU;
            existed.CategoryId = (int)productVM.CategoryId;
            existed.Category = productVM.Category;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _context.Products.Include(x=>x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
            if (product is null) return NotFound();

            foreach (var item in product.ProductImages ?? new List<ProductImage>())
            {
                item.Url.Delete(_env.WebRootPath, "assets", "images", "website-images");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _context.Products
                .Include(x=>x.Category)
                .Include(x=>x.ProductImages)
                .Include(x=>x.ProductTags)
                    .ThenInclude(x=>x.Tag)
                .Include(x=>x.ProductPlatforms)
                    .ThenInclude(x=>x.Platform)
                .Include(x=>x.ProductEditions)
                    .ThenInclude(x=>x.Edition)
                .FirstOrDefaultAsync(x => x.Id == id);
            return View(product);
        }



    }
}
