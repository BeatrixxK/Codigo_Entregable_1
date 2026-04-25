using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonNutrex.App.Models;
using DragonNutrex.App.Services;
using Microsoft.Extensions.Caching.Memory;

namespace DragonNutrex.App.Controllers;

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
    public void CrearMenu(Menu menu)
    {
        _service.CrearMenu(menu);
        LimpiarCache();
    }

    public void ActualizarMenu(Menu menu)
    {
        _service.ActualizarMenu(menu);
        LimpiarCache();
    }

    public void EliminarMenu(Guid id)
    {
        _service.EliminarMenu(id);
        LimpiarCache();
    }

    private void LimpiarCache()
    {
        _cache.Remove(MENUS_CACHE_KEY);
    }
}