using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IdentityExample.Data;
using Microsoft.AspNetCore.Identity;
using NETCore.MailKit.Core;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public HomeController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
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
            //if (await _roleManager.FindByNameAsync("Admin") == null)
            //{
            //    //Add role
            //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
            //}
            if (result.Succeeded)
            {
                ////Add to role
                ////await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(username), "Admin");
                ////sign user here
                //await _signInManager.PasswordSignInAsync(user, password, false, false);

                //Generation of the email token
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, confirmationToken },Request.Scheme,Request.Host.ToString());
                await _emailService.SendAsync(user.UserName + "@fmail.com", "Confirm your account", $"<a href='{link}'>Click here to verify email</a>",true);
                
                return RedirectToAction("EmailVerification");
            }
            return RedirectToAction("Index");
        }
        //[Route("VerifyEmail/{userId}/{token}")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return BadRequest();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }
            return BadRequest();
        }
        public IActionResult EmailVerification() => View();
    }
}
