using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using StackExchange.Redis;

namespace DragonNutrex.App.Repositories;

public class ProductoRedisRepository : IRepository<Producto>
{
    private readonly IDatabase _db;

    private const string PREFIX = "producto";
    private const string SET_KEY = "productos:ids";

    public ProductoRedisRepository(RedisConnection redisConnection)
    {
        _db = redisConnection.GetDatabase();
    }

    // =====================================================
    // CREATE
    // =====================================================
    public void Create(Producto entity)
    {
        var key = $"{PREFIX}:{entity.Id}";
        var entries = ToHash(entity);

        _db.HashSet(key, entries);
        _db.SetAdd(SET_KEY, entity.Id.ToString());
    }

    // =====================================================
    // UPDATE (🔥 MODO INFALIBLE - UPSERT)
    // =====================================================
    public void Update(Producto entity)
    {
        var key = $"{PREFIX}:{entity.Id}";

        // Eliminamos el KeyExists. Ahora si el admin actualiza, 
        // simplemente sobrescribimos y aseguramos que esté en la lista.
        var entries = ToHash(entity);
        _db.HashSet(key, entries);
        _db.SetAdd(SET_KEY, entity.Id.ToString());
    }

    // =====================================================
    // DELETE
    // =====================================================
    public void Delete(Guid id)
    {
        var key = $"{PREFIX}:{id}";

        if (!_db.KeyExists(key))
            throw new Exception("Producto no encontrado.");

        _db.KeyDelete(key);
        _db.SetRemove(SET_KEY, id.ToString());
    }

    // =====================================================
    // GET ALL (Síncrono - Mantenido por compatibilidad)
    // =====================================================
    public List<Producto> GetAll()
    {
        var ids = _db.SetMembers(SET_KEY);
        var productos = new List<Producto>();

        foreach (var id in ids)
        {
            if (Guid.TryParse(id.ToString(), out var productoId))
            {
                var producto = GetById(productoId);
                if (producto != null)
                    productos.Add(producto);
            }
        }

        return productos;
    }

    // =====================================================
    // GET ALL ASYNC (🚀 LA AUTOPISTA DE ALTA VELOCIDAD)
    // =====================================================
    public async Task<List<Producto>> GetAllAsync()
    {
        var ids = await _db.SetMembersAsync(SET_KEY);
        var productos = new List<Producto>();

        if (ids.Length == 0) return productos;

        // Empacamos todas las peticiones en una sola caja (Pipelining)
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

        // Enviamos la caja en un solo viaje de red a Redis Cloud
        batch.Execute(); 
        await Task.WhenAll(tareasRedis);

        // Desempacamos
        for (int i = 0; i < tareasRedis.Count; i++)
        {
            var entries = tareasRedis[i].Result;
            if (entries.Length > 0)
            {
                var producto = FromHash(entries);
                if (Guid.TryParse(idsProcesados[i], out var parsedId))
                {
                    producto.Id = parsedId;
                }
                productos.Add(producto);
            }
        }

        return productos;
    }

    // =====================================================
    // GET BY ID
    // =====================================================
    public Producto? GetById(Guid id)
    {
        var key = $"{PREFIX}:{id}";
        var entries = _db.HashGetAll(key);

        if (entries.Length == 0)
            return null;

        var producto = FromHash(entries);
        producto.Id = id; // Aseguramos que el ID empate con el buscado
        return producto;
    }

    // =====================================================
    // MAPPER → OBJETO A HASH
    // =====================================================
    private HashEntry[] ToHash(Producto p)
    {
        return new HashEntry[]
        {
            new("Id", p.Id.ToString()),
            new("Nombre", p.Nombre ?? string.Empty),
            new("Calorias", p.Calorias.ToString(CultureInfo.InvariantCulture)),
            new("Proteinas", p.Proteinas.ToString(CultureInfo.InvariantCulture)),
            new("Carbohidratos", p.Carbohidratos.ToString(CultureInfo.InvariantCulture)),
            new("Grasas", p.Grasas.ToString(CultureInfo.InvariantCulture))
        };
    }

    // =====================================================
    // MAPPER → HASH A OBJETO
    // =====================================================
    private Producto FromHash(HashEntry[] entries)
    {
        var dict = entries.ToDictionary(
            x => x.Name.ToString(), 
            x => x.Value.ToString()
        );

        _ = Guid.TryParse(dict.GetValueOrDefault("Id"), out var id);
        _ = decimal.TryParse(dict.GetValueOrDefault("Calorias", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var calorias);
        _ = decimal.TryParse(dict.GetValueOrDefault("Proteinas", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var proteinas);
        _ = decimal.TryParse(dict.GetValueOrDefault("Carbohidratos", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var carbohidratos);
        _ = decimal.TryParse(dict.GetValueOrDefault("Grasas", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var grasas);

        return new Producto
        {
            Id = id,
            Nombre = dict.GetValueOrDefault("Nombre", string.Empty),
            Calorias = calorias,
            Proteinas = proteinas,
            Carbohidratos = carbohidratos,
            Grasas = grasas
        };
    }
}