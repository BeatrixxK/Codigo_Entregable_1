// =====================================================
// IMPORTACIONES
// =====================================================
using System;
using System.Collections.Generic;
using System.Linq;
using DragonNutrex.App.Models;
using DragonNutrex.App.Services;
using DragonNutrex.App.Repositories; // Necesario para consultar Redis directamente

namespace DragonNutrex.App.Controllers;

// =====================================================
// CLASE ESTADISTICAS NUTRICION CONTROLLER
// =====================================================
public class EstadisticasNutricionController
{
    // Servicios y Repositorios
    private readonly EstadisticasNutricionService _service;
    private readonly UsuarioRedisRepository _usuarioRepository;
    private readonly MenuRedisRepository _menuRepository;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Ahora inyectamos los repositorios para poder hacer consultas globales (LINQ)
    public EstadisticasNutricionController(
        EstadisticasNutricionService service,
        UsuarioRedisRepository usuarioRepository,
        MenuRedisRepository menuRepository)
    {
        _service = service;
        _usuarioRepository = usuarioRepository;
        _menuRepository = menuRepository;
    }

    // =====================================================
    // MÉTODOS ORIGINALES (Fase 1)
    // =====================================================
    
    public List<TipoDietaMeta> ObtenerDietasDisponibles()
    {
        return _service.ObtenerDietasDisponibles();
    }

    public ResumenEstadisticaNutricional ObtenerResumen(
        Guid usuarioId,
        DateTime fechaInicio,
        DateTime fechaFin,
        string tipoDieta)
    {
        return _service.ObtenerResumen(usuarioId, fechaInicio, fechaFin, tipoDieta);
    }

    // =====================================================
    // NUEVAS ESTADÍSTICAS AVANZADAS (Fase 2)
    // =====================================================

    // 1. Porcentaje de dietas de todos los usuarios
    public Dictionary<string, double> ObtenerPorcentajesDietas()
    {
        // Filtramos solo los usuarios activos
        var usuarios = _usuarioRepository.GetAll().Where(u => u.Activo).ToList();
        var total = usuarios.Count;
        
        if (total == 0) return new Dictionary<string, double>();

        // Agrupamos por dieta y sacamos el porcentaje
        return usuarios
            .GroupBy(u => string.IsNullOrWhiteSpace(u.TipoDieta) ? "Sin especificar" : u.TipoDieta)
            .ToDictionary(
                g => g.Key, 
                g => Math.Round((double)g.Count() / total * 100, 2)
            );
    }

    // 2. Ranking de usuarios con más menús (Top 5 por defecto)
    public List<KeyValuePair<string, int>> ObtenerTopUsuariosConMasMenus(int cantidadTop = 5)
    {
        var menus = _menuRepository.GetAll();
        var usuarios = _usuarioRepository.GetAll();

        return menus
            .GroupBy(m => m.UsuarioId)
            .Select(g => new KeyValuePair<string, int>(
                // Buscamos el nombre del usuario, si no existe ponemos "Desconocido"
                usuarios.FirstOrDefault(u => u.Id == g.Key)?.Nombre ?? "Usuario Desconocido",
                g.Count() // Contamos cuántos menús tiene
            ))
            .OrderByDescending(x => x.Value) // Del que tiene más al que tiene menos
            .Take(cantidadTop) // Sacamos el Top X
            .ToList();
    }

    // 3. Producto más consumido en un rango de fechas
    public string ObtenerProductoMasConsumido(DateTime fechaInicio, DateTime fechaFin)
    {
        // Traemos los menús que caen en las fechas indicadas
        var menusEnRango = _menuRepository.GetAll()
            .Where(m => m.Fecha.Date >= fechaInicio.Date && m.Fecha.Date <= fechaFin.Date)
            .ToList();

        if (!menusEnRango.Any()) return "Sin datos en estas fechas";

        // Extrae todos los registros de comida de esos menús (aplanamos la lista)
        var todosLosRegistros = menusEnRango.SelectMany(m => m.Registros ?? new List<RegistroComida>());

        // Agrupa por nombre de producto y sumamos las cantidades
        var productoTop = todosLosRegistros
            .GroupBy(r => r.NombreProducto)
            .OrderByDescending(g => g.Sum(r => r.Cantidad))
            .FirstOrDefault();

        if (productoTop == null) return "Sin datos consumidos";

        return $"{productoTop.Key} (Consumido: {productoTop.Sum(r => r.Cantidad):F2} unidades)";
    }
}