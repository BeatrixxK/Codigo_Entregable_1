/* =====================================================
   CONTROLADOR DE USUARIO
   Conecta la capa de servicio con la UI
   ✔ conecta servicio con UI
   ===================================================== */

// Modelo Usuario
using DragonNutrex.App.Models;

// Servicio de lógica de negocio
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

// =====================================================
// CLASE USUARIO CONTROLLER
// =====================================================
// Actúa como intermediario entre la UI y el servicio
public class UsuarioController
{
    // Servicio de usuarios
    private readonly UsuarioService _service;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe el servicio para usar la lógica de negocio
    public UsuarioController(UsuarioService service)
    {
        _service = service;
    }

    // =====================================================
    // MÉTODO CREAR USUARIO
    // =====================================================
    // Envía el usuario al servicio para validación y guardado
    public void CrearUsuario(Usuario usuario)
    {
        _service.CrearUsuario(usuario);
    }

    // =====================================================
    // MÉTODO ACTUALIZAR USUARIO
    // =====================================================
    // Envía el usuario al servicio para actualización
    public void ActualizarUsuario(Usuario usuario)
    {
        _service.ActualizarUsuario(usuario);
    }

    // =====================================================
    // MÉTODO ELIMINAR USUARIO
    // =====================================================
    // Envía el ID al servicio para eliminar el usuario
    public void EliminarUsuario(Guid id)
    {
        _service.EliminarUsuario(id);
    }

    // =====================================================
    // MÉTODO OBTENER USUARIOS
    // =====================================================
    // Solicita al servicio la lista de usuarios
    public List<Usuario> ObtenerUsuarios()
    {
        return _service.ObtenerUsuarios();
    }

    // =====================================================
    // MÉTODO OBTENER USUARIO
    // =====================================================
    // Solicita al servicio un usuario específico por ID
    public Usuario? ObtenerUsuario(Guid id)
    {
        return _service.ObtenerUsuario(id);
    }
}