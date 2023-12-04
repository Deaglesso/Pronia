using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Entities;
using Pronia.ViewModels;
using System.Text.RegularExpressions;

namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            //REGEX 
            string regexPattern = @"^[\w]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
            Regex regex = new Regex(regexPattern);
            string inputString = userVM.Email;
            Match match = regex.Match(inputString);

            if (!match.Success)
            {
                ModelState.AddModelError("Email", "Mail is not valid");
                return View();
            }
            


            AppUser user = new AppUser 
            {
                Name = userVM.Name,
                Email = userVM.Email,
                Surname = userVM.Surname,
                UserName = userVM.Name,
                Gender = userVM.Gender,

            };
            var result = await _userManager.CreateAsync(user,userVM.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
            return View();

        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
