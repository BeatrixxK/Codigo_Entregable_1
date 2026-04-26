/// <summary>
/// Constituye el punto de entrada principal (Entry Point) de la aplicación web DragonNutrex.
/// Configura el contenedor de inyección de dependencias, inicializa la conexión a la base de datos Redis,
/// establece las reglas de autenticación y construye el pipeline de procesamiento de solicitudes HTTP.
/// </summary>

using Microsoft.AspNetCore.Components.Authorization; // Agregado para la seguridad
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
/// Habilita la propagación del estado de autenticación en cascada y configura 
/// el núcleo de autorización global para proteger rutas y componentes.
/// </summary>
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();

/// <summary>
/// Carga las variables de entorno desde el archivo .env físico del servidor.
/// Obtiene la cadena de conexión y registra la instancia global de conexión a Redis 
/// como un servicio de ciclo de vida único (Singleton) para toda la aplicación.
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
/// Define el ciclo de vida por solicitud (Scoped) para garantizar transacciones aisladas 
/// en la gestión en caché de Usuarios, Productos y Menús.
/// </summary>
builder.Services.AddScoped<IRepository<Usuario>, UsuarioRedisRepository>();
builder.Services.AddScoped<IRepository<Producto>, ProductoRedisRepository>();
builder.Services.AddScoped<IRepository<Menu>, MenuRedisRepository>();

// Registro directo por si los servicios no usan la interfaz
builder.Services.AddScoped<UsuarioRedisRepository>();
builder.Services.AddScoped<ProductoRedisRepository>();
builder.Services.AddScoped<MenuRedisRepository>();

/// <summary>
/// Registra los servicios de dominio que encapsulan la lógica de negocio principal.
/// Permite a los controladores y componentes visuales consumir operaciones de exportación, 
/// generación de reportes y cálculos estadísticos de manera inyectable.
/// </summary>
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<EstadisticasNutricionService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ExportacionService>();

/// <summary>
/// Registra los controladores de la aplicación y vincula la seguridad de la sesión.
/// Sobrescribe el proveedor de estado de autenticación predeterminado de Blazor con la implementación 
/// personalizada (UserSession) para integrar la validación de la interfaz con los registros de Redis.
/// </summary>
builder.Services.AddScoped<UserSession>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<UserSession>());

builder.Services.AddScoped<UsuarioController>();
builder.Services.AddScoped<ProductoController>();
builder.Services.AddScoped<MenuController>();
builder.Services.AddScoped<EstadisticasNutricionController>();

/// <summary>
/// Implementa el uso de caché en la memoria local del servidor para optimizar 
/// la carga de datos de acceso frecuente sin impactar a la base de datos principal.
/// </summary>
builder.Services.AddMemoryCache(); 

var app = builder.Build();

/// <summary>
/// Construye la aplicación web y configura el pipeline de procesamiento de solicitudes.
/// Activa el enrutamiento de archivos estáticos, la protección contra falsificación de solicitudes (Antiforgery)
/// y el mapeo en tiempo real de los componentes interactivos del servidor.
/// </summary>
app.UseStaticFiles();
app.UseAntiforgery();

// Mapeo correcto al archivo App principal
app.MapRazorComponents<DragonNutrex.Web.Components.App>()
    .AddInteractiveServerRenderMode();

/// <summary>
/// Ejecuta un proceso temporal de sincronización de datos de Redis al arrancar la aplicación.
/// Recorre las claves de usuario existentes y las vincula al conjunto maestro "usuarios:ids" 
/// para garantizar la consistencia en listados globales y consultas de administradores.
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<DragonNutrex.App.Interfaces.IRepository<DragonNutrex.App.Models.Usuario>>();
    var db = scope.ServiceProvider.GetRequiredService<DragonNutrex.App.Repositories.RedisConnection>().GetDatabase();
    
    // Obtenemos todas las llaves que empiezan con "Usuario:"
    var server = db.Multiplexer.GetServer(db.Multiplexer.GetEndPoints().First());
    var keys = server.Keys(pattern: "Usuario:*").ToList();

    foreach (var key in keys)
    {
        // Extraemos el ID de la llave (ej: de "Usuario:10" sacamos "10")
        var idStr = key.ToString().Split(':').Last();
        // Lo agregamos al set maestro que usa el GetAllAsync
        db.SetAdd("usuarios:ids", idStr);
    }
}
// ------------------------------------------------

app.Run();