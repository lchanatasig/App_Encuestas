using App_Encuestas.Models;
using App_Encuestas.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Configuraci�n de la base de datos
builder.Services.AddDbContext<AppEncuestasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();



// Agregar servicios para la cach� distribuida en memoria y la sesi�n
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de expirar la sesi�n
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Obligatorio seg�n GDPR
});
// Habilitar Razor Pages y Controllers con recompilaci�n en tiempo de ejecuci�n
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddScoped<EncuestaService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
