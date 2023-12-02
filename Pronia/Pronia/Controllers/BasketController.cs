using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();

            string oldBasket = Request.Cookies["Basket"];


            if (oldBasket is not null)
            {
                List <BasketCookieVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieVM>>(oldBasket);
                foreach (var item in cookies)
                {
                    Product product = await _context.Products.Include(x=>x.ProductImages.Where(y=>y.IsPrimary == true)).FirstOrDefaultAsync(x=>x.Id==item.Id);
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

            return View(items);
        }
            
        public async Task<IActionResult> AddBasket(int id,int count = 1)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

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

                BasketCookieVM old = basketVM.FirstOrDefault(x=>x.Id == id);

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

            Response.Cookies.Append("Basket",json);

            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> ManageItemBasket(int id,string mode)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            string oldBasket = Request.Cookies["Basket"];

            List<BasketCookieVM> basketVM = new List<BasketCookieVM>();
            
            basketVM = JsonConvert.DeserializeObject<List<BasketCookieVM>>(oldBasket);

            BasketCookieVM old = basketVM.FirstOrDefault(x => x.Id == id);

            if (old != null)
            {
                if(mode == "full")
                {
                    basketVM.Remove(old);
                }
                else if(mode == "minus")
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
                else if(mode == "plus")
                {
                    old.Count++;
                }
                

            }

            string json = JsonConvert.SerializeObject(basketVM);

            Response.Cookies.Append("Basket", json);

            return RedirectToAction("Index", "Basket");
        }

        public ActionResult GetBasket() 
        {
            return Content(Request.Cookies["Basket"]);
        }
    }
}
