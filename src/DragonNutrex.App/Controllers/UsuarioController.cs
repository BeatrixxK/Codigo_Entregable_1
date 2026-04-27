using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonNutrex.App.Models;
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

/// <summary>
/// Controlador principal encargado de intermediar entre las vistas de Blazor y los servicios de base de datos.
/// Gestiona la lógica de autenticación, encriptación de credenciales y validación de la integridad de los usuarios.
/// </summary>
public class UsuarioController
{
    private readonly UsuarioService _usuarioService;

    public UsuarioController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Recupera todos los usuarios activos de la base de datos Redis.
    /// Implementa un filtro de seguridad relajado para descartar registros corruptos,
    /// permitiendo la visualización de usuarios antiguos que no poseen correo registrado.
    /// </summary>
    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        var usuariosCrudos = await _usuarioService.ObtenerUsuariosAsync();

        // 🔥 FILTRO RELAJADO: Quitamos la validación del correo para que aparezcan los antiguos.
        // Solo exigimos que el objeto exista, tenga un Nombre y un ID válido.
        var usuariosSanos = usuariosCrudos?
            .Where(u => u != null 
                     && !string.IsNullOrWhiteSpace(u.Nombre) 
                     && u.Id != Guid.Empty)
            .ToList();

        return usuariosSanos ?? new List<Usuario>();
    }

    /// <summary>
    /// Busca un usuario específico utilizando su correo electrónico como criterio principal.
    /// </summary>
    public async Task<Usuario?> ObtenerUsuarioPorCorreoAsync(string correo)
    {
        if (string.IsNullOrWhiteSpace(correo)) return null;

        var todos = await ObtenerTodosAsync();
        return todos.FirstOrDefault(u => u.Correo != null && u.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Valida las credenciales de un usuario comparando la contraseña en texto plano 
    /// contra el hash BCrypt almacenado, protegiendo así el acceso al sistema.
    /// </summary>
    public async Task<Usuario?> AutenticarUsuarioAsync(string correo, string passwordPlano)
    {
        var usuario = await ObtenerUsuarioPorCorreoAsync(correo);
        
        if (usuario == null || string.IsNullOrWhiteSpace(usuario.Password)) 
            return null;

        // Compara de forma segura el texto ingresado con la encriptación
        bool esValido = BCrypt.Net.BCrypt.Verify(passwordPlano, usuario.Password);

        return esValido ? usuario : null;
    }

    /// <summary>
    /// Persiste un nuevo perfil de usuario en la base de datos.
    /// Se encarga de asignar un identificador único (GUID) y de aplicar el algoritmo de hash 
    /// a la contraseña antes de su almacenamiento.
    /// </summary>
    public void CrearUsuario(Usuario usuario)
    {
        if (usuario == null) throw new ArgumentNullException(nameof(usuario));

        if (usuario.Id == Guid.Empty)
        {
            usuario.Id = Guid.NewGuid();
        }

        // Si la contraseña existe y no está encriptada aún (no empieza con el prefijo de BCrypt)
        if (!string.IsNullOrWhiteSpace(usuario.Password) && !usuario.Password.StartsWith("$2a$"))
        {
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
        }

        _usuarioService.CrearUsuario(usuario);
    }

    /// <summary>
    /// Actualiza la información de un perfil existente. Verifica si la contraseña fue modificada
    /// o reseteada para aplicar la encriptación correspondiente antes de la persistencia.
    /// </summary>
    public void ActualizarUsuario(Usuario usuario)
    {
        if (usuario == null) throw new ArgumentNullException(nameof(usuario));

        // Encripta la contraseña si detecta que fue cambiada (por ejemplo, en un reseteo de admin a "Dragon123")
        if (!string.IsNullOrWhiteSpace(usuario.Password) && !usuario.Password.StartsWith("$2a$"))
        {
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
        }

        _usuarioService.ActualizarUsuario(usuario);
    }

    /// <summary>
    /// Remueve permanentemente un registro de usuario de la base de datos Redis.
    /// </summary>
    public void EliminarUsuario(Guid id)
    {
        if (id == Guid.Empty) return;
        _usuarioService.EliminarUsuario(id);
    }
}