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

    // Prefijo con "M" mayúscula para coincidir con la BD
    private const string PREFIX = "Menu"; 
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
    // UPDATE
    // =====================================================
    public void Update(Menu entity)
    {
        var key = $"{PREFIX}:{entity.Id}";

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
    // GET ALL ASYNC
    // =====================================================
    public async Task<List<Menu>> GetAllAsync()
    {
        var ids = await _db.SetMembersAsync(SET_KEY);
        var menus = new List<Menu>();

        if (ids.Length == 0) return menus;

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
        await Task.WhenAll(tareasRedis);

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
        menu.Id = id; 
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
            new("Registros", JsonSerializer.Serialize(m.Registros ?? new List<RegistroComida>())),
            new("TotalCalorias", m.TotalCalorias.ToString(CultureInfo.InvariantCulture)),
            new("TotalProteinas", m.TotalProteinas.ToString(CultureInfo.InvariantCulture)),
            new("TotalCarbohidratos", m.TotalCarbohidratos.ToString(CultureInfo.InvariantCulture)),
            new("TotalGrasas", m.TotalGrasas.ToString(CultureInfo.InvariantCulture))
        };
    }

    // =====================================================
    // MAPPER → HASH A OBJETO (CON REPARTO DE MACROS)
    // =====================================================
    private Menu FromHash(HashEntry[] entries)
    {
        var dict = entries.ToDictionary(
            x => x.Name.ToString(),
            x => x.Value.ToString()
        );

        _ = Guid.TryParse(dict.GetValueOrDefault("Id"), out var id);
        _ = Guid.TryParse(dict.GetValueOrDefault("UsuarioId"), out var usuarioId);

        // 1. FECHA
        var fechaStr = dict.GetValueOrDefault("Fecha", DateTime.UtcNow.ToString("O"));
        if (!DateTime.TryParseExact(fechaStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
        {
            _ = DateTime.TryParse(fechaStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out fecha);
        }

        // 2. REGISTROS
        var registrosStr = dict.GetValueOrDefault("Registros", "[]");
        var registros = new List<RegistroComida>();
        bool esDePython = false;

        if (!string.IsNullOrWhiteSpace(registrosStr))
        {
            if (registrosStr.Trim().StartsWith("[")) 
            {
                try { registros = JsonSerializer.Deserialize<List<RegistroComida>>(registrosStr) ?? new(); }
                catch { }
            }
            else 
            {
                esDePython = true;
                registros = registrosStr.Split(',')
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Select(nombre => new RegistroComida { 
                        Id = Guid.NewGuid(),
                        NombreProducto = nombre.Trim(), 
                        Cantidad = 1m 
                    }).ToList();
            }
        }

        // 3. TOTALES NUTRICIONALES REALES DE REDIS
        _ = decimal.TryParse(dict.GetValueOrDefault("TotalCalorias", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var cals);
        _ = decimal.TryParse(dict.GetValueOrDefault("TotalProteinas", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var prot);
        _ = decimal.TryParse(dict.GetValueOrDefault("TotalCarbohidratos", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var carb);
        _ = decimal.TryParse(dict.GetValueOrDefault("TotalGrasas", "0"), NumberStyles.Any, CultureInfo.InvariantCulture, out var gras);

        // 4. EL REPARTO EQUITATIVO (Para que sume correctamente en la UI)
        if (esDePython && registros.Any())
        {
            var cantidadAlimentos = registros.Count;
            foreach (var r in registros)
            {
                r.Calorias = cals / cantidadAlimentos;
                r.Proteinas = prot / cantidadAlimentos;
                r.Carbohidratos = carb / cantidadAlimentos;
                r.Grasas = gras / cantidadAlimentos;
            }
        }

        return new Menu
        {
            Id = id,
            UsuarioId = usuarioId,
            Fecha = fecha,
            Registros = registros,
            TotalCalorias = cals,
            TotalProteinas = prot,
            TotalCarbohidratos = carb,
            TotalGrasas = gras
        };
    }
}