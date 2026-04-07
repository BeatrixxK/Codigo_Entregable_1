// =====================================================
// IMPORTACIONES
// =====================================================

// Modelos (TipoDietaMeta, ResumenEstadisticaNutricional)
using DragonNutrex.App.Models;

// Servicio de estadísticas nutricionales
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

// =====================================================
// CLASE ESTADISTICAS NUTRICION CONTROLLER
// =====================================================
// Actúa como intermediario entre la UI y el servicio de estadísticas
// ✔ conecta servicio con UI
public class EstadisticasNutricionController
{
    // Servicio de estadísticas nutricionales
    private readonly EstadisticasNutricionService _service;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe el servicio para usar la lógica de negocio
    public EstadisticasNutricionController(EstadisticasNutricionService service)
    {
        _service = service;
    }

    // =====================================================
    // MÉTODO OBTENER DIETAS DISPONIBLES
    // =====================================================
    // Solicita al servicio la lista de dietas con sus objetivos
    public List<TipoDietaMeta> ObtenerDietasDisponibles()
    {
        return _service.ObtenerDietasDisponibles();
    }

    // =====================================================
    // MÉTODO OBTENER RESUMEN
    // =====================================================
    // Solicita al servicio el resumen estadístico nutricional
    // de un usuario en un rango de fechas y según una dieta
    public ResumenEstadisticaNutricional ObtenerResumen(
        Guid usuarioId,
        DateTime fechaInicio,
        DateTime fechaFin,
        string tipoDieta)
    {
        return _service.ObtenerResumen(usuarioId, fechaInicio, fechaFin, tipoDieta);
    }
}