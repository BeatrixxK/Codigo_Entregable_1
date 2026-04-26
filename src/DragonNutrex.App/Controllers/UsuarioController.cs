using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using BCrypt.Net;

namespace DragonNutrex.App.Controllers;

/// <summary>
/// Controlador que gestiona las operaciones relacionadas con los usuarios, incluyendo autenticación, registro y mantenimiento de cuentas.
/// Este controlador actúa como intermediario entre la capa de presentación y el repositorio de datos, proporcionando métodos para manipular la información de usuarios de manera segura y eficiente.
/// </summary>
public class UsuarioController
{
    private readonly IRepository<Usuario> _usuarioRepository;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de usuarios con el repositorio especificado.
    /// Esta operación configura el controlador para utilizar el repositorio proporcionado, permitiendo el acceso a las operaciones de datos necesarias para la gestión de usuarios.
    /// </summary>
    /// <param name="usuarioRepository">El repositorio de usuarios utilizado para acceder a los datos.</param>
    public UsuarioController(IRepository<Usuario> usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    // =====================================================
    // OBTENER TODOS
    // =====================================================
    /// <summary>
    /// Obtiene una lista completa de todos los usuarios almacenados en el repositorio.
    /// Esta operación realiza una consulta asíncrona al repositorio para recuperar todos los registros de usuarios, lo que permite su uso en interfaces de administración o procesos que requieren acceso a la totalidad de los datos de usuarios.
    /// </summary>
    /// <returns>Una tarea que representa la operación asíncrona, devolviendo una lista de objetos Usuario.</returns>
    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await _usuarioRepository.GetAllAsync();
    }

    // =====================================================
    // OBTENER POR CORREO
    // =====================================================
    /// <summary>
    /// Busca y obtiene un usuario específico basado en su dirección de correo electrónico, ignorando mayúsculas y minúsculas.
    /// Esta función itera a través de todos los usuarios en el repositorio para encontrar una coincidencia exacta en el campo de correo, facilitando la autenticación y la gestión de cuentas de usuario sin distinción de caso.
    /// </summary>
    /// <param name="correo">La dirección de correo electrónico del usuario a buscar.</param>
    /// <returns>Una tarea que representa la operación asíncrona, devolviendo el objeto Usuario si se encuentra, o null en caso contrario.</returns>
    public async Task<Usuario?> ObtenerUsuarioPorCorreoAsync(string correo)
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        return usuarios.FirstOrDefault(u => u.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));
    }

    // =====================================================
    // AUTENTICAR (INICIAR SESIÓN CON SEGURIDAD)
    // =====================================================
    /// <summary>
    /// Autentica a un usuario verificando su correo electrónico y contraseña. Incluye compatibilidad hacia atrás para contraseñas en texto plano, pero prioriza la verificación con hashes BCrypt para seguridad.
    /// Esta operación primero busca el usuario por correo, luego verifica la contraseña. Si el hash de la contraseña almacenada no comienza con "$2", se asume que es una contraseña en texto plano (para retrocompatibilidad), de lo contrario, se utiliza BCrypt para la verificación segura.
    /// </summary>
    /// <param name="correo">La dirección de correo electrónico del usuario.</param>
    /// <param name="password">La contraseña proporcionada por el usuario.</param>
    /// <returns>Una tarea que representa la operación asíncrona, devolviendo el objeto Usuario si la autenticación es exitosa, o null en caso contrario.</returns>
    public async Task<Usuario?> AutenticarUsuarioAsync(string correo, string password)
    {
        var usuario = await ObtenerUsuarioPorCorreoAsync(correo);
        
        if (usuario == null) return null;

        // Si la contraseña es nula, rechazamos de inmediato
        if (string.IsNullOrWhiteSpace(usuario.Password)) return null;

        // MAGIA DE RETROCOMPATIBILIDAD: 
        // BCrypt siempre genera hashes que empiezan con "$2". 
        // Si no empieza con eso, significa que es un usuario viejo con contraseña en texto plano.
        if (!usuario.Password.StartsWith("$2"))
        {
            return usuario.Password == password ? usuario : null;
        }

        // VALIDACIÓN PROFESIONAL CON BCRYPT
        bool passwordCorrecta = BCrypt.Net.BCrypt.Verify(password, usuario.Password);
        return passwordCorrecta ? usuario : null;
    }

    // =====================================================
    // CREAR USUARIO (REGISTRO)
    // =====================================================
    /// <summary>
    /// Crea un nuevo usuario en el sistema. Si la contraseña no está encriptada (no comienza con "$2"), la encripta utilizando BCrypt antes de almacenarla.
    /// Esta operación asegura que todas las contraseñas se almacenen de forma segura, verificando primero si ya están hasheadas para evitar doble encriptación. Luego, invoca el método de creación del repositorio para persistir el nuevo usuario.
    /// </summary>
    /// <param name="nuevoUsuario">El objeto Usuario con los datos del nuevo usuario.</param>
    public void CrearUsuario(Usuario nuevoUsuario)
    {
        // Encriptamos la contraseña justo antes de guardarla en la base de datos
        if (!string.IsNullOrWhiteSpace(nuevoUsuario.Password) && !nuevoUsuario.Password.StartsWith("$2"))
        {
            nuevoUsuario.Password = BCrypt.Net.BCrypt.HashPassword(nuevoUsuario.Password);
        }
        
        _usuarioRepository.Create(nuevoUsuario);
    }

    // =====================================================
    // ACTUALIZAR USUARIO (CAMBIO DE CONTRASEÑA)
    // =====================================================
    /// <summary>
    /// Actualiza la información de un usuario existente. Si se proporciona una nueva contraseña en texto plano, la encripta utilizando BCrypt antes de la actualización.
    /// Esta operación detecta si la contraseña proporcionada está en texto plano (no comienza con "$2") y la encripta automáticamente para mantener la seguridad. Posteriormente, actualiza el registro del usuario en el repositorio.
    /// </summary>
    /// <param name="usuario">El objeto Usuario con los datos actualizados.</param>
    public void ActualizarUsuario(Usuario usuario)
    {
        // Si viene de la pantalla de recuperación, la contraseña vendrá en texto plano.
        // Aquí la detectamos y la encriptamos antes de mandarla a Redis.
        if (!string.IsNullOrWhiteSpace(usuario.Password) && !usuario.Password.StartsWith("$2"))
        {
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
        }

        _usuarioRepository.Update(usuario);
    }

    // =====================================================
    // ELIMINAR USUARIO
    // =====================================================
    /// <summary>
    /// Elimina un usuario del sistema basado en su identificador único.
    /// Esta operación remueve permanentemente el registro del usuario del repositorio, lo que puede afectar a la integridad de datos relacionados si no se maneja con cuidado. Se recomienda verificar dependencias antes de ejecutar esta acción.
    /// </summary>
    /// <param name="id">El identificador GUID del usuario a eliminar.</param>
    public void EliminarUsuario(Guid id)
    {
        _usuarioRepository.Delete(id);
    }
}