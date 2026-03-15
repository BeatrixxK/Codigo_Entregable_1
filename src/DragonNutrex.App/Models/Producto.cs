namespace DragonNutrex.App.Models;

public class Producto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nombre { get; set; } = "";

    public decimal Calorias { get; set; }

    public decimal Proteinas { get; set; }

    public decimal Carbohidratos { get; set; }

    public decimal Grasas { get; set; }
}