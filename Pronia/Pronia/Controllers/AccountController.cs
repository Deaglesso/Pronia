using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Entities;
using Pronia.Utilities.Enums;
using Pronia.Utilities.Extensions;
using Pronia.ViewModels;
using System.Text.RegularExpressions;

namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login, string? returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            string errormsg = "Username or password is not valid";
            if (login.UsernameorEmail.Contains('@'))
            {
                errormsg = "Mail or password is not valid";
            }
            AppUser user = await _userManager.FindByNameAsync(login.UsernameorEmail);

            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(login.UsernameorEmail);
                if (user is null)
                {
                    ModelState.AddModelError(string.Empty, errormsg);
                    return View();
                }

            }
            var res = await _signInManager.PasswordSignInAsync(user, login.Password, login.isRemembered, true);

            if (!res.Succeeded)
            {
                if (res.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, $"You blocked until {user.LockoutEnd}");
                }

                ModelState.AddModelError(string.Empty, errormsg);
                return View();
            }
            if (Request.Cookies["Basket"] != null)
            {
                Response.Cookies.Delete("Basket");
                
            }
            if (returnUrl is not null)
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

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
                Name = userVM.Name.FormatCapitalizeTrim(),
                Email = userVM.Email,
                Surname = userVM.Surname.FormatCapitalizeTrim(),
                UserName = userVM.Username,
                Gender = userVM.Gender,

            };
            
            var result = await _userManager.CreateAsync(user, userVM.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
            return View();

        }

        public async Task<IActionResult> CreateRole()
        {
            foreach (var role in System.Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString()
                    });
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies["Basket"] != null)
            {
                Response.Cookies.Delete("Basket");

            }
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
