using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Authentication and Authorization setup
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login"; // Admin giriþine yönlendir
        options.AccessDeniedPath = "/Admin/Login"; // Eriþim reddedildiðinde yönlendirme
        options.LogoutPath = "/Admin/Logout"; // Admin çýkýþý
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin")); // Admin rolüne özel eriþim
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Authentication middleware
app.UseAuthorization(); // Authorization middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WebtoonList}/{action=Index}/{id?}");

app.Run();
