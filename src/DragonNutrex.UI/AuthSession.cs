using System;
using DragonNutrex.App.Models;

namespace DragonNutrex.UI;

public static class AuthSession
{
    public static bool EsAdmin { get; private set; }
    public static Guid UsuarioId { get; private set; }
    public static string NombreUsuario { get; private set; } = string.Empty;

    public static void IniciarAdmin()
    {
        EsAdmin = true;
        UsuarioId = Guid.Empty;
        NombreUsuario = "Administrador";
    }

    public static void IniciarUsuario(Usuario usuario)
    {
        EsAdmin = false;
        UsuarioId = usuario.Id;
        NombreUsuario = usuario.Nombre;
    }

    public static void Cerrar()
    {
        EsAdmin = false;
        UsuarioId = Guid.Empty;
        NombreUsuario = string.Empty;
    }

public static void CerrarSesion()
{
    UsuarioId = Guid.Empty;
    NombreUsuario = string.Empty;
    EsAdmin = false;
}

}