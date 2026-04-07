// =====================================================
// MODELO REGISTRO COMIDA
// =====================================================
// Representa un producto consumido dentro de un menú
// Incluye cantidad y valores nutricionales calculados
namespace DragonNutrex.App.Models;

public class RegistroComida
{
    // Identificador único del registro
    public Guid Id { get; set; } = Guid.NewGuid();

    // ID del producto asociado
    public Guid ProductoId { get; set; }

    // Nombre del producto (para mostrar en UI)
    public string NombreProducto { get; set; } = "";

    // Cantidad consumida del producto
    public decimal Cantidad { get; set; }

    // =====================================================
    // VALORES NUTRICIONALES CALCULADOS
    // =====================================================

    // Calorías totales (producto * cantidad)
    public decimal Calorias { get; set; }

    // Proteínas totales
    public decimal Proteinas { get; set; }

    // Carbohidratos totales
    public decimal Carbohidratos { get; set; }

    // Grasas totales
    public decimal Grasas { get; set; }
}