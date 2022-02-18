using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Server;
using System.Linq;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("OAuth")
    .AddJwtBearer("OAuth", config =>
    {
        var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
        var key = new SymmetricSecurityKey(secretBytes);

        //!!!
        //Enable to pass token into url
        //!!!
        config.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Query.ContainsKey("access_token"))
                {
                    context.Token = context.Request.Query["access_token"];
                }

                return Task.CompletedTask;
            }
        };

        config.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = Constants.Issuer,
            ValidAudience = Constants.Audience,
            IssuerSigningKey = key
        };
    });

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
