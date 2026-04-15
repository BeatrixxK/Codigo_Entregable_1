using System.Globalization;
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
        // Depende de la corrección en RedisConnection.cs para resolver CS1061
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
    // UPDATE
    // =====================================================
    public void Update(Producto entity)
    {
        var key = $"{PREFIX}:{entity.Id}";

        if (!_db.KeyExists(key))
            throw new Exception("Producto no encontrado.");

        var entries = ToHash(entity);
        _db.HashSet(key, entries);
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
    // GET ALL
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
    // GET BY ID
    // =====================================================
    public Producto? GetById(Guid id)
    {
        var key = $"{PREFIX}:{id}";
        var entries = _db.HashGetAll(key);

        if (entries.Length == 0)
            return null;

        return FromHash(entries);
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
    // MAPPER → HASH A OBJETO (MEJORADO)
    // =====================================================
    private Producto FromHash(HashEntry[] entries)
    {
        var dict = entries.ToDictionary(
            x => x.Name.ToString(), 
            x => x.Value.ToString()
        );

        // Uso de Parseo seguro para evitar excepciones si los valores son nulos o están corruptos
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