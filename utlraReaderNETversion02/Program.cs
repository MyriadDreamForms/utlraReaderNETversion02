using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 🛠 Authentication ve Authorization servisleri
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
    });

builder.Services.AddAuthorization(); // (isteğe bağlı, ama iyi olur)

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 📦 Middleware sıralaması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🧠 Kimlik Doğrulama ve Yetkilendirme sırası ÖNEMLİ
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WebtoonList}/{action=Index}/{id?}");

app.Run();
