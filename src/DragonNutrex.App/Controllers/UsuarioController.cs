using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonNutrex.App.Models;
using DragonNutrex.App.Services; 
using Microsoft.Extensions.Caching.Memory;

namespace DragonNutrex.App.Controllers;

public class UsuarioController
{
    private readonly UsuarioService _service;
    private readonly IMemoryCache _cache;
    private const string USUARIOS_CACHE_KEY = "lista_usuarios_maestra";

    public UsuarioController(UsuarioService service, IMemoryCache cache)
    {
        _service = service;
        _cache = cache;
    }

    // =====================================================
    // MÉTODO ASÍNCRONO REAL (Para el Login - Sin cuelgues)
    // =====================================================
    public async Task<List<Usuario>> ObtenerUsuariosAsync()
    {
        if (_cache.TryGetValue(USUARIOS_CACHE_KEY, out List<Usuario>? usuariosCache))
        {
            return usuariosCache ?? new List<Usuario>();
        }

        var usuariosDesdeDb = await _service.ObtenerUsuariosAsync();

        // Eliminamos el SetSize(1) para evitar excepciones silenciosas en .NET
        var opcionesCache = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(60)); 

        _cache.Set(USUARIOS_CACHE_KEY, usuariosDesdeDb, opcionesCache);

        return usuariosDesdeDb ?? new List<Usuario>();
    }

    // =====================================================
    // MÉTODO SÍNCRONO (Mantenido para no romper otras pantallas)
    // =====================================================
    public List<Usuario> ObtenerUsuarios()
    {
        if (_cache.TryGetValue(USUARIOS_CACHE_KEY, out List<Usuario>? usuariosCache))
        {
            return usuariosCache ?? new List<Usuario>();
        }

        var usuariosDesdeDb = _service.ObtenerUsuarios();

        var opcionesCache = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(60)); 

        _cache.Set(USUARIOS_CACHE_KEY, usuariosDesdeDb, opcionesCache);

        return usuariosDesdeDb ?? new List<Usuario>();
    }

    // =====================================================
    // MÉTODOS DE ESCRITURA (Invalidan el Caché instantáneamente)
    // =====================================================
    public void CrearUsuario(Usuario usuario)
    {
        _service.CrearUsuario(usuario);
        LimpiarCache();
    }

    public void ActualizarUsuario(Usuario usuario)
    {
        _service.ActualizarUsuario(usuario);
        LimpiarCache();
    }

    public void EliminarUsuario(Guid id)
    {
        _service.EliminarUsuario(id);
        LimpiarCache();
    }

    public Usuario? ObtenerUsuario(Guid id)
    {
        return _service.ObtenerUsuario(id);
    }

    // =====================================================
    // AYUDANTE: LIMPIAR CACHÉ
    // =====================================================
    private void LimpiarCache()
    {
        _cache.Remove(USUARIOS_CACHE_KEY);
    }
}