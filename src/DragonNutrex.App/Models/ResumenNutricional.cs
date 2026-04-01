namespace DragonNutrex.App.Models;

public class ResumenNutricional
{
    public string NombreUsuario { get; set; } = "";
    public DateTime Fecha { get; set; }

    public decimal Imc { get; set; }
    public string CategoriaImc { get; set; } = "";

    public decimal CaloriasConsumidas { get; set; }
    public decimal ProteinasConsumidas { get; set; }
    public decimal CarbohidratosConsumidos { get; set; }
    public decimal GrasasConsumidas { get; set; }

    public decimal CaloriasObjetivo { get; set; }
    public decimal ProteinasObjetivo { get; set; }
    public decimal CarbohidratosObjetivo { get; set; }
    public decimal GrasasObjetivo { get; set; }

    public string EstadoCalorico { get; set; } = "";
    public string Recomendacion { get; set; } = "";
}