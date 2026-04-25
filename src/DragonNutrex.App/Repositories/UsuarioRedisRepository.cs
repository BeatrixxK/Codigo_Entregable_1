using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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

    public void Create(Usuario entity) { /* Igual que antes */ 
        var key = $"{PREFIX}:{entity.Id}";
        _db.HashSet(key, ToHash(entity));
        _db.SetAdd(SET_KEY, entity.Id.ToString());
    }

    public void Update(Usuario entity) { /* Igual que antes */ 
        var key = $"{PREFIX}:{entity.Id}";
        _db.HashSet(key, ToHash(entity));
        _db.SetAdd(SET_KEY, entity.Id.ToString());
    }

    public void Delete(Guid id) { /* Igual que antes */ 
        var key = $"{PREFIX}:{id}";
        _db.KeyDelete(key);
        _db.SetRemove(SET_KEY, id.ToString());
    }

    public List<Usuario> GetAll() {
        return GetAllAsync().GetAwaiter().GetResult(); // Solo por compatibilidad vieja
    }

    // =====================================================
    // EL NUEVO MOTOR ASÍNCRONO SIN BLOQUEOS
    // =====================================================
    public async Task<List<Usuario>> GetAllAsync()
    {
        var ids = await _db.SetMembersAsync(SET_KEY);
        var usuarios = new List<Usuario>();

        if (ids.Length == 0) return usuarios;

        var batch = _db.CreateBatch();
        var tareasRedis = new List<Task<HashEntry[]>>();
        var idsProcesados = new List<string>();

        foreach (var id in ids)
        {
            var rawId = id.ToString().Trim();
            var key = $"{PREFIX}:{rawId}";
            tareasRedis.Add(batch.HashGetAllAsync(key));
            idsProcesados.Add(rawId);
        }

        batch.Execute(); 
        await Task.WhenAll(tareasRedis); // ¡Aquí la pantalla NO se congela!

        for (int i = 0; i < tareasRedis.Count; i++)
        {
            var entries = tareasRedis[i].Result;
            if (entries.Length > 0)
            {
                var usuario = FromHash(entries);
                if (Guid.TryParse(idsProcesados[i], out var parsedId))
                {
                    usuario.Id = parsedId;
                }
                usuarios.Add(usuario);
            }
        }

        return usuarios;
    }

    public Usuario? GetById(Guid id) {
        var key = $"{PREFIX}:{id}";
        var entries = _db.HashGetAll(key);
        if (entries.Length == 0) return null;
        var usuario = FromHash(entries);
        usuario.Id = id; 
        return usuario;
    }

    private HashEntry[] ToHash(Usuario u) {
        return new HashEntry[] {
            new("Id", u.Id.ToString()), new("Nombre", u.Nombre ?? ""),
            new("Peso", u.Peso.ToString(CultureInfo.InvariantCulture)),
            new("Altura", u.Altura.ToString(CultureInfo.InvariantCulture)),
            new("Actividad", u.Actividad ?? ""), new("Objetivo", u.Objetivo ?? ""),
            new("TipoDieta", u.TipoDieta ?? ""), new("Password", u.Password ?? ""),
            new("Activo", u.Activo.ToString())
        };
    }

    private Usuario FromHash(HashEntry[] entries) {
        var dict = entries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        if (!Guid.TryParse(dict.GetValueOrDefault("Id"), out var id)) id = Guid.NewGuid();
        _ = decimal.TryParse(dict.GetValueOrDefault("Peso", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var peso);
        _ = decimal.TryParse(dict.GetValueOrDefault("Altura", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var altura);
        _ = bool.TryParse(dict.GetValueOrDefault("Activo", "True"), out var activo); 

        return new Usuario {
            Id = id, Nombre = dict.GetValueOrDefault("Nombre", ""), Peso = peso, Altura = altura,
            Actividad = dict.GetValueOrDefault("Actividad", ""), Objetivo = dict.GetValueOrDefault("Objetivo", ""),
            TipoDieta = dict.GetValueOrDefault("TipoDieta", ""), Password = dict.GetValueOrDefault("Password", ""),
            Activo = activo
        };
    }
}