using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Pronia.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users.Include(x => x.BasketItems)
                    .ThenInclude(y => y.Product)
                    .ThenInclude(z => z.ProductImages.Where(img => img.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));


                foreach (var item in user.BasketItems)
                {
                    items.Add(new BasketItemVM
                    {
                        Id = item.ProductId,
                        Price = item.Product.Price,
                        Count = item.Count,
                        Name = item.Product.Name,
                        Subtotal = item.Count * item.Product.Price,
                        Image = item.Product.ProductImages[0].Url,


                    }); 

                }
            }
            else
            {
                string oldBasket = Request.Cookies["Basket"];


                if (oldBasket is not null)
                {
                    List<BasketCookieVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieVM>>(oldBasket);
                    foreach (var item in cookies)
                    {
                        Product product = await _context.Products.Include(x => x.ProductImages.Where(y => y.IsPrimary == true)).FirstOrDefaultAsync(x => x.Id == item.Id);
                        if (product != null)
                        {
                            BasketItemVM itemVM = new BasketItemVM
                            {
                                Id = item.Id,
                                Name = product.Name,
                                Image = product.ProductImages.FirstOrDefault().Url,
                                Price = product.Price,
                                Count = item.Count,
                                Subtotal = product.Price * item.Count
                            };
                            items.Add(itemVM);
                        }
                    }
                }
            }


            

            return View(items);
        }
            
        public async Task<IActionResult> AddBasket(int id,int count = 1)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            if (!User.Identity.IsAuthenticated)
            {
                List<BasketCookieVM> basketVM = new List<BasketCookieVM>();

                string oldBasket = Request.Cookies["Basket"];

                if (oldBasket == null)
                {
                    BasketCookieVM vm = new BasketCookieVM
                    {
                        Id = id,
                        Count = count
                    };

                    basketVM.Add(vm);
                }
                else
                {
                    basketVM = JsonConvert.DeserializeObject<List<BasketCookieVM>>(oldBasket);

                    BasketCookieVM old = basketVM.FirstOrDefault(x => x.Id == id);

                    if (old == null)
                    {
                        BasketCookieVM vm = new BasketCookieVM
                        {
                            Id = id,
                            Count = count
                        };

                        basketVM.Add(vm);
                    }
                    else
                    {
                        old.Count++;
                    }


                }
                



                string json = JsonConvert.SerializeObject(basketVM);

                Response.Cookies.Append("Basket", json);
            }
            else
            {
                AppUser user = await _userManager.Users.Include(x => x.BasketItems)
                    .FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (user == null) return NotFound();


                BasketItem item = user.BasketItems.FirstOrDefault(x => x.ProductId == product.Id);

                if (item is null)
                {
                    item = new BasketItem
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Count = 1,
                        Price = product.Price,

                    };
                    user.BasketItems.Add(item);

                }
                else
                {
                    item.Count++;
                }



                await _context.SaveChangesAsync();
            }


            

            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> ManageItemBasket(int id,string mode,string lastctrl)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users.Include(x => x.BasketItems)
                    .ThenInclude(y => y.Product)
                    .ThenInclude(z => z.ProductImages.Where(img => img.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                BasketItem old = user.BasketItems.FirstOrDefault(x => x.ProductId == product.Id);
                if (mode == "full")
                {
                    user.BasketItems.Remove(old);
                }
                else if (mode == "minus")
                {
                    if (old.Count > 0)
                    {
                        old.Count--;
                    }
                    if (old.Count == 0)
                    {
                        user.BasketItems.Remove(old);
                    }
                }
                else if (mode == "plus")
                {
                    old.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                string oldBasket = Request.Cookies["Basket"];

                List<BasketCookieVM> basketVM = new List<BasketCookieVM>();

                basketVM = JsonConvert.DeserializeObject<List<BasketCookieVM>>(oldBasket);

                BasketCookieVM old = basketVM.FirstOrDefault(x => x.Id == id);

                if (old != null)
                {
                    if (mode == "full")
                    {
                        basketVM.Remove(old);
                    }
                    else if (mode == "minus")
                    {
                        if (old.Count > 0)
                        {
                            old.Count--;
                        }
                        if (old.Count == 0)
                        {
                            basketVM.Remove(old);
                        }
                    }
                    else if (mode == "plus")
                    {
                        old.Count++;
                    }


                }

                string json = JsonConvert.SerializeObject(basketVM);

                Response.Cookies.Append("Basket", json);
            }

            


            return RedirectToAction("Index", lastctrl);
        }

        public ActionResult GetBasket() 
        {
            return Content(Request.Cookies["Basket"]);
        }
    }
}
