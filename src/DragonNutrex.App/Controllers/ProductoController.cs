// =====================================================
// IMPORTACIONES
// =====================================================
using DragonNutrex.App.Models;
using DragonNutrex.App.Services;
using Microsoft.Extensions.Caching.Memory; // Necesario para el manejo de RAM

namespace DragonNutrex.App.Controllers;

// =====================================================
// CLASE PRODUCTO CONTROLLER (OPTIMIZADA CON CACHÉ)
// =====================================================
public class ProductoController
{
    private readonly ProductoService _service;
    private readonly IMemoryCache _cache;
    
    // Identificador único para los productos en la memoria RAM
    private const string PRODUCTOS_CACHE_KEY = "lista_productos_maestra";

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Inyectamos tanto el servicio como el motor de caché
    public ProductoController(ProductoService service, IMemoryCache cache)
    {
        _service = service;
        _cache = cache;
    }

    // =====================================================
    // MÉTODO OBTENER PRODUCTOS (EL MOTOR DE VELOCIDAD)
    // =====================================================
    public List<Producto> ObtenerProductos()
    {
        // 1. Intentar obtener los datos de la memoria RAM local
        if (_cache.TryGetValue(PRODUCTOS_CACHE_KEY, out List<Producto>? productosCache))
        {
            // Si existen, los devolvemos sin viajar a Redis Cloud
            return productosCache ?? new List<Producto>();
        }

        // 2. Si no están en RAM, vamos a la biblioteca (Redis Cloud / Servicio)
        var productosDesdeDb = _service.ObtenerProductos();

        // 3. Guardar el resultado en RAM para la próxima vez
        // Configuramos para que la "foto" dure 60 minutos
        var opcionesCache = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(60))
            .SetSize(1); // Opcional: define peso en memoria

        _cache.Set(PRODUCTOS_CACHE_KEY, productosDesdeDb, opcionesCache);

        return productosDesdeDb;
    }

    // =====================================================
    // MÉTODOS DE ESCRITURA (CON INVALIDACIÓN DE CACHÉ)
    // =====================================================
    // Cada vez que los datos cambian, debemos "romper" la foto vieja 
    // para que el sistema se vea obligado a recargar desde Redis.

    public void CrearProducto(Producto producto)
    {
        _service.CrearProducto(producto);
        LimpiarCache();
    }

    public void ActualizarProducto(Producto producto)
    {
        _service.ActualizarProducto(producto);
        LimpiarCache();
    }

    public void EliminarProducto(Guid id)
    {
        _service.EliminarProducto(id);
        LimpiarCache();
    }

    public Producto? ObtenerProducto(Guid id)
    {
        // Para consultas individuales, solemos ir directo al servicio 
        // o filtrar la lista que ya tenemos en caché.
        return _service.ObtenerProducto(id);
    }

    // =====================================================
    // AYUDANTE: LIMPIAR CACHÉ
    // =====================================================
    private void LimpiarCache()
    {
        _cache.Remove(PRODUCTOS_CACHE_KEY);
    }
}