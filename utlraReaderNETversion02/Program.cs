using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 🛠 Authentication ve Authorization servisleri
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Giriş yapmamış kullanıcı buraya yönlendirilir
        options.LoginPath = "/Account/Login"; // Ortak giriş sayfası istenirse değiştirilebilir
        options.AccessDeniedPath = "/Account/AccessDenied"; // Erişim yetkisi olmayanlar buraya yönlendirilir
        options.LogoutPath = "/Account/Logout";
        options.Cookie.Name = "UltraReaderAuth"; // İsteğe bağlı: çerez ismi
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Oturum süresi
    });

builder.Services.AddAuthorization(options =>
{
    // Gerekirse özel policy'ler eklenebilir
    // options.AddPolicy("ModeratorOnly", policy => policy.RequireRole("Moderator"));
});

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

// 🧠 Kimlik Doğrulama ve Yetkilendirme sırası önemli
app.UseAuthentication();
app.UseAuthorization();

// 🌐 Varsayılan route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WebtoonList}/{action=Index}/{id?}");

app.Run();
