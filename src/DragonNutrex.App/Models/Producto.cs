// =====================================================
// MODELO PRODUCTO
// =====================================================
// Representa un alimento o producto nutricional del sistema
// Contiene su información nutricional básica
namespace DragonNutrex.App.Models;

/// <summary>
/// Define la estructura de un producto nutricional en el sistema DragonNutrex.
/// Esta clase encapsula toda la información relevante de un alimento o producto,
/// incluyendo su identidad única y sus valores nutricionales para procesamiento,
/// almacenamiento y análisis dietético.
/// </summary>
public class Producto
{
    /// <summary>
    /// Obtiene o establece el identificador único del producto.
    /// Genera automáticamente un valor GUID único al crear una nueva instancia,
    /// garantizando la identificación unívoca en el sistema y evitando colisiones de datos.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Obtiene o establece el nombre descriptivo del producto.
    /// Almacena la denominación común del alimento (ejemplos: arroz, pollo, yogurt).
    /// Utilizado para la identificación legible en la interfaz de usuario y reportes.
    /// </summary>
    public string Nombre { get; set; } = "";

    /// <summary>
    /// Obtiene o establece la cantidad de calorías del producto.
    /// Registra el valor energético total expresado en kilocalorías (kcal).
    /// Fundamental para cálculos de ingesta calórica diaria y planificación nutricional.
    /// </summary>
    public decimal Calorias { get; set; }

    /// <summary>
    /// Obtiene o establece la cantidad de proteínas del producto.
    /// Almacena el contenido proteico expresado en gramos por porción estándar.
    /// Esencial para análisis de aporte proteico en planes dietéticos y control nutricional.
    /// </summary>
    public decimal Proteinas { get; set; }

    /// <summary>
    /// Obtiene o establece la cantidad de carbohidratos del producto.
    /// Registra el contenido de carbohidratos en gramos por porción.
    /// Necesario para monitoreo de ingesta de carbohidratos y cálculo de macronutrientes.
    /// </summary>
    public decimal Carbohidratos { get; set; }

    /// <summary>
    /// Obtiene o establece la cantidad de grasas del producto.
    /// Almacena el contenido total de lípidos expresado en gramos por porción.
    /// Crítico para evaluación del perfil lipídico y balance de macronutrientes en la dieta.
    /// </summary>
    public decimal Grasas { get; set; }
}