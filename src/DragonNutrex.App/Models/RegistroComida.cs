using System;

namespace DragonNutrex.App.Models;

/// <summary>
/// Representa un elemento de registro de comida consumida en un menú.
/// </summary>
/// <remarks>
/// Contiene información de identidad del registro, datos del producto y valores nutricionales calculados.
/// </remarks>
public class RegistroComida
{
    // Identificador único del registro
    public Guid Id { get; set; } = Guid.NewGuid();

    // ID del producto asociado
    public Guid ProductoId { get; set; }

    // Nombre del producto (para mostrar en UI)
    public string NombreProducto { get; set; } = "";

    // Cantidad consumida del producto
    public decimal Cantidad { get; set; } = 1m;

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