using DragonNutrex.App.Models;
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

public class EstadisticasNutricionController
{
    private readonly EstadisticasNutricionService _service;

    public EstadisticasNutricionController(EstadisticasNutricionService service)
    {
        _service = service;
    }

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
}