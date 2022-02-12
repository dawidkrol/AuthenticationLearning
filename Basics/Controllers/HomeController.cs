using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }
        public IActionResult Authenticate()
        {
            List<Claim> grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email,"Bob@fmail.com"),
                new Claim("Grandma.Says","Very nice boi.")
            };

            var licenceClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob K Foo"),
                new Claim("DrivingLicence","A+")
            };

            var Grandmasidentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            var licenceIdentity = new ClaimsIdentity(licenceClaims, "Goverment Identity");

            var userPrincipal = new ClaimsPrincipal(new[] { Grandmasidentity, licenceIdentity });

            HttpContext.SignInAsync(userPrincipal).Wait();

            return RedirectToAction("Index");
        }
    }
}
