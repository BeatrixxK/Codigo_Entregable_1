namespace DragonNutrex.App.Models;

public class RegistroComida
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductoId { get; set; }
    public string NombreProducto { get; set; } = "";
    public decimal Cantidad { get; set; }

    public decimal Calorias { get; set; }
    public decimal Proteinas { get; set; }
    public decimal Carbohidratos { get; set; }
    public decimal Grasas { get; set; }
}