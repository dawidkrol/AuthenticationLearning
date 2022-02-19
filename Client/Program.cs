using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(config =>
{
    // We check the cooke if we are authenticated
    config.DefaultAuthenticateScheme = "ClientCookie";
    // When we sign in we will deal out with cookie
    config.DefaultSignInScheme = "ClientCookie";
    // Use this to check if we allowed to do something
    config.DefaultChallengeScheme = "OurServer";
})
    .AddCookie("ClientCookie")
    .AddOAuth("OurServer", config =>
    {
        config.CallbackPath = "/oauth/callback";
        config.ClientId = "client_id";
        config.ClientSecret = "client_secret";
        config.AuthorizationEndpoint = "https://localhost:7007/oauth/authorize";
        config.TokenEndpoint = "https://localhost:7007/oauth/token";
        config.SaveTokens = true;

        config.Events = new OAuthEvents()
        {
            OnCreatingTicket = context =>
            {
                byte[] DecodeUrlBase64(string s)
                {
                    s = s.Replace('-', '+').Replace('_', '/').PadRight(4 * ((s.Length + 3) / 4), '=');
                    return Convert.FromBase64String(s);
                }

                string accessToken = context.AccessToken;
                var base64payloadarray = accessToken.Split('.')[1];
                var bytes = DecodeUrlBase64(base64payloadarray);
                var jsonPayload = Encoding.UTF8.GetString(bytes);
                var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                foreach (var claim in claims)
                {
                    context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
