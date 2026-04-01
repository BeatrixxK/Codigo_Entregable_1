namespace DragonNutrex.App.Models;

public class ResumenEstadisticaNutricional
{
    public string NombreUsuario { get; set; } = "";
    public string TipoDieta { get; set; } = "";

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    public int CantidadMenus { get; set; }

    public decimal Imc { get; set; }
    public string CategoriaImc { get; set; } = "";

    public decimal CaloriasTotalesConsumidas { get; set; }
    public decimal ProteinasTotalesConsumidas { get; set; }
    public decimal CarbohidratosTotalesConsumidos { get; set; }
    public decimal GrasasTotalesConsumidas { get; set; }

    public decimal CaloriasPromedio { get; set; }
    public decimal ProteinasPromedio { get; set; }
    public decimal CarbohidratosPromedio { get; set; }
    public decimal GrasasPromedio { get; set; }

    public decimal CaloriasObjetivoDieta { get; set; }
    public decimal ProteinasObjetivoDieta { get; set; }
    public decimal CarbohidratosObjetivoDieta { get; set; }
    public decimal GrasasObjetivoDieta { get; set; }

    public decimal DiferenciaCalorias { get; set; }
    public decimal DiferenciaProteinas { get; set; }
    public decimal DiferenciaCarbohidratos { get; set; }
    public decimal DiferenciaGrasas { get; set; }

    public string EstadoCalorico { get; set; } = "";
    public string Recomendacion { get; set; } = "";
}