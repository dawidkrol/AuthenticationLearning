using Basics.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "Grandmas.Cookie";
        config.LoginPath = "/Home/Authenticate";
    });

builder.Services.AddAuthorization(config =>
{
    //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    //var defaultAuthPolicy = defaultAuthBuilder
    //    .RequireAuthenticatedUser()
    //    .RequireClaim(ClaimTypes.DateOfBirth)
    //    .Build();

    //config.DefaultPolicy = defaultAuthPolicy;

    //config.AddPolicy("Claim.DoB", policyBuilder =>
    //{
    //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
    //});

    //config.AddPolicy("Claim.DoB", policyBuilder =>
    //{
    //    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
    //});

    config.AddPolicy("Claim.DoB", policyBuilder =>
    {
        policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
    });
});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAuthorizationHandler,CustomRequireClaimType>();

var app = builder.Build();

app.UseRouting();

//Who are you?
app.UseAuthentication();

//Are you allowed?
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
