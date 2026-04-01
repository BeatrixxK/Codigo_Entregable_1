using DragonNutrex.App.Models;
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

public class NutricionController
{
    private readonly NutricionService _service;

    public NutricionController(NutricionService service)
    {
        _service = service;
    }

    public ResumenNutricional ObtenerResumen(Guid usuarioId, DateTime fecha)
    {
        return _service.ObtenerResumen(usuarioId, fecha);
    }
}