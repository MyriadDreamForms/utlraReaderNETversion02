using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using utlraReaderNETversion02.Data;
using utlraReaderNETversion02.Models;

var builder = WebApplication.CreateBuilder(args);

// Bağlantı dizesini "PostgreSQL" anahtarıyla alıyoruz.
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
Console.WriteLine($"Alınan Connection String: {connectionString}");

// Veritabanı bağlantısını test et
try
{
    using var connection = new Npgsql.NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    Console.WriteLine("PostgreSQL bağlantısı başarılı!");
}
catch (Exception ex)
{
    Console.WriteLine($"Bağlantı hatası: {ex.Message}");
    throw;
}

// PostgreSQL için Npgsql kullanıyoruz
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity ayarları
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>();

// ⛔ Cookie ayarları: AccessDenied için yönlendirme ekleniyor!
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";               // Giriş yapılmadıysa buraya yönlendir
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Giriş var ama yetki yoksa buraya yönlendir
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // kimlik doğrulama
app.UseAuthorization();  // yetkilendirme

// Varsayılan route ayarı
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WebtoonList}/{action=Index}/{id?}");

// Identity Razor Pages (Identity sayfalarını etkinleştirir)
app.MapRazorPages();

app.Run();
