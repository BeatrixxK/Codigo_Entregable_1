/// <summary>
/// Constituye el punto de entrada principal (Entry Point) de la aplicación web DragonNutrex.
/// Configura el contenedor de inyección de dependencias, inicializa la conexión a la base de datos Redis,
/// establece las reglas de autenticación y construye el pipeline de procesamiento de solicitudes HTTP.
/// </summary>

using Microsoft.AspNetCore.Components.Authorization; 
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

/// <summary>
/// Registra los servicios base de seguridad nativa de Blazor.
/// </summary>
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();

/// <summary>
/// Carga las variables de entorno desde el archivo .env físico del servidor.
/// </summary>
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

/// <summary>
/// Registra los repositorios de acceso a datos en el contenedor de inyección de dependencias.
/// </summary>
builder.Services.AddScoped<IRepository<Usuario>, UsuarioRedisRepository>();
builder.Services.AddScoped<IRepository<Producto>, ProductoRedisRepository>();
builder.Services.AddScoped<IRepository<Menu>, MenuRedisRepository>();

builder.Services.AddScoped<UsuarioRedisRepository>();
builder.Services.AddScoped<ProductoRedisRepository>();
builder.Services.AddScoped<MenuRedisRepository>();

/// <summary>
/// Registra los servicios de dominio que encapsulan la lógica de negocio principal.
/// </summary>
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<EstadisticasNutricionService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ExportacionService>();

/// <summary>
/// 🔥 CORRECCIÓN CRÍTICA: Se utiliza AddSingleton en lugar de AddScoped.
/// Esto garantiza que la sesión del usuario persista de forma global y no se destruya
/// debido al enrutamiento estático de Blazor .NET 8, eliminando el Efecto Amnesia.
/// </summary>
builder.Services.AddSingleton<UserSession>();
builder.Services.AddSingleton<AuthenticationStateProvider>(provider => provider.GetRequiredService<UserSession>());

builder.Services.AddScoped<UsuarioController>();
builder.Services.AddScoped<ProductoController>();
builder.Services.AddScoped<MenuController>();
builder.Services.AddScoped<EstadisticasNutricionController>();

/// <summary>
/// Implementa el uso de caché en la memoria local del servidor.
/// </summary>
builder.Services.AddMemoryCache(); 

var app = builder.Build();

/// <summary>
/// Construye la aplicación web y configura el pipeline de procesamiento de solicitudes.
/// </summary>
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<DragonNutrex.Web.Components.App>()
    .AddInteractiveServerRenderMode();

/// <summary>
/// Ejecuta un proceso temporal de sincronización de datos de Redis al arrancar la aplicación.
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<DragonNutrex.App.Interfaces.IRepository<DragonNutrex.App.Models.Usuario>>();
    var db = scope.ServiceProvider.GetRequiredService<DragonNutrex.App.Repositories.RedisConnection>().GetDatabase();
    
    var server = db.Multiplexer.GetServer(db.Multiplexer.GetEndPoints().First());
    var keys = server.Keys(pattern: "Usuario:*").ToList();

    foreach (var key in keys)
    {
        var idStr = key.ToString().Split(':').Last();
        db.SetAdd("usuarios:ids", idStr);
    }
}

app.Run();