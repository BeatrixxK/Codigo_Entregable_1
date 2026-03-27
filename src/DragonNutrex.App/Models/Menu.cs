namespace DragonNutrex.App.Models;

public class Menu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public DateTime Fecha { get; set; }
    public List<RegistroComida> Registros { get; set; } = new();

    public decimal TotalCalorias { get; set; }
    public decimal TotalProteinas { get; set; }
    public decimal TotalCarbohidratos { get; set; }
    public decimal TotalGrasas { get; set; }
}