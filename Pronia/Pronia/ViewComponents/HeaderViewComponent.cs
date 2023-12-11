using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.ViewModels;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace Pronia.ViewComponents
{

    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;

        public HeaderViewComponent(AppDbContext context, IHttpContextAccessor http,UserManager<AppUser> userManager)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(x => x.Key, x => x.Value);

            List<BasketItemVM> items = new List<BasketItemVM>();

            if (_http.HttpContext.User.Identity.IsAuthenticated)
            {

                AppUser user = await _userManager.Users.Include(x => x.BasketItems.Where(i => i.OrderId == null))
                    .ThenInclude(y => y.Product)
                    .ThenInclude(z => z.ProductImages.Where(img => img.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));


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
                string oldBasket = _http.HttpContext.Request.Cookies["Basket"];


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
            

            var send = Tuple.Create( settings, items);
            return View(send);
        }
    }
}
