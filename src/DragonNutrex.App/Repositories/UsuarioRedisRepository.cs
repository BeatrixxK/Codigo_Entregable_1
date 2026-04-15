using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using StackExchange.Redis;

namespace DragonNutrex.App.Repositories;

public class UsuarioRedisRepository : IRepository<Usuario>
{
    private readonly IDatabase _db;

    private const string PREFIX = "usuario";
    private const string SET_KEY = "usuarios:ids";

    public UsuarioRedisRepository(RedisConnection redisConnection)
    {
        _db = redisConnection.GetDatabase();
    }

    // =====================================================
    // CREATE
    // =====================================================
    public void Create(Usuario entity)
    {
        var key = $"{PREFIX}:{entity.Id}";
        var entries = ToHash(entity);

        _db.HashSet(key, entries);
        _db.SetAdd(SET_KEY, entity.Id.ToString());
    }

    // =====================================================
    // UPDATE (🔥 MODO INFALIBLE - UPSERT)
    // =====================================================
    public void Update(Usuario entity)
    {
        var key = $"{PREFIX}:{entity.Id}";

        // Hemos eliminado el "if (!_db.KeyExists)" que causaba el error.
        // Ahora, el sistema simplemente sobreescribe los datos asegurando el guardado.
        
        var entries = ToHash(entity);
        _db.HashSet(key, entries);
        
        // Lo re-vinculamos a la lista maestra por si era un usuario huérfano
        _db.SetAdd(SET_KEY, entity.Id.ToString());
    }

    // =====================================================
    // DELETE
    // =====================================================
    public void Delete(Guid id)
    {
        var key = $"{PREFIX}:{id}";

        if (!_db.KeyExists(key))
            throw new Exception("Usuario no encontrado.");

        _db.KeyDelete(key);
        _db.SetRemove(SET_KEY, id.ToString());
    }

    // =====================================================
    // GET ALL (🔥 SINCRONIZACIÓN PERFECTA DE IDs)
    // =====================================================
    public List<Usuario> GetAll()
    {
        var ids = _db.SetMembers(SET_KEY);
        var usuarios = new List<Usuario>();

        foreach (var id in ids)
        {
            var rawId = id.ToString().Trim();
            var key = $"{PREFIX}:{rawId}";
            
            var entries = _db.HashGetAll(key);

            if (entries.Length > 0)
            {
                var usuario = FromHash(entries);
                
                // Forzamos que el objeto en memoria de C# tenga exactamente 
                // el mismo ID de la llave de Redis, para evitar errores al guardar.
                if (Guid.TryParse(rawId, out var parsedId))
                {
                    usuario.Id = parsedId;
                }
                
                usuarios.Add(usuario);
            }
        }

        return usuarios;
    }

    // =====================================================
    // GET BY ID
    // =====================================================
    public Usuario? GetById(Guid id)
    {
        var key = $"{PREFIX}:{id}";
        var entries = _db.HashGetAll(key);

        if (entries.Length == 0)
            return null;

        var usuario = FromHash(entries);
        usuario.Id = id; // Sincronización también aplicada aquí
        return usuario;
    }

    // =====================================================
    // MAPPER → OBJETO A HASH
    // =====================================================
    private HashEntry[] ToHash(Usuario u)
    {
        return new HashEntry[]
        {
            new("Id", u.Id.ToString()),
            new("Nombre", u.Nombre ?? string.Empty),
            new("Peso", u.Peso.ToString(CultureInfo.InvariantCulture)),
            new("Altura", u.Altura.ToString(CultureInfo.InvariantCulture)),
            new("Actividad", u.Actividad ?? string.Empty),
            new("Objetivo", u.Objetivo ?? string.Empty),
            new("TipoDieta", u.TipoDieta ?? string.Empty),
            new("Password", u.Password ?? string.Empty)
        };
    }

    // =====================================================
    // MAPPER → HASH A OBJETO
    // =====================================================
    private Usuario FromHash(HashEntry[] entries)
    {
        var dict = entries.ToDictionary(
            x => x.Name.ToString(),
            x => x.Value.ToString()
        );

        if (!Guid.TryParse(dict.GetValueOrDefault("Id"), out var id))
        {
            id = Guid.NewGuid();
        }

        _ = decimal.TryParse(dict.GetValueOrDefault("Peso", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var peso);
        _ = decimal.TryParse(dict.GetValueOrDefault("Altura", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var altura);

        return new Usuario
        {
            Id = id,
            Nombre = dict.GetValueOrDefault("Nombre", string.Empty),
            Peso = peso,
            Altura = altura,
            Actividad = dict.GetValueOrDefault("Actividad", string.Empty),
            Objetivo = dict.GetValueOrDefault("Objetivo", string.Empty),
            TipoDieta = dict.GetValueOrDefault("TipoDieta", string.Empty),
            Password = dict.GetValueOrDefault("Password", string.Empty)
        };
    }
}