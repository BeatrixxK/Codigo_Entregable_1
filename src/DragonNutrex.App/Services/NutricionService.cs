// =====================================================
// IMPORTACIONES
// =====================================================

// Interfaz del repositorio genérico
using DragonNutrex.App.Interfaces;

// Modelos de la aplicación
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

// =====================================================
// CLASE NUTRICION SERVICE
// =====================================================
// Maneja la lógica de negocio relacionada con nutrición
// Calcula IMC, objetivos nutricionales y recomendaciones
public class NutricionService
{
    // Repositorio de usuarios
    private readonly IRepository<Usuario> _usuarioRepository;

    // Repositorio de menús
    private readonly IRepository<Menu> _menuRepository;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe los repositorios necesarios para consultar datos
    public NutricionService(
        IRepository<Usuario> usuarioRepository,
        IRepository<Menu> menuRepository)
    {
        _usuarioRepository = usuarioRepository;
        _menuRepository = menuRepository;
    }

    // =====================================================
    // MÉTODO OBTENER RESUMEN
    // =====================================================
    // Genera un resumen nutricional de un usuario en una fecha específica
    public ResumenNutricional ObtenerResumen(Guid usuarioId, DateTime fecha)
    {
        // Busca el usuario por ID
        var usuario = _usuarioRepository.GetById(usuarioId);

        // Si no existe, lanza error
        if (usuario == null)
            throw new Exception("Usuario no encontrado.");

        // Busca el menú del usuario en la fecha indicada
        var menu = _menuRepository
            .GetAll()
            .FirstOrDefault(m => m.UsuarioId == usuarioId && m.Fecha.Date == fecha.Date);

        // Si no existe menú, lanza error
        if (menu == null)
            throw new Exception("No existe un menú para ese usuario en esa fecha.");

        // Calcula IMC del usuario
        var imc = CalcularIMC(usuario);

        // Obtiene la categoría del IMC
        var categoriaImc = ObtenerCategoriaIMC(imc);

        // Calcula calorías objetivo según peso, actividad y objetivo
        var caloriasObjetivo = CalcularCaloriasObjetivo(usuario);

        // Calcula proteínas objetivo
        var proteinasObjetivo = Math.Round(usuario.Peso * 2.0m, 2);

        // Calcula grasas objetivo
        var grasasObjetivo = Math.Round(usuario.Peso * 0.8m, 2);

        // Calcula calorías restantes para asignar a carbohidratos
        var caloriasRestantesParaCarbos =
            caloriasObjetivo - (proteinasObjetivo * 4m) - (grasasObjetivo * 9m);

        // Calcula carbohidratos objetivo
        var carbohidratosObjetivo = Math.Round(caloriasRestantesParaCarbos / 4m, 2);

        // Si los carbohidratos salen negativos, los deja en 0
        if (carbohidratosObjetivo < 0)
            carbohidratosObjetivo = 0;

        // Devuelve el objeto resumen con todos los datos calculados
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

    // =====================================================
    // MÉTODO CALCULAR IMC
    // =====================================================
    // Calcula el índice de masa corporal del usuario
    private decimal CalcularIMC(Usuario usuario)
    {
        // Si la altura no es válida, retorna 0
        if (usuario.Altura <= 0)
            return 0;

        // Fórmula del IMC = peso / altura²
        return Math.Round(usuario.Peso / (usuario.Altura * usuario.Altura), 2);
    }

    // =====================================================
    // MÉTODO OBTENER CATEGORÍA IMC
    // =====================================================
    // Clasifica el IMC en una categoría nutricional
    private string ObtenerCategoriaIMC(decimal imc)
    {
        if (imc < 18.5m) return "Bajo peso";
        if (imc < 25m) return "Normal";
        if (imc < 30m) return "Sobrepeso";
        return "Obesidad";
    }

    // =====================================================
    // MÉTODO CALCULAR CALORÍAS OBJETIVO
    // =====================================================
    // Calcula las calorías recomendadas según peso, actividad y objetivo
    private decimal CalcularCaloriasObjetivo(Usuario usuario)
    {
        // Cálculo base de calorías
        decimal baseCalorias = usuario.Peso * 24m;

        // Normaliza actividad y objetivo a minúsculas
        var actividad = (usuario.Actividad ?? "").Trim().ToLowerInvariant();
        var objetivo = (usuario.Objetivo ?? "").Trim().ToLowerInvariant();

        // Define el factor según nivel de actividad
        decimal factorActividad = actividad switch
        {
            "sedentaria" => 1.2m,
            "ligera" => 1.375m,
            "moderada" => 1.55m,
            "alta" => 1.725m,
            _ => 1.2m
        };

        // Calcula calorías de mantenimiento
        decimal mantenimiento = baseCalorias * factorActividad;

        // Ajusta calorías según objetivo del usuario
        decimal resultado = objetivo switch
        {
            "bajar peso" => mantenimiento - 300m,
            "ganar masa" => mantenimiento + 300m,
            _ => mantenimiento
        };

        // Retorna el valor redondeado
        return Math.Round(resultado, 2);
    }

    // =====================================================
    // MÉTODO OBTENER ESTADO
    // =====================================================
    // Compara calorías consumidas contra el objetivo
    private string ObtenerEstado(decimal consumidas, decimal objetivo)
    {
        // Si consumió menos del 90% del objetivo
        if (consumidas < objetivo * 0.9m)
            return "Por debajo del objetivo";

        // Si consumió más del 110% del objetivo
        if (consumidas > objetivo * 1.1m)
            return "Por encima del objetivo";

        // Si está dentro del rango esperado
        return "Dentro del rango recomendado";
    }

    // =====================================================
    // MÉTODO OBTENER RECOMENDACIÓN
    // =====================================================
    // Genera una recomendación según el consumo del usuario
    private string ObtenerRecomendacion(decimal consumidas, decimal objetivo, string objetivoUsuario)
    {
        // Recomienda aumentar si consumió poco
        if (consumidas < objetivo * 0.9m)
            return "Se recomienda aumentar la ingesta diaria.";

        // Recomienda reducir si consumió demasiado
        if (consumidas > objetivo * 1.1m)
            return "Se recomienda reducir la ingesta diaria.";

        // Si está en rango, confirma alineación con el objetivo
        return $"El consumo está alineado con el objetivo: {objetivoUsuario}.";
    }
}