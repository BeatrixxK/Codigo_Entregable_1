namespace DragonNutrex.App.Models;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = "";
    public decimal Peso { get; set; }
    public decimal Altura { get; set; }
    public string Actividad { get; set; } = "";
    public string Objetivo { get; set; } = "";
    public string TipoDieta { get; set; } = "";
    public string Password { get; set; } = "";
    public string Correo { get; set; } = "";
    
}