using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using StackExchange.Redis;

namespace DragonNutrex.App.Repositories;

/// <summary>
/// Repositorio de datos para la entidad Usuario utilizando Redis como base de datos.
/// Implementa operaciones CRUD (Crear, Leer, Actualizar, Eliminar) con almacenamiento en memoria
/// utilizando estructuras Hash y un índice secundario por correo electrónico para consultas optimizadas.
/// </summary>
public class UsuarioRedisRepository : IRepository<Usuario>
{
    private readonly IDatabase _db;
    
    // =====================================================
    // 🔥 TUS LLAVES MAESTRAS EXACTAS
    // =====================================================
    private const string PREFIX = "Usuario"; 
    private const string SET_KEY = "usuarios:ids";
    private const string EMAIL_INDEX_PREFIX = "Usuario:email";

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de usuarios en Redis.
    /// </summary>
    /// <param name="redisConnection">Conexión a la base de datos Redis configurada.</param>
    public UsuarioRedisRepository(RedisConnection redisConnection)
    {
        _db = redisConnection.GetDatabase();
    }

    // =====================================================
    // ESCRITURA (Mantienen el Índice Secundario actualizado)
    // =====================================================
    public void Create(Usuario entity) 
    { 
        var key = $"{PREFIX}:{entity.Id}";
        var emailKey = $"{EMAIL_INDEX_PREFIX}:{entity.Correo.ToLower()}"; 

        _db.HashSet(key, ToHash(entity));
        _db.SetAdd(SET_KEY, entity.Id.ToString());
        _db.StringSet(emailKey, entity.Id.ToString()); 
    }

    public void Update(Usuario entity) 
    { 
        var key = $"{PREFIX}:{entity.Id}";
        var emailKey = $"{EMAIL_INDEX_PREFIX}:{entity.Correo.ToLower()}"; 

        _db.HashSet(key, ToHash(entity));
        _db.SetAdd(SET_KEY, entity.Id.ToString());
        _db.StringSet(emailKey, entity.Id.ToString());
    }

    public void Delete(Guid id) 
    { 
        var key = $"{PREFIX}:{id}";
        
        // Buscamos el correo viejo para borrar también su índice secundario
        var user = GetById(id);
        if (user != null && !string.IsNullOrWhiteSpace(user.Correo))
        {
            _db.KeyDelete($"{EMAIL_INDEX_PREFIX}:{user.Correo.ToLower()}");
        }

        _db.KeyDelete(key);
        _db.SetRemove(SET_KEY, id.ToString());
    }

    public List<Usuario> GetAll() 
    {
        return GetAllAsync().GetAwaiter().GetResult(); 
    }

    // =====================================================
    // MOTOR ASÍNCRONO DIRECTO (A prueba de índices rotos)
    // =====================================================
    public async Task<List<Usuario>> GetAllAsync()
    {
        var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
        
        // 1. Buscamos todas las llaves que empiecen con "Usuario:" 
        // y descartamos las que dicen "email" para no chocar con el índice secundario.
        var keys = server.Keys(pattern: $"{PREFIX}:*")
                         .Where(k => !k.ToString().ToLower().Contains("email"))
                         .ToArray();

        var usuarios = new List<Usuario>();
        if (keys.Length == 0) return usuarios;

        var batch = _db.CreateBatch();
        var tareasRedis = new List<Task<HashEntry[]>>();

        // 2. Descargamos todos los Hashes en paralelo (Ultra rápido)
        foreach (var key in keys)
        {
            tareasRedis.Add(batch.HashGetAllAsync(key));
        }

        batch.Execute(); 
        await Task.WhenAll(tareasRedis); 

        // 3. Los convertimos en objetos C#
        foreach (var entries in tareasRedis.Select(t => t.Result))
        {
            if (entries.Length > 0)
            {
                usuarios.Add(FromHash(entries));
            }
        }

        // Ordenamos alfabéticamente para que se vea bonito en las tablas
        return usuarios.OrderBy(u => u.Nombre).ToList();
    }

    // =====================================================
    // 🔥 LA JOYA DE LA CORONA: LOGIN A LA VELOCIDAD DE LA LUZ
    // =====================================================
    public async Task<Usuario?> GetByCorreoAsync(string correo)
    {
        if (string.IsNullOrWhiteSpace(correo)) return null;

        // 1. Buscamos directo en el Índice Secundario usando la constante maestra
        var llaveCorreo = $"{EMAIL_INDEX_PREFIX}:{correo.ToLower()}";
        var idDelUsuario = await _db.StringGetAsync(llaveCorreo);

        // Si no existe, el correo no está registrado
        if (!idDelUsuario.HasValue) 
        {
            return null; 
        }

        // 2. Vamos directo a abrir la carpeta HASH de ese usuario
        var llaveUsuario = $"{PREFIX}:{idDelUsuario}";
        var entries = await _db.HashGetAllAsync(llaveUsuario);

        // 3. Convertimos y retornamos (0.001s)
        if (entries.Length > 0)
        {
            return FromHash(entries);
        }

        return null; 
    }

    public Usuario? GetById(Guid id) 
    {
        var key = $"{PREFIX}:{id}";
        var entries = _db.HashGetAll(key);
        if (entries.Length == 0) return null;
        var usuario = FromHash(entries);
        usuario.Id = id; 
        return usuario;
    }

    // =====================================================
    // MAPEO DE DATOS
    // =====================================================
    private HashEntry[] ToHash(Usuario u) 
    {
        return new HashEntry[] {
            new("Id", u.Id.ToString()), 
            new("Nombre", u.Nombre ?? ""),
            new("Usuario", u.Correo ?? ""), // Guardamos como "Usuario" en Redis
            new("Peso", u.Peso.ToString(CultureInfo.InvariantCulture)),
            new("Altura", u.Altura.ToString(CultureInfo.InvariantCulture)),
            new("Actividad", u.Actividad ?? ""), 
            new("Objetivo", u.Objetivo ?? ""),
            new("TipoDieta", u.TipoDieta ?? ""), 
            new("Password", u.Password ?? ""),
            new("Activo", u.Activo.ToString())
        };
    }

    private Usuario FromHash(HashEntry[] entries) 
    {
        var dict = entries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        if (!Guid.TryParse(dict.GetValueOrDefault("Id"), out var id)) id = Guid.NewGuid();
        _ = decimal.TryParse(dict.GetValueOrDefault("Peso", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var peso);
        _ = decimal.TryParse(dict.GetValueOrDefault("Altura", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var altura);
        _ = bool.TryParse(dict.GetValueOrDefault("Activo", "True"), out var activo); 

        return new Usuario {
            Id = id, 
            Nombre = dict.GetValueOrDefault("Nombre", ""), 
            Correo = dict.GetValueOrDefault("Usuario", ""), // Leemos el campo "Usuario"
            Peso = peso, 
            Altura = altura,
            Actividad = dict.GetValueOrDefault("Actividad", ""), 
            Objetivo = dict.GetValueOrDefault("Objetivo", ""),
            TipoDieta = dict.GetValueOrDefault("TipoDieta", ""), 
            Password = dict.GetValueOrDefault("Password", ""),
            Activo = activo
        };
    }
}