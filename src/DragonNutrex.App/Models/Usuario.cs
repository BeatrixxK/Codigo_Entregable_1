using System;

namespace DragonNutrex.App.Models;

public class Usuario
{
    // El ID que en tu Redis es 10, 11, 26, etc. 
    // Lo manejamos como Guid por compatibilidad con el resto de tu App
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Este es el campo "Usuario" en Redis (el email).
    /// Lo llamamos Correo en C# para no confundirlo con el nombre de la clase.
    /// </summary>
    public string Correo { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    // Datos nutricionales básicos
    public decimal Peso { get; set; }
    public decimal Altura { get; set; }

    // Preferencias del sistema
    public string Actividad { get; set; } = string.Empty; // Ej: Ligera
    public string Objetivo { get; set; } = string.Empty;  // Ej: Bajar peso
    public string TipoDieta { get; set; } = string.Empty; // Ej: Vegana

    // Estado de la cuenta
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Propiedad calculada para facilitar verificaciones en el frontend
    /// </summary>
    public bool EsAdmin => Correo.ToLower() == "admin@admin.com" || Nombre.ToLower() == "admin";
}