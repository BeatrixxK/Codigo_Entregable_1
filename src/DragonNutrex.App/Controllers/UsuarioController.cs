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
    // 🔥 NUEVO: LOGIN ULTRA RÁPIDO (Por Correo)
    // =====================================================
    public async Task<Usuario?> AutenticarUsuarioAsync(string correo, string password)
    {
        var userDb = await ObtenerUsuarioPorCorreoAsync(correo);
        
        if (userDb != null && userDb.Password == password && userDb.Activo)
        {
            return userDb;
        }
        return null;
    }

    // =====================================================
    // 🔥 NUEVO: BÚSQUEDA INDIVIDUAL CON CACHÉ (Carga en 0ms)
    // =====================================================
    public async Task<Usuario?> ObtenerUsuarioPorCorreoAsync(string correo)
    {
        if (string.IsNullOrWhiteSpace(correo)) return null;

        string cacheKey = $"user_{correo.ToLower()}";
        
        // 1. Buscamos en RAM primero para que MiProgreso cargue al instante
        if (_cache.TryGetValue(cacheKey, out Usuario? userCache))
        {
            return userCache;
        }

        // 2. Si no está, vamos a la base de datos directo a buscar ese correo
        var usuarioDb = await _service.GetByCorreoAsync(correo);

        // 3. Lo guardamos en RAM por 30 minutos
        if (usuarioDb != null)
        {
            _cache.Set(cacheKey, usuarioDb, TimeSpan.FromMinutes(30));
        }

        return usuarioDb;
    }

    // =====================================================
    // MÉTODO ASÍNCRONO MAESTRO (Para la vista de Admin)
    // =====================================================
    public async Task<List<Usuario>> ObtenerUsuariosAsync()
    {
        if (_cache.TryGetValue(USUARIOS_CACHE_KEY, out List<Usuario>? usuariosCache))
        {
            return usuariosCache ?? new List<Usuario>();
        }

        var usuariosDesdeDb = await _service.ObtenerUsuariosAsync();

        var opcionesCache = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(60)); 

        _cache.Set(USUARIOS_CACHE_KEY, usuariosDesdeDb, opcionesCache);

        return usuariosDesdeDb ?? new List<Usuario>();
    }

    // =====================================================
    // MÉTODO SÍNCRONO MAESTRO (Por compatibilidad vieja)
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
    // MÉTODOS DE ESCRITURA (Limpian la lista y el caché individual)
    // =====================================================
    public void CrearUsuario(Usuario usuario)
    {
        _service.CrearUsuario(usuario);
        LimpiarCaches(usuario.Correo);
    }

    public void ActualizarUsuario(Usuario usuario)
    {
        _service.ActualizarUsuario(usuario);
        LimpiarCaches(usuario.Correo);
    }

    public void EliminarUsuario(Guid id)
    {
        var usuarioViejo = ObtenerUsuario(id);
        _service.EliminarUsuario(id);
        LimpiarCaches(usuarioViejo?.Correo);
    }

    public Usuario? ObtenerUsuario(Guid id)
    {
        return _service.ObtenerUsuario(id);
    }

    // =====================================================
    // AYUDANTE: DESTRUCTOR DE CACHÉ
    // =====================================================
    private void LimpiarCaches(string? correo)
    {
        _cache.Remove(USUARIOS_CACHE_KEY); // Limpia la lista del Admin
        
        if (!string.IsNullOrWhiteSpace(correo))
        {
            _cache.Remove($"user_{correo.ToLower()}"); // Limpia el caché individual de ese usuario
        }
    }
}