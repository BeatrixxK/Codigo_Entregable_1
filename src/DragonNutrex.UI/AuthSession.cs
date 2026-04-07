// =====================================================
// IMPORTACIONES
// =====================================================

// Librería base del sistema
using System;

// Modelo de Usuario (capa de dominio)
using DragonNutrex.App.Models;

// Namespace de la interfaz
namespace DragonNutrex.UI;

// =====================================================
// CLASE DE SESIÓN DE AUTENTICACIÓN
// =====================================================
// Maneja el estado global del usuario logueado en la aplicación
public static class AuthSession
{
    // Indica si el usuario actual es administrador
    public static bool EsAdmin { get; private set; }

    // Guarda el ID del usuario autenticado
    public static Guid UsuarioId { get; private set; }

    // Guarda el nombre del usuario autenticado
    public static string NombreUsuario { get; private set; } = string.Empty;

    // =====================================================
    // MÉTODO INICIAR ADMIN
    // =====================================================
    // Configura la sesión como administrador
    public static void IniciarAdmin()
    {
        EsAdmin = true; // Marca como admin
        UsuarioId = Guid.Empty; // No usa ID específico
        NombreUsuario = "Administrador"; // Nombre por defecto
    }

    // =====================================================
    // MÉTODO INICIAR USUARIO
    // =====================================================
    // Configura la sesión con un usuario normal
    public static void IniciarUsuario(Usuario usuario)
    {
        EsAdmin = false; // No es admin
        UsuarioId = usuario.Id; // Asigna ID del usuario
        NombreUsuario = usuario.Nombre; // Asigna nombre
    }

    // =====================================================
    // MÉTODO CERRAR
    // =====================================================
    // Limpia la sesión actual (logout)
    public static void Cerrar()
    {
        EsAdmin = false; // Quita privilegios de admin
        UsuarioId = Guid.Empty; // Resetea ID
        NombreUsuario = string.Empty; // Limpia nombre
    }

    // =====================================================
    // MÉTODO CERRAR SESIÓN
    // =====================================================
    // Hace lo mismo que Cerrar(): reinicia los datos de sesión
    public static void CerrarSesion()
    {
        UsuarioId = Guid.Empty; // Resetea ID
        NombreUsuario = string.Empty; // Limpia nombre
        EsAdmin = false; // Quita rol de admin
    }

}