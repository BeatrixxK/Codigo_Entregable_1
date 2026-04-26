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
/// <summary>
/// Controlador encargado de gestionar las operaciones CRUD de productos, implementando un sistema de caché en memoria para optimizar el rendimiento de las consultas.
/// </summary>
public class ProductoController
{
    /// <summary>
    /// Servicio utilizado para acceder a los datos de productos.
    /// </summary>
    private readonly ProductoService _service;
    /// <summary>
    /// Instancia de caché en memoria para almacenar temporalmente los datos de productos.
    /// </summary>
    private readonly IMemoryCache _cache;
    
    /// <summary>
    /// Clave utilizada para identificar la lista de productos en la caché.
    /// </summary>
    private const string PRODUCTOS_CACHE_KEY = "lista_productos_maestra";

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    /// <summary>
    /// Inicializa una nueva instancia del controlador, inyectando las dependencias necesarias.
    /// </summary>
    /// <param name="service">Servicio de productos.</param>
    /// <param name="cache">Caché en memoria.</param>
    // Inyectamos tanto el servicio como el motor de caché
    public ProductoController(ProductoService service, IMemoryCache cache)
    {
        _service = service;
        _cache = cache;
    }

    // =====================================================
    // MÉTODO OBTENER PRODUCTOS (EL MOTOR DE VELOCIDAD)
    // =====================================================
    /// <summary>
    /// Obtiene la lista completa de productos. Utiliza la caché en memoria para devolver los datos rápidamente si están disponibles; de lo contrario, recupera los datos desde el servicio y los almacena en caché con una expiración de 60 minutos.
    /// </summary>
    /// <returns>Lista de objetos Producto.</returns>
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

    /// <summary>
    /// Crea un nuevo producto en el sistema mediante el servicio y posteriormente limpia la caché para garantizar que las consultas futuras reflejen los cambios realizados.
    /// </summary>
    /// <param name="producto">Objeto Producto a crear.</param>
    public void CrearProducto(Producto producto)
    {
        _service.CrearProducto(producto);
        LimpiarCache();
    }

    /// <summary>
    /// Actualiza la información de un producto existente utilizando el servicio y limpia la caché para asegurar la consistencia de los datos.
    /// </summary>
    /// <param name="producto">Objeto Producto con la información actualizada.</param>
    public void ActualizarProducto(Producto producto)
    {
        _service.ActualizarProducto(producto);
        LimpiarCache();
    }

    /// <summary>
    /// Elimina un producto del sistema basado en su identificador único, utilizando el servicio, y limpia la caché para mantener la integridad de los datos.
    /// </summary>
    /// <param name="id">Identificador único del producto a eliminar.</param>
    public void EliminarProducto(Guid id)
    {
        _service.EliminarProducto(id);
        LimpiarCache();
    }

    /// <summary>
    /// Obtiene un producto específico basado en su identificador único, consultando directamente el servicio.
    /// </summary>
    /// <param name="id">Identificador único del producto.</param>
    /// <returns>Objeto Producto si se encuentra, o null en caso contrario.</returns>
    public Producto? ObtenerProducto(Guid id)
    {
        // Para consultas individuales, solemos ir directo al servicio 
        // o filtrar la lista que ya tenemos en caché.
        return _service.ObtenerProducto(id);
    }

    // =====================================================
    // AYUDANTE: LIMPIAR CACHÉ
    // =====================================================
    /// <summary>
    /// Elimina la entrada de caché correspondiente a la lista de productos, forzando la recarga de datos desde el servicio en la próxima consulta.
    /// </summary>
    private void LimpiarCache()
    {
        _cache.Remove(PRODUCTOS_CACHE_KEY);
    }
}