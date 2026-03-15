namespace DragonNutrex.App.Models;

public class RegistroComida
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UsuarioId { get; set; }

    public Guid ProductoId { get; set; }

    public DateTime Fecha { get; set; }

    public decimal Cantidad { get; set; }
}