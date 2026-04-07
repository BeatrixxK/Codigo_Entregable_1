// =====================================================
// MODELO RESUMEN NUTRICIONAL
// =====================================================
// Representa el resultado del análisis nutricional diario
// de un usuario (comparación consumo vs objetivo)
namespace DragonNutrex.App.Models;

public class ResumenNutricional
{
    // Nombre del usuario
    public string NombreUsuario { get; set; } = "";

    // Fecha del análisis
    public DateTime Fecha { get; set; }

    // =====================================================
    // DATOS DEL IMC
    // =====================================================

    // Índice de Masa Corporal
    public decimal Imc { get; set; }

    // Categoría del IMC (Normal, Sobrepeso, etc.)
    public string CategoriaImc { get; set; } = "";

    // =====================================================
    // CONSUMO REAL DEL USUARIO
    // =====================================================

    // Calorías consumidas
    public decimal CaloriasConsumidas { get; set; }

    // Proteínas consumidas
    public decimal ProteinasConsumidas { get; set; }

    // Carbohidratos consumidos
    public decimal CarbohidratosConsumidos { get; set; }

    // Grasas consumidas
    public decimal GrasasConsumidas { get; set; }

    // =====================================================
    // OBJETIVOS NUTRICIONALES
    // =====================================================

    // Calorías objetivo
    public decimal CaloriasObjetivo { get; set; }

    // Proteínas objetivo
    public decimal ProteinasObjetivo { get; set; }

    // Carbohidratos objetivo
    public decimal CarbohidratosObjetivo { get; set; }

    // Grasas objetivo
    public decimal GrasasObjetivo { get; set; }

    // =====================================================
    // RESULTADO FINAL
    // =====================================================

    // Estado calórico (déficit, exceso o balance)
    public string EstadoCalorico { get; set; } = "";

    // Recomendación basada en el análisis
    public string Recomendacion { get; set; } = "";
}