using DragonNutrex.Web;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// =====================================================
// 1. CARGAR VARIABLES DE ENTORNO Y REDIS
// =====================================================
if (File.Exists(".env"))
{
    Env.Load(".env");
}
else
{
    Env.TraversePath().Load();
}

var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL");
builder.Services.AddSingleton(new RedisConnection(redisUrl ?? ""));

// =====================================================
// 2. REGISTRAR REPOSITORIOS (Base de datos)
// =====================================================
builder.Services.AddScoped<IRepository<Usuario>, UsuarioRedisRepository>();
builder.Services.AddScoped<IRepository<Producto>, ProductoRedisRepository>();
builder.Services.AddScoped<IRepository<Menu>, MenuRedisRepository>();

// Registro directo por si los servicios no usan la interfaz
builder.Services.AddScoped<UsuarioRedisRepository>();
builder.Services.AddScoped<ProductoRedisRepository>();
builder.Services.AddScoped<MenuRedisRepository>();

// =====================================================
// 3. REGISTRAR SERVICIOS (Lógica de negocio)
// =====================================================
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<EstadisticasNutricionService>();

// =====================================================
// 4. REGISTRAR CONTROLADORES Y SESIÓN
// =====================================================
builder.Services.AddSingleton<UserSession>();
builder.Services.AddScoped<UsuarioController>();
builder.Services.AddScoped<ProductoController>();
builder.Services.AddScoped<MenuController>();
builder.Services.AddScoped<EstadisticasNutricionController>();

//Usar caché en la memoria local (SessionStorage / Singleton)

builder.Services.AddMemoryCache(); // Agregamos el servicio de caché en memoria para optimizar la lectura de productos

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

// Mapeo correcto al archivo App principal
app.MapRazorComponents<DragonNutrex.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();