﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdentityExample.Controllers
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
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,"some_id"),
                new Claim("granny","cookie")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var alhorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key,alhorithm);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience,
                claims,
                notBefore:DateTime.Now,
                expires:DateTime.Now.AddMinutes(60),
                signingCredentials
                );

            var tokenJSON = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { access_token = tokenJSON });
        }

        public IActionResult Decode(string part)
        {
            var bytes = Convert.FromBase64String(part);

            return Ok(Encoding.UTF8.GetString(bytes));
        }
    }
}
