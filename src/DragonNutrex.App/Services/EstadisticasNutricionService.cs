// =====================================================
// IMPORTACIONES
// =====================================================

// Interfaz del repositorio genérico
using DragonNutrex.App.Interfaces;

// Modelos de la aplicación
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

// =====================================================
// CLASE ESTADISTICAS NUTRICION SERVICE
// =====================================================
// Maneja la lógica de negocio para estadísticas nutricionales
// Calcula resúmenes por rango de fechas y por tipo de dieta
public class EstadisticasNutricionService
{
    // Repositorio de usuarios
    private readonly IRepository<Usuario> _usuarioRepository;

    // Repositorio de menús
    private readonly IRepository<Menu> _menuRepository;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe los repositorios necesarios para consultar datos
    public EstadisticasNutricionService(
        IRepository<Usuario> usuarioRepository,
        IRepository<Menu> menuRepository)
    {
        _usuarioRepository = usuarioRepository;
        _menuRepository = menuRepository;
    }

    // =====================================================
    // MÉTODO OBTENER DIETAS DISPONIBLES
    // =====================================================
    // Devuelve la lista de dietas con sus metas nutricionales base
    public List<TipoDietaMeta> ObtenerDietasDisponibles()
    {
        return new List<TipoDietaMeta>
        {
            new TipoDietaMeta
            {
                Nombre = "Balanceada",
                CaloriasObjetivo = 2200,
                ProteinasObjetivo = 140,
                CarbohidratosObjetivo = 250,
                GrasasObjetivo = 70
            },
            new TipoDietaMeta
            {
                Nombre = "Vegetariana",
                CaloriasObjetivo = 2100,
                ProteinasObjetivo = 120,
                CarbohidratosObjetivo = 260,
                GrasasObjetivo = 65
            },
            new TipoDietaMeta
            {
                Nombre = "Vegana",
                CaloriasObjetivo = 2050,
                ProteinasObjetivo = 110,
                CarbohidratosObjetivo = 280,
                GrasasObjetivo = 55
            },
            new TipoDietaMeta
            {
                Nombre = "Alta proteína",
                CaloriasObjetivo = 2300,
                ProteinasObjetivo = 180,
                CarbohidratosObjetivo = 180,
                GrasasObjetivo = 75
            },
            new TipoDietaMeta
            {
                Nombre = "Keto",
                CaloriasObjetivo = 2000,
                ProteinasObjetivo = 130,
                CarbohidratosObjetivo = 50,
                GrasasObjetivo = 140
            }
        };
    }

    // =====================================================
    // MÉTODO OBTENER RESUMEN
    // =====================================================
    // Genera un resumen estadístico nutricional para un usuario
    // en un rango de fechas y según un tipo de dieta
    public ResumenEstadisticaNutricional ObtenerResumen(
        Guid usuarioId,
        DateTime fechaInicio,
        DateTime fechaFin,
        string tipoDieta)
    {
        // Valida que la fecha inicial no sea mayor a la final
        if (fechaInicio.Date > fechaFin.Date)
            throw new Exception("La fecha inicio no puede ser mayor que la fecha fin.");

        // Busca el usuario por ID
        var usuario = _usuarioRepository.GetById(usuarioId);
        if (usuario == null)
            throw new Exception("Usuario no encontrado.");

        // Busca la dieta seleccionada dentro de las disponibles
        var dieta = ObtenerDietasDisponibles()
            .FirstOrDefault(d => d.Nombre.Equals(tipoDieta, StringComparison.OrdinalIgnoreCase));

        // Si no existe la dieta, lanza error
        if (dieta == null)
            throw new Exception("Tipo de dieta no válido.");

        // Obtiene todos los menús del usuario dentro del rango de fechas
        var menus = _menuRepository.GetAll()
            .Where(m => m.UsuarioId == usuarioId
                     && m.Fecha.Date >= fechaInicio.Date
                     && m.Fecha.Date <= fechaFin.Date)
            .OrderBy(m => m.Fecha)
            .ToList();

        // Si no hay menús en el rango, lanza error
        if (menus.Count == 0)
            throw new Exception("No hay registros de menús para ese usuario en ese rango de fechas.");

        // Cantidad total de menús encontrados
        var cantidadMenus = menus.Count;

        // =====================================================
        // CÁLCULO DE TOTALES CONSUMIDOS
        // =====================================================
        var caloriasTotales = menus.Sum(m => m.TotalCalorias);
        var proteinasTotales = menus.Sum(m => m.TotalProteinas);
        var carbohidratosTotales = menus.Sum(m => m.TotalCarbohidratos);
        var grasasTotales = menus.Sum(m => m.TotalGrasas);

        // =====================================================
        // CÁLCULO DE PROMEDIOS
        // =====================================================
        var caloriasPromedio = Math.Round(caloriasTotales / cantidadMenus, 2);
        var proteinasPromedio = Math.Round(proteinasTotales / cantidadMenus, 2);
        var carbohidratosPromedio = Math.Round(carbohidratosTotales / cantidadMenus, 2);
        var grasasPromedio = Math.Round(grasasTotales / cantidadMenus, 2);

        // =====================================================
        // CÁLCULO DE IMC
        // =====================================================
        var imc = CalcularImc(usuario);
        var categoriaImc = ObtenerCategoriaImc(imc);

        // =====================================================
        // CÁLCULO DE OBJETIVOS DE MACRONUTRIENTES
        // =====================================================
        var (proteinasObjetivo, carbohidratosObjetivo, grasasObjetivo) =
            CalcularDistribucionMacros(usuario.Peso, dieta.CaloriasObjetivo, dieta.Nombre);

        // =====================================================
        // CÁLCULO DE DIFERENCIAS ENTRE PROMEDIO Y OBJETIVO
        // =====================================================
        var diferenciaCalorias = Math.Round(caloriasPromedio - dieta.CaloriasObjetivo, 2);
        var diferenciaProteinas = Math.Round(proteinasPromedio - proteinasObjetivo, 2);
        var diferenciaCarbohidratos = Math.Round(carbohidratosPromedio - carbohidratosObjetivo, 2);
        var diferenciaGrasas = Math.Round(grasasPromedio - grasasObjetivo, 2);

        // Devuelve el resumen completo
        return new ResumenEstadisticaNutricional
        {
            NombreUsuario = usuario.Nombre,
            TipoDieta = dieta.Nombre,
            FechaInicio = fechaInicio.Date,
            FechaFin = fechaFin.Date,
            CantidadMenus = cantidadMenus,

            Imc = imc,
            CategoriaImc = categoriaImc,

            CaloriasTotalesConsumidas = caloriasTotales,
            ProteinasTotalesConsumidas = proteinasTotales,
            CarbohidratosTotalesConsumidos = carbohidratosTotales,
            GrasasTotalesConsumidas = grasasTotales,

            CaloriasPromedio = caloriasPromedio,
            ProteinasPromedio = proteinasPromedio,
            CarbohidratosPromedio = carbohidratosPromedio,
            GrasasPromedio = grasasPromedio,

            CaloriasObjetivoDieta = dieta.CaloriasObjetivo,
            ProteinasObjetivoDieta = proteinasObjetivo,
            CarbohidratosObjetivoDieta = carbohidratosObjetivo,
            GrasasObjetivoDieta = grasasObjetivo,

            DiferenciaCalorias = diferenciaCalorias,
            DiferenciaProteinas = diferenciaProteinas,
            DiferenciaCarbohidratos = diferenciaCarbohidratos,
            DiferenciaGrasas = diferenciaGrasas,

            EstadoCalorico = ObtenerEstadoCalorico(caloriasPromedio, dieta.CaloriasObjetivo),
            Recomendacion = ObtenerRecomendacion(caloriasPromedio, dieta.CaloriasObjetivo, dieta.Nombre)
        };
    }

    // =====================================================
    // MÉTODO CALCULAR IMC
    // =====================================================
    // Calcula el índice de masa corporal del usuario
    private decimal CalcularImc(Usuario usuario)
    {
        // Si la altura no es válida, retorna 0
        if (usuario.Altura <= 0)
            return 0;

        // Fórmula IMC = peso / altura²
        return Math.Round(usuario.Peso / (usuario.Altura * usuario.Altura), 2);
    }

    // =====================================================
    // MÉTODO OBTENER CATEGORÍA IMC
    // =====================================================
    // Clasifica el IMC del usuario
    private string ObtenerCategoriaImc(decimal imc)
    {
        if (imc < 18.5m) return "Bajo peso";
        if (imc < 25m) return "Normal";
        if (imc < 30m) return "Sobrepeso";
        return "Obesidad";
    }

    // =====================================================
    // MÉTODO CALCULAR DISTRIBUCIÓN DE MACROS
    // =====================================================
    // Calcula proteínas, carbohidratos y grasas objetivo
    // según el tipo de dieta y el peso del usuario
    private (decimal proteinas, decimal carbohidratos, decimal grasas) CalcularDistribucionMacros(
        decimal peso,
        decimal caloriasObjetivo,
        string tipoDieta)
    {
        decimal proteinas;
        decimal grasas;
        decimal carbohidratos;

        // Ajusta la distribución según la dieta
        switch (tipoDieta.ToLowerInvariant())
        {
            case "keto":
                proteinas = peso * 1.8m;
                grasas = (caloriasObjetivo * 0.7m) / 9m;
                carbohidratos = (caloriasObjetivo - (proteinas * 4m) - (grasas * 9m)) / 4m;
                break;

            case "alta proteína":
                proteinas = peso * 2.2m;
                grasas = peso * 0.8m;
                carbohidratos = (caloriasObjetivo - (proteinas * 4m) - (grasas * 9m)) / 4m;
                break;

            case "vegana":
            case "vegetariana":
                proteinas = peso * 1.6m;
                grasas = peso * 0.7m;
                carbohidratos = (caloriasObjetivo - (proteinas * 4m) - (grasas * 9m)) / 4m;
                break;

            default:
                proteinas = peso * 2.0m;
                grasas = peso * 0.8m;
                carbohidratos = (caloriasObjetivo - (proteinas * 4m) - (grasas * 9m)) / 4m;
                break;
        }

        // Evita carbohidratos negativos
        if (carbohidratos < 0)
            carbohidratos = 0;

        // Retorna los valores redondeados
        return (
            Math.Round(proteinas, 2),
            Math.Round(carbohidratos, 2),
            Math.Round(grasas, 2)
        );
    }

    // =====================================================
    // MÉTODO OBTENER ESTADO CALÓRICO
    // =====================================================
    // Compara el promedio consumido con el objetivo de calorías
    private string ObtenerEstadoCalorico(decimal caloriasPromedio, decimal caloriasObjetivo)
    {
        if (caloriasPromedio < caloriasObjetivo * 0.9m)
            return "Por debajo del objetivo";

        if (caloriasPromedio > caloriasObjetivo * 1.1m)
            return "Por encima del objetivo";

        return "Dentro del rango recomendado";
    }

    // =====================================================
    // MÉTODO OBTENER RECOMENDACIÓN
    // =====================================================
    // Genera una recomendación según el promedio calórico
    private string ObtenerRecomendacion(decimal caloriasPromedio, decimal caloriasObjetivo, string tipoDieta)
    {
        if (caloriasPromedio < caloriasObjetivo * 0.9m)
            return $"El promedio consumido está por debajo de la meta para la dieta {tipoDieta}. Se recomienda aumentar la ingesta diaria.";

        if (caloriasPromedio > caloriasObjetivo * 1.1m)
            return $"El promedio consumido está por encima de la meta para la dieta {tipoDieta}. Se recomienda reducir la ingesta diaria.";

        return $"El promedio consumido está alineado con la dieta {tipoDieta}.";
    }
}