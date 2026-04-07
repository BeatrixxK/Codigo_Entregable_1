// =====================================================
// MODELO USUARIO
// =====================================================
// Representa la entidad Usuario dentro del sistema
// Contiene información personal y datos nutricionales
namespace DragonNutrex.App.Models;

public class Usuario
{
    // Identificador único del usuario
    public Guid Id { get; set; } = Guid.NewGuid();

    // Nombre del usuario
    public string Nombre { get; set; } = "";

    // Peso del usuario (en kg)
    public decimal Peso { get; set; }

    // Altura del usuario (en metros)
    public decimal Altura { get; set; }

    // Nivel de actividad física (ej: sedentaria, moderada, etc.)
    public string Actividad { get; set; } = "";

    // Objetivo del usuario (bajar peso, ganar masa, etc.)
    public string Objetivo { get; set; } = "";

    // Tipo de dieta seleccionada
    public string TipoDieta { get; set; } = "";

    // Contraseña del usuario (para login)
    public string Password { get; set; } = "";

    // Correo electrónico del usuario
    public string Correo { get; set; } = "";
}