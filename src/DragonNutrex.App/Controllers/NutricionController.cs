// =====================================================
// IMPORTACIONES
// =====================================================

// Modelos (ResumenNutricional)
using DragonNutrex.App.Models;

// Servicio de nutrición (lógica de negocio)
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

// =====================================================
// CLASE NUTRICION CONTROLLER
// =====================================================
// Actúa como intermediario entre la UI y el servicio de nutrición
// ✔ conecta servicio con UI
public class NutricionController
{
    // Servicio de nutrición
    private readonly NutricionService _service;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe el servicio para usar la lógica de negocio
    public NutricionController(NutricionService service)
    {
        _service = service;
    }

    // =====================================================
    // MÉTODO OBTENER RESUMEN
    // =====================================================
    // Solicita al servicio el resumen nutricional de un usuario en una fecha
    public ResumenNutricional ObtenerResumen(Guid usuarioId, DateTime fecha)
    {
        return _service.ObtenerResumen(usuarioId, fecha);
    }
}