namespace DragonNutrex.App.Models;

public class Menu
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nombre { get; set; } = "";

    public string TipoDieta { get; set; } = "";

    public decimal CaloriasTotales { get; set; }
}