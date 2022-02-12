var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "Grandmas.Cookie";
        config.LoginPath = "/Home/Authenticate";
    });
builder.Services.AddControllersWithViews();

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
