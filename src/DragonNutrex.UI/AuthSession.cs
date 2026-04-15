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
    // ✨ CORRECCIÓN: Se quitó 'private' para permitir que MainWindow actualice la sesión
    public static bool EsAdmin { get; set; }

    public static Guid UsuarioId { get; set; }

    public static string NombreUsuario { get; set; } = string.Empty;

    // =====================================================
    // MÉTODO INICIAR ADMIN
    // =====================================================
    // Configura la sesión como administrador
    public static void IniciarAdmin()
    {
        EsAdmin = true; 
        UsuarioId = Guid.Empty; 
        NombreUsuario = "Administrador"; 
    }

    // =====================================================
    // MÉTODO INICIAR USUARIO
    // =====================================================
    // Configura la sesión con un usuario normal
    public static void IniciarUsuario(Usuario usuario)
    {
        EsAdmin = false; 
        UsuarioId = usuario.Id; 
        NombreUsuario = usuario.Nombre; 
    }

    // =====================================================
    // MÉTODO CERRAR
    // =====================================================
    // Limpia la sesión actual (logout)
    public static void Cerrar()
    {
        EsAdmin = false; 
        UsuarioId = Guid.Empty; 
        NombreUsuario = string.Empty; 
    }

    // =====================================================
    // MÉTODO CERRAR SESIÓN
    // =====================================================
    // Hace lo mismo que Cerrar(): reinicia los datos de sesión
    public static void CerrarSesion()
    {
        UsuarioId = Guid.Empty; 
        NombreUsuario = string.Empty; 
        EsAdmin = false; 
    }
}