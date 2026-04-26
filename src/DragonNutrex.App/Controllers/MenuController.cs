using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonNutrex.App.Models;
using DragonNutrex.App.Services;
using Microsoft.Extensions.Caching.Memory;

namespace DragonNutrex.App.Controllers;

/// <summary>
/// Controlador encargado de gestionar las operaciones relacionadas con los menús, incluyendo obtención, creación, actualización y eliminación, con soporte para caché en memoria.
/// </summary>
public class MenuController
{
    private readonly MenuService _service;
    private readonly IMemoryCache _cache;
    private const string MENUS_CACHE_KEY = "lista_menus_maestra";

    public MenuController(MenuService service, IMemoryCache cache)
    {
        _service = service;
        _cache = cache;
    }

    // =====================================================
    // MÉTODO ASÍNCRONO (El que necesita MiProgreso.razor)
    // =====================================================
    /// <summary>
    /// Obtiene la lista de menús de forma asincrónica, utilizando caché en memoria para mejorar el rendimiento. Si no está en caché, recupera desde la base de datos y almacena en caché por 30 minutos.
    /// </summary>
    /// <returns>Una tarea que representa la operación asincrónica, con una lista de objetos Menu.</returns>
    public async Task<List<Menu>> ObtenerMenusAsync()
    {
        // 1. Intentamos leer de la RAM para que sea instantáneo
        if (_cache.TryGetValue(MENUS_CACHE_KEY, out List<Menu>? menusCache))
        {
            return menusCache ?? new List<Menu>();
        }

        // 2. Si no está en RAM, vamos a Redis de forma asíncrona
        var menusDesdeDb = await _service.ObtenerMenusAsync();

        // 3. Guardamos en RAM por 30 minutos
        var opcionesCache = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

        _cache.Set(MENUS_CACHE_KEY, menusDesdeDb, opcionesCache);

        return menusDesdeDb ?? new List<Menu>();
    }

    // =====================================================
    // MÉTODO SÍNCRONO (Por compatibilidad)
    // =====================================================
    /// <summary>
    /// Obtiene la lista de menús de forma síncrona, utilizando caché en memoria para mejorar el rendimiento. Si no está en caché, recupera desde la base de datos y almacena en caché por 30 minutos.
    /// </summary>
    /// <returns>Una lista de objetos Menu.</returns>
    public List<Menu> ObtenerMenus()
    {
        if (_cache.TryGetValue(MENUS_CACHE_KEY, out List<Menu>? menusCache))
        {
            return menusCache ?? new List<Menu>();
        }

        var menusDesdeDb = _service.ObtenerMenus();

        var opcionesCache = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

        _cache.Set(MENUS_CACHE_KEY, menusDesdeDb, opcionesCache);

        return menusDesdeDb ?? new List<Menu>();
    }

    // =====================================================
    // ESCRITURA (Limpian el caché para evitar datos viejos)
    // =====================================================
    /// <summary>
    /// Crea un nuevo menú en la base de datos y limpia el caché para asegurar que los datos estén actualizados.
    /// </summary>
    /// <param name="menu">El objeto Menu a crear.</param>
    public void CrearMenu(Menu menu)
    {
        _service.CrearMenu(menu);
        LimpiarCache();
    }

    /// <summary>
    /// Actualiza un menú existente en la base de datos y limpia el caché para asegurar que los datos estén actualizados.
    /// </summary>
    /// <param name="menu">El objeto Menu a actualizar.</param>
    public void ActualizarMenu(Menu menu)
    {
        _service.ActualizarMenu(menu);
        LimpiarCache();
    }

    /// <summary>
    /// Elimina un menú de la base de datos por su identificador único y limpia el caché para asegurar que los datos estén actualizados.
    /// </summary>
    /// <param name="id">El identificador único del menú a eliminar.</param>
    public void EliminarMenu(Guid id)
    {
        _service.EliminarMenu(id);
        LimpiarCache();
    }

    /// <summary>
    /// Limpia la entrada de caché correspondiente a la lista de menús para forzar la recarga de datos frescos.
    /// </summary>
    private void LimpiarCache()
    {
        _cache.Remove(MENUS_CACHE_KEY);
    }
}