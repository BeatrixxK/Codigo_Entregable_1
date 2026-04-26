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
/// <summary>
/// Controlador de nutrición que actúa como intermediario entre la interfaz de usuario y el servicio de nutrición.
/// Conecta el servicio con la interfaz de usuario para gestionar las operaciones nutricionales.
/// </summary>
public class NutricionController
{
    // Servicio de nutrición
    private readonly NutricionService _service;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe el servicio para usar la lógica de negocio
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="NutricionController"/>.
    /// Recibe el servicio de nutrición para utilizar la lógica de negocio.
    /// </summary>
    /// <param name="service">El servicio de nutrición.</param>
    public NutricionController(NutricionService service)
    {
        _service = service;
    }

    // =====================================================
    // MÉTODO OBTENER RESUMEN
    // =====================================================
    // Solicita al servicio el resumen nutricional de un usuario en una fecha
    /// <summary>
    /// Obtiene el resumen nutricional de un usuario para una fecha específica.
    /// Solicita al servicio el resumen nutricional correspondiente al usuario y la fecha proporcionados.
    /// </summary>
    /// <param name="usuarioId">El identificador único del usuario.</param>
    /// <param name="fecha">La fecha para la cual se obtiene el resumen nutricional.</param>
    /// <returns>El resumen nutricional del usuario en la fecha especificada.</returns>
    public ResumenNutricional ObtenerResumen(Guid usuarioId, DateTime fecha)
    {
        return _service.ObtenerResumen(usuarioId, fecha);
    }
}