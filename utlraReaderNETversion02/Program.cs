using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using utlraReaderNETversion02.Data;
using utlraReaderNETversion02.Models;

var builder = WebApplication.CreateBuilder(args);

// Environment Variables'dan oku
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "postgres";
var username = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
var password = Environment.GetEnvironmentVariable("DB_PASS") ?? "Bolpirasa.123";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";

// Connection string'i kendimiz kuruyoruz
var connectionString = $"Host={host};Port={port};Database={dbName};Username={username};Password={password}";

Console.WriteLine($"Oluşturulan Connection String: {connectionString}");

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
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WebtoonList}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
