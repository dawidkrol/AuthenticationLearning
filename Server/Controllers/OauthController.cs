using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Controllers
{
    public class OauthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string response_type,
            string client_id,
            string redirect_uri,
            string scope,
            string state)
        {

            var q = new QueryBuilder();
            q.Add("redirectUri", redirect_uri);
            q.Add("state", state);

            return View(model: q.ToString());
        }
        [HttpPost]
        public IActionResult Authorize(
            string username,
            string password,
            string redirectUri,
            string state)
        {
            var code = "dsadsa";
            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);

            return Redirect($"{redirectUri}{query.ToString()}");
        }

        public IActionResult Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_id)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,"some_id"),
                new Claim(JwtRegisteredClaimNames.Name,"Dawid"),
                new Claim("granny","cookie")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials
                );

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial"
            };

            //var responseJson = JsonConvert.SerializeObject(responseObject);
            //var responseBytes = Encoding.UTF8.GetBytes(responseJson);
            //Response.Body.Write(responseBytes,0,responseBytes.Length);

            return new JsonResult(responseObject);
        }
    }
}
