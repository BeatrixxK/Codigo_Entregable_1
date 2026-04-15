using System.Globalization;
using System.Text.Json;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using StackExchange.Redis;

namespace DragonNutrex.App.Repositories;

public class MenuRedisRepository : IRepository<Menu>
{
    private readonly IDatabase _db;

    private const string PREFIX = "menu";
    private const string SET_KEY = "menus:ids";

    public MenuRedisRepository(RedisConnection redisConnection)
    {
        // ⚠️ Requiere el método GetDatabase() en RedisConnection.cs
        _db = redisConnection.GetDatabase();
    }

    // =====================================================
    // CREATE
    // =====================================================
    public void Create(Menu entity)
    {
        var key = $"{PREFIX}:{entity.Id}";
        var entries = ToHash(entity);

        _db.HashSet(key, entries);
        _db.SetAdd(SET_KEY, entity.Id.ToString());
    }

    // =====================================================
    // UPDATE
    // =====================================================
    public void Update(Menu entity)
    {
        var key = $"{PREFIX}:{entity.Id}";

        if (!_db.KeyExists(key))
            throw new Exception("Menú no encontrado.");

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
            throw new Exception("Menú no encontrado.");

        _db.KeyDelete(key);
        _db.SetRemove(SET_KEY, id.ToString());
    }

    // =====================================================
    // GET ALL
    // =====================================================
    public List<Menu> GetAll()
    {
        var ids = _db.SetMembers(SET_KEY);
        var menus = new List<Menu>();

        foreach (var id in ids)
        {
            if (Guid.TryParse(id.ToString(), out var menuId))
            {
                var menu = GetById(menuId);
                if (menu != null)
                    menus.Add(menu);
            }
        }

        return menus;
    }

    // =====================================================
    // GET BY ID
    // =====================================================
    public Menu? GetById(Guid id)
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
    private HashEntry[] ToHash(Menu m)
    {
        return new HashEntry[]
        {
            new("Id", m.Id.ToString()),
            new("UsuarioId", m.UsuarioId.ToString()),
            new("Fecha", m.Fecha.ToString("O", CultureInfo.InvariantCulture)),
            new("Registros", JsonSerializer.Serialize(m.Registros ?? new List<RegistroComida>()))
        };
    }

    // =====================================================
    // MAPPER → HASH A OBJETO (MEJORADO)
    // =====================================================
    private Menu FromHash(HashEntry[] entries)
    {
        var dict = entries.ToDictionary(
            x => x.Name.ToString(),
            x => x.Value.ToString()
        );

        // 1. Parseo seguro de GUIDs
        _ = Guid.TryParse(dict.GetValueOrDefault("Id"), out var id);
        _ = Guid.TryParse(dict.GetValueOrDefault("UsuarioId"), out var usuarioId);

        // 2. Parseo seguro de Fechas
        var fechaStr = dict.GetValueOrDefault("Fecha", DateTime.UtcNow.ToString("O"));
        _ = DateTime.TryParse(fechaStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var fecha);

        // 3. Deserialización segura de JSON para la lista de comidas
        var registrosStr = dict.GetValueOrDefault("Registros", "[]");
        List<RegistroComida> registros;
        try
        {
            registros = string.IsNullOrWhiteSpace(registrosStr)
                ? new List<RegistroComida>()
                : JsonSerializer.Deserialize<List<RegistroComida>>(registrosStr) ?? new List<RegistroComida>();
        }
        catch (JsonException)
        {
            // Si el JSON en Redis está corrupto, devolvemos una lista vacía en lugar de romper la app
            registros = new List<RegistroComida>(); 
        }

        return new Menu
        {
            Id = id,
            UsuarioId = usuarioId,
            Fecha = fecha,
            Registros = registros
        };
    }
}