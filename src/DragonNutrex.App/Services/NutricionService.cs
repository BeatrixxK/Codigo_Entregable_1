using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

public class NutricionService
{
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IRepository<Menu> _menuRepository;

    public NutricionService(
        IRepository<Usuario> usuarioRepository,
        IRepository<Menu> menuRepository)
    {
        _usuarioRepository = usuarioRepository;
        _menuRepository = menuRepository;
    }

    public ResumenNutricional ObtenerResumen(Guid usuarioId, DateTime fecha)
    {
        var usuario = _usuarioRepository.GetById(usuarioId);
        if (usuario == null)
            throw new Exception("Usuario no encontrado.");

        var menu = _menuRepository
            .GetAll()
            .FirstOrDefault(m => m.UsuarioId == usuarioId && m.Fecha.Date == fecha.Date);

        if (menu == null)
            throw new Exception("No existe un menú para ese usuario en esa fecha.");

        var imc = CalcularIMC(usuario);
        var categoriaImc = ObtenerCategoriaIMC(imc);

        var caloriasObjetivo = CalcularCaloriasObjetivo(usuario);
        var proteinasObjetivo = Math.Round(usuario.Peso * 2.0m, 2);
        var grasasObjetivo = Math.Round(usuario.Peso * 0.8m, 2);

        var caloriasRestantesParaCarbos =
            caloriasObjetivo - (proteinasObjetivo * 4m) - (grasasObjetivo * 9m);

        var carbohidratosObjetivo = Math.Round(caloriasRestantesParaCarbos / 4m, 2);

        if (carbohidratosObjetivo < 0)
            carbohidratosObjetivo = 0;

        return new ResumenNutricional
        {
            NombreUsuario = usuario.Nombre,
            Fecha = fecha.Date,

            Imc = imc,
            CategoriaImc = categoriaImc,

            CaloriasConsumidas = menu.TotalCalorias,
            ProteinasConsumidas = menu.TotalProteinas,
            CarbohidratosConsumidos = menu.TotalCarbohidratos,
            GrasasConsumidas = menu.TotalGrasas,

            CaloriasObjetivo = caloriasObjetivo,
            ProteinasObjetivo = proteinasObjetivo,
            CarbohidratosObjetivo = carbohidratosObjetivo,
            GrasasObjetivo = grasasObjetivo,

            EstadoCalorico = ObtenerEstado(menu.TotalCalorias, caloriasObjetivo),
            Recomendacion = ObtenerRecomendacion(menu.TotalCalorias, caloriasObjetivo, usuario.Objetivo)
        };
    }

    private decimal CalcularIMC(Usuario usuario)
    {
        if (usuario.Altura <= 0)
            return 0;

        return Math.Round(usuario.Peso / (usuario.Altura * usuario.Altura), 2);
    }

    private string ObtenerCategoriaIMC(decimal imc)
    {
        if (imc < 18.5m) return "Bajo peso";
        if (imc < 25m) return "Normal";
        if (imc < 30m) return "Sobrepeso";
        return "Obesidad";
    }

    private decimal CalcularCaloriasObjetivo(Usuario usuario)
    {
        decimal baseCalorias = usuario.Peso * 24m;

        var actividad = (usuario.Actividad ?? "").Trim().ToLowerInvariant();
        var objetivo = (usuario.Objetivo ?? "").Trim().ToLowerInvariant();

        decimal factorActividad = actividad switch
        {
            "sedentaria" => 1.2m,
            "ligera" => 1.375m,
            "moderada" => 1.55m,
            "alta" => 1.725m,
            _ => 1.2m
        };

        decimal mantenimiento = baseCalorias * factorActividad;

        decimal resultado = objetivo switch
        {
            "bajar peso" => mantenimiento - 300m,
            "ganar masa" => mantenimiento + 300m,
            _ => mantenimiento
        };

        return Math.Round(resultado, 2);
    }

    private string ObtenerEstado(decimal consumidas, decimal objetivo)
    {
        if (consumidas < objetivo * 0.9m)
            return "Por debajo del objetivo";

        if (consumidas > objetivo * 1.1m)
            return "Por encima del objetivo";

        return "Dentro del rango recomendado";
    }

    private string ObtenerRecomendacion(decimal consumidas, decimal objetivo, string objetivoUsuario)
    {
        if (consumidas < objetivo * 0.9m)
            return "Se recomienda aumentar la ingesta diaria.";

        if (consumidas > objetivo * 1.1m)
            return "Se recomienda reducir la ingesta diaria.";

        return $"El consumo está alineado con el objetivo: {objetivoUsuario}.";
    }
}