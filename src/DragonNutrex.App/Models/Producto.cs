// =====================================================
// MODELO PRODUCTO
// =====================================================
// Representa un alimento o producto nutricional del sistema
// Contiene su información nutricional básica
namespace DragonNutrex.App.Models;

public class Producto
{
    // Identificador único del producto
    public Guid Id { get; set; } = Guid.NewGuid();

    // Nombre del producto (ej: arroz, pollo, yogurt)
    public string Nombre { get; set; } = "";

    // Cantidad de calorías del producto
    public decimal Calorias { get; set; }

    // Cantidad de proteínas (en gramos)
    public decimal Proteinas { get; set; }

    // Cantidad de carbohidratos (en gramos)
    public decimal Carbohidratos { get; set; }

    // Cantidad de grasas (en gramos)
    public decimal Grasas { get; set; }
}