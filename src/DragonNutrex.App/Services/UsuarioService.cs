using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;
using DragonNutrex.App.Repositories; // Necesario para acceder al método específico de Redis

namespace DragonNutrex.App.Services;

/// <summary>
/// Servicio encargado de gestionar todas las operaciones relacionadas con usuarios.
/// Proporciona métodos síncronos y asincrones para crear, obtener, actualizar y eliminar usuarios,
/// además de facilitar búsquedas por correo electrónico optimizadas mediante Redis.
/// Actúa como capa de abstracción entre los controladores y el repositorio de datos.
/// </summary>
public class UsuarioService
{
    private readonly IRepository<Usuario> _usuarioRepository;

    /// <summary>
    /// Inicializa una nueva instancia del servicio UsuarioService.
    /// Realiza la inyección de dependencia del repositorio genérico de usuarios.
    /// </summary>
    /// <param name="usuarioRepository">Implementación del repositorio genérico que gestiona las operaciones de persistencia de usuarios.</param>
    public UsuarioService(IRepository<Usuario> usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    /// <summary>
    /// Obtiene un usuario de forma asincrónica mediante su dirección de correo electrónico.
    /// Implementa un patrón puente que detecta si el repositorio es una instancia de UsuarioRedisRepository
    /// para ejecutar búsquedas optimizadas contra Redis. Si la implementación no es Redis,
    /// retorna null indicando que la operación no es soportada.
    /// </summary>
    /// <param name="correo">Dirección de correo electrónico del usuario a buscar.</param>
    /// <returns>Retorna una tarea asincrónica que contiene el usuario encontrado o null si no existe o el repositorio no soporta esta operación.</returns>
    public async Task<Usuario?> GetByCorreoAsync(string correo)
    {
        // Como la interfaz es genérica, la casteamos a UsuarioRedisRepository 
        // para destrabar el método especial ultra rápido que construimos hoy.
        if (_usuarioRepository is UsuarioRedisRepository redisRepo)
        {
            return await redisRepo.GetByCorreoAsync(correo);
        }
        
        return null;
    }

    /// <summary>
    /// Obtiene de forma asincrónica la lista completa de todos los usuarios registrados en el sistema.
    /// Ejecuta una consulta optimizada que retorna todos los registros disponibles en el repositorio.
    /// Esta operación es recomendada para operaciones que requieren acceso a múltiples usuarios sin criterios específicos de filtrado.
    /// </summary>
    /// <returns>Retorna una tarea asincrónica que contiene una colección con todos los usuarios del sistema.</returns>
    public async Task<List<Usuario>> ObtenerUsuariosAsync()
    {
        return await _usuarioRepository.GetAllAsync();
    }

    /// <summary>
    /// Obtiene de forma sincrónica la lista completa de todos los usuarios registrados en el sistema.
    /// Implementa una consulta bloqueante que espera la respuesta del repositorio de datos.
    /// Se recomienda utilizar este método solo en contextos donde las operaciones asincrónicas no son disponibles.
    /// </summary>
    /// <returns>Retorna una colección con todos los usuarios del sistema.</returns>
    public List<Usuario> ObtenerUsuarios()
    {
        return _usuarioRepository.GetAll();
    }

    /// <summary>
    /// Crea un nuevo registro de usuario en el sistema.
    /// Persiste la información del usuario proporcionado en el repositorio de datos.
    /// Valida que el objeto usuario contenga los datos necesarios antes de proceder con la creación.
    /// </summary>
    /// <param name="usuario">Objeto usuario que contiene los datos a registrar en el sistema.</param>
    public void CrearUsuario(Usuario usuario)
    {
        _usuarioRepository.Create(usuario);
    }

    /// <summary>
    /// Actualiza los datos de un usuario existente en el sistema.
    /// Reemplaza la información anterior del usuario con los datos proporcionados en el objeto actualizado.
    /// La operación requiere que el usuario identificado exista previamente en la base de datos.
    /// </summary>
    /// <param name="usuario">Objeto usuario que contiene los datos actualizados a persistir.</param>
    public void ActualizarUsuario(Usuario usuario)
    {
        _usuarioRepository.Update(usuario);
    }

    /// <summary>
    /// Elimina un registro de usuario del sistema por su identificador único.
    /// Realiza la supresión definitiva del usuario especificado del repositorio de datos.
    /// La operación es irreversible y afecta todos los datos asociados al identificador proporcionado.
    /// </summary>
    /// <param name="id">Identificador único (GUID) del usuario a eliminar.</param>
    public void EliminarUsuario(Guid id)
    {
        _usuarioRepository.Delete(id);
    }

    /// <summary>
    /// Obtiene los datos de un usuario específico mediante su identificador único.
    /// Realiza una búsqueda directa por clave primaria en el repositorio de datos.
    /// Retorna el usuario si existe o null si no se encuentra registro con el identificador proporcionado.
    /// </summary>
    /// <param name="id">Identificador único (GUID) del usuario a recuperar.</param>
    /// <returns>Retorna el usuario encontrado o null si el usuario no existe en el sistema.</returns>
    public Usuario? ObtenerUsuario(Guid id)
    {
        return _usuarioRepository.GetById(id);
    }
}