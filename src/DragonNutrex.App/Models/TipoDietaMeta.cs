// =====================================================
// MODELO TIPO DIETA META
// =====================================================
// Representa los valores objetivo nutricionales de una dieta
namespace DragonNutrex.App.Models;

/// <summary>
/// Define el modelo de metadatos para una dieta, encapsulando los valores objetivo nutricionales 
/// que se deben alcanzar en el consumo diario de un usuario.
/// 
/// Esta clase almacena los parámetros de configuración nutritiva de diferentes tipos de dietas
/// (Cetogénica, Vegana, Balanceada, etc.) y proporciona la estructura base para el seguimiento
/// y validación de objetivos dietéticos.
/// </summary>
public class TipoDietaMeta
{
    /// <summary>
    /// Obtiene o establece el nombre descriptivo de la dieta.
    /// 
    /// Almacena la identificación de la categoría dietética (ej: Keto, Vegana, Balanceada, Mediterránea).
    /// Se inicializa con una cadena vacía por defecto.
    /// </summary>
    public string Nombre { get; set; } = "";

    /// <summary>
    /// Obtiene o establece el valor objetivo de calorías diarias.
    /// 
    /// Define la cantidad total de kilocalorías (kcal) que debe consumir un usuario en un día
    /// para mantener el régimen dietético establecido. Este valor sirve como referencia primaria
    /// para el cálculo y validación del consumo energético.
    /// </summary>
    public decimal CaloriasObjetivo { get; set; }

    /// <summary>
    /// Obtiene o establece el valor objetivo de proteínas diarias en gramos.
    /// 
    /// Especifica la cantidad de proteínas (en gramos) que debe ingerir un usuario diariamente
    /// para cumplir con los requerimientos nutricionales del tipo de dieta. Las proteínas son
    /// macronutrientes esenciales para la síntesis muscular y reparación tisular.
    /// </summary>
    public decimal ProteinasObjetivo { get; set; }

    /// <summary>
    /// Obtiene o establece el valor objetivo de carbohidratos diarios en gramos.
    /// 
    /// Define la cantidad de carbohidratos (en gramos) que constituye el objetivo de consumo
    /// diario según el tipo de dieta. Los carbohidratos son la principal fuente de energía
    /// y su cantidad varía significativamente según el régimen (menor en dietas cetogénicas).
    /// </summary>
    public decimal CarbohidratosObjetivo { get; set; }

    /// <summary>
    /// Obtiene o establece el valor objetivo de grasas diarias en gramos.
    /// 
    /// Especifica la cantidad de grasas (en gramos) permitida en el consumo diario del usuario.
    /// Las grasas son macronutrientes necesarios para la absorción de vitaminas liposolubles
    /// y la salud celular, con proporciones variables según el tipo de dieta.
    /// </summary>
    public decimal GrasasObjetivo { get; set; }
}