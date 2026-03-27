namespace DragonNutrex.App.Models;

public class TipoDietaMeta
{
    public string Nombre { get; set; } = "";
    public decimal CaloriasObjetivo { get; set; }
    public decimal ProteinasObjetivo { get; set; }
    public decimal CarbohidratosObjetivo { get; set; }
    public decimal GrasasObjetivo { get; set; }
}