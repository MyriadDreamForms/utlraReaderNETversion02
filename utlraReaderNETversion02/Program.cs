using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using utlraReaderNETversion02.Data;
using utlraReaderNETversion02.Models;

var builder = WebApplication.CreateBuilder(args);

// Baðlantý dizesini "PostgreSQL" anahtarýyla alýyoruz.
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
Console.WriteLine($"Alýnan Connection String: {connectionString}");

// Veritabaný baðlantýsýný test et
try
{
    using var connection = new Npgsql.NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    Console.WriteLine("PostgreSQL baðlantýsý baþarýlý!");
}
catch (Exception ex)
{
    Console.WriteLine($"Baðlantý hatasý: {ex.Message}");
    throw;
}


// PostgreSQL için Npgsql kullanýyoruz
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity ayarlarý
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>();

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

// Varsayýlan route ayarý
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WebtoonList}/{action=Index}/{id?}");

// Identity Razor Pages (Identity sayfalarýný etkinleþtirir)
app.MapRazorPages();

app.Run();
