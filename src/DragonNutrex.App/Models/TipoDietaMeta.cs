// =====================================================
// MODELO TIPO DIETA META
// =====================================================
// Representa los valores objetivo nutricionales de una dieta
namespace DragonNutrex.App.Models;

public class TipoDietaMeta
{
    // Nombre de la dieta (ej: Keto, Vegana, Balanceada)
    public string Nombre { get; set; } = "";

    // Calorías objetivo diarias
    public decimal CaloriasObjetivo { get; set; }

    // Proteínas objetivo diarias (en gramos)
    public decimal ProteinasObjetivo { get; set; }

    // Carbohidratos objetivo diarios (en gramos)
    public decimal CarbohidratosObjetivo { get; set; }

    // Grasas objetivo diarias (en gramos)
    public decimal GrasasObjetivo { get; set; }
}