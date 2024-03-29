﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.Utilities.Exceptions;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            throw new WrongRequestException("Test");
            List<Slide> slideList = _context.Slides.OrderBy(s=>s.Order).ToList();
            List<Product> productList = _context.Products.Include(x=>x.ProductImages).ToList();
            HomeVM vm = new HomeVM
            {
                Products = productList,
                Slides = slideList,
                LatestProducts = productList.OrderByDescending(p => p.Id).Take(8).ToList()
            };
            return View(vm);
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult ErrorPage(string error = "An error occured")
        {
            return View(model:error);

        }
    }
}
