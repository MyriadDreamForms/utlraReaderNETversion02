using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using utlraReaderNETversion02.Data;
using utlraReaderNETversion02.Models;

var builder = WebApplication.CreateBuilder(args);

// Ba�lant� dizesini "PostgreSQL" anahtar�yla al�yoruz.
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
Console.WriteLine($"Al�nan Connection String: {connectionString}");

// Veritaban� ba�lant�s�n� test et
try
{
    using var connection = new Npgsql.NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    Console.WriteLine("PostgreSQL ba�lant�s� ba�ar�l�!");
}
catch (Exception ex)
{
    Console.WriteLine($"Ba�lant� hatas�: {ex.Message}");
    throw;
}


// PostgreSQL i�in Npgsql kullan�yoruz
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity ayarlar�
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

// Varsay�lan route ayar�
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WebtoonList}/{action=Index}/{id?}");

// Identity Razor Pages (Identity sayfalar�n� etkinle�tirir)
app.MapRazorPages();

app.Run();
