using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
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
    // UPDATE (🔥 MODO INFALIBLE - UPSERT)
    // =====================================================
    public void Update(Menu entity)
    {
        var key = $"{PREFIX}:{entity.Id}";

        // Sobrescribimos y reaseguramos su existencia en la lista maestra
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
            throw new Exception("Menú no encontrado.");

        _db.KeyDelete(key);
        _db.SetRemove(SET_KEY, id.ToString());
    }

    // =====================================================
    // GET ALL (Síncrono - Mantenido por compatibilidad)
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
    // GET ALL ASYNC (🚀 LA AUTOPISTA DE ALTA VELOCIDAD)
    // =====================================================
    public async Task<List<Menu>> GetAllAsync()
    {
        var ids = await _db.SetMembersAsync(SET_KEY);
        var menus = new List<Menu>();

        if (ids.Length == 0) return menus;

        // Pipelining: Empacamos todas las consultas en una sola caja
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

        // Un solo viaje a la red
        batch.Execute(); 
        await Task.WhenAll(tareasRedis);

        // Armamos la lista localmente
        for (int i = 0; i < tareasRedis.Count; i++)
        {
            var entries = tareasRedis[i].Result;
            if (entries.Length > 0)
            {
                var menu = FromHash(entries);
                if (Guid.TryParse(idsProcesados[i], out var parsedId))
                {
                    menu.Id = parsedId;
                }
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

        var menu = FromHash(entries);
        menu.Id = id; // Aseguramos sincronización
        return menu;
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

        _ = Guid.TryParse(dict.GetValueOrDefault("Id"), out var id);
        _ = Guid.TryParse(dict.GetValueOrDefault("UsuarioId"), out var usuarioId);

        var fechaStr = dict.GetValueOrDefault("Fecha", DateTime.UtcNow.ToString("O"));
        _ = DateTime.TryParse(fechaStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var fecha);

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