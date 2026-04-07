// =====================================================
// MODELO RESUMEN ESTADÍSTICA NUTRICIONAL
// =====================================================
// Representa el análisis nutricional de un usuario
// en un rango de fechas (no solo un día)
// Incluye totales, promedios, objetivos y diferencias
namespace DragonNutrex.App.Models;

public class ResumenEstadisticaNutricional
{
    // Nombre del usuario analizado
    public string NombreUsuario { get; set; } = "";

    // Tipo de dieta seleccionada para comparación
    public string TipoDieta { get; set; } = "";

    // Fecha inicial del análisis
    public DateTime FechaInicio { get; set; }

    // Fecha final del análisis
    public DateTime FechaFin { get; set; }

    // Cantidad de menús evaluados en el rango
    public int CantidadMenus { get; set; }

    // =====================================================
    // DATOS DEL IMC
    // =====================================================

    // Índice de masa corporal del usuario
    public decimal Imc { get; set; }

    // Categoría del IMC (Normal, Sobrepeso, etc.)
    public string CategoriaImc { get; set; } = "";

    // =====================================================
    // CONSUMO TOTAL EN EL RANGO
    // =====================================================

    // Calorías totales consumidas
    public decimal CaloriasTotalesConsumidas { get; set; }

    // Proteínas totales consumidas
    public decimal ProteinasTotalesConsumidas { get; set; }

    // Carbohidratos totales consumidos
    public decimal CarbohidratosTotalesConsumidos { get; set; }

    // Grasas totales consumidas
    public decimal GrasasTotalesConsumidas { get; set; }

    // =====================================================
    // PROMEDIOS DIARIOS
    // =====================================================

    // Promedio de calorías por día
    public decimal CaloriasPromedio { get; set; }

    // Promedio de proteínas por día
    public decimal ProteinasPromedio { get; set; }

    // Promedio de carbohidratos por día
    public decimal CarbohidratosPromedio { get; set; }

    // Promedio de grasas por día
    public decimal GrasasPromedio { get; set; }

    // =====================================================
    // OBJETIVOS SEGÚN DIETA
    // =====================================================

    // Calorías objetivo de la dieta
    public decimal CaloriasObjetivoDieta { get; set; }

    // Proteínas objetivo de la dieta
    public decimal ProteinasObjetivoDieta { get; set; }

    // Carbohidratos objetivo de la dieta
    public decimal CarbohidratosObjetivoDieta { get; set; }

    // Grasas objetivo de la dieta
    public decimal GrasasObjetivoDieta { get; set; }

    // =====================================================
    // DIFERENCIAS (PROMEDIO VS OBJETIVO)
    // =====================================================

    // Diferencia de calorías
    public decimal DiferenciaCalorias { get; set; }

    // Diferencia de proteínas
    public decimal DiferenciaProteinas { get; set; }

    // Diferencia de carbohidratos
    public decimal DiferenciaCarbohidratos { get; set; }

    // Diferencia de grasas
    public decimal DiferenciaGrasas { get; set; }

    // =====================================================
    // RESULTADO FINAL
    // =====================================================

    // Estado calórico (déficit, exceso o balance)
    public string EstadoCalorico { get; set; } = "";

    // Recomendación final según el análisis
    public string Recomendacion { get; set; } = "";
}