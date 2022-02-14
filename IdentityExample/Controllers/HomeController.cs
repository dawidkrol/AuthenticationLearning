using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IdentityExample.Data;
using Microsoft.AspNetCore.Identity;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }

        [Authorize(Policy = "Adm")]
        public IActionResult SecurePolicy()
        {
            return View("Secure");
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if(user != null){
                //sign in
                await _signInManager.PasswordSignInAsync(user, password, false, false);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser
            {
                UserName = username
            };
            var result = await _userManager.CreateAsync(user, password);
            //---------------------------------------------------------------
            if (await _roleManager.FindByNameAsync("Admin") == null)
            {
                //Add role
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (result.Succeeded)
            {
                //Add to role
                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(username), "Admin");
                //sign user here
                await _signInManager.PasswordSignInAsync(user, password, false, false);
            }
            return RedirectToAction("Index");
        }
    }
}
