// =====================================================
// MODELO MENU
// =====================================================
// Representa un menú diario de un usuario
// Contiene los alimentos consumidos y sus totales nutricionales
namespace DragonNutrex.App.Models;

public class Menu
{
    // Identificador único del menú
    public Guid Id { get; set; } = Guid.NewGuid();

    // ID del usuario al que pertenece el menú
    public Guid UsuarioId { get; set; }

    // Fecha del menú (día de consumo)
    public DateTime Fecha { get; set; }

    // Lista de registros de comida (productos consumidos)
    public List<RegistroComida> Registros { get; set; } = new();

    // =====================================================
    // TOTALES NUTRICIONALES DEL MENÚ
    // =====================================================

    // Total de calorías consumidas en el día
    public decimal TotalCalorias { get; set; }

    // Total de proteínas consumidas
    public decimal TotalProteinas { get; set; }

    // Total de carbohidratos consumidos
    public decimal TotalCarbohidratos { get; set; }

    // Total de grasas consumidas
    public decimal TotalGrasas { get; set; }
}