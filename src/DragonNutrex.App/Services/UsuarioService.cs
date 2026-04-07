/* =====================================================
   SERVICIO DE USUARIOS
   Maneja:
   • validaciones
   • lógica de negocio
   • conexión entre controlador y repositorio

   Métodos principales:
   - CrearUsuario()      -> valida y guarda
   - ActualizarUsuario() -> valida y actualiza
   - EliminarUsuario()   -> elimina
   - ObtenerUsuarios()   -> lista usuarios
   - ObtenerUsuario()    -> busca usuario

   ✔ validaciones
   ✔ conexión con repositorio
   ===================================================== */

// Interfaz genérica del repositorio
using DragonNutrex.App.Interfaces;

// Modelo Usuario
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

// =====================================================
// CLASE USUARIO SERVICE
// =====================================================
// Contiene la lógica de negocio para trabajar con usuarios
public class UsuarioService
{
    // Repositorio de usuarios
    private readonly IRepository<Usuario> _repository;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe el repositorio para poder acceder a los datos
    public UsuarioService(IRepository<Usuario> repository)
    {
        _repository = repository;
    }

    // =====================================================
    // MÉTODO CREAR USUARIO
    // =====================================================
    // Valida los datos del usuario y luego lo guarda
    public void CrearUsuario(Usuario usuario)
    {
        ValidarUsuario(usuario);
        _repository.Create(usuario);
    }

    // =====================================================
    // MÉTODO ACTUALIZAR USUARIO
    // =====================================================
    // Valida los datos del usuario y luego actualiza el registro
    public void ActualizarUsuario(Usuario usuario)
    {
        ValidarUsuario(usuario);
        _repository.Update(usuario);
    }

    // =====================================================
    // MÉTODO ELIMINAR USUARIO
    // =====================================================
    // Elimina un usuario por su ID
    public void EliminarUsuario(Guid id)
    {
        _repository.Delete(id);
    }

    // =====================================================
    // MÉTODO OBTENER USUARIOS
    // =====================================================
    // Devuelve la lista completa de usuarios
    public List<Usuario> ObtenerUsuarios()
    {
        return _repository.GetAll();
    }

    // =====================================================
    // MÉTODO OBTENER USUARIO
    // =====================================================
    // Busca un usuario por su ID
    public Usuario? ObtenerUsuario(Guid id)
    {
        return _repository.GetById(id);
    }

    // =====================================================
    // MÉTODO VALIDAR USUARIO
    // =====================================================
    // Verifica que los datos del usuario sean correctos
    private void ValidarUsuario(Usuario usuario)
    {
        // Valida que el nombre no esté vacío
        if (string.IsNullOrWhiteSpace(usuario.Nombre))
            throw new Exception("El nombre es obligatorio.");

        // Valida que el peso sea mayor que cero
        if (usuario.Peso <= 0)
            throw new Exception("El peso debe ser mayor que cero.");

        // Valida que la altura sea mayor que cero
        if (usuario.Altura <= 0)
            throw new Exception("La altura debe ser mayor que cero.");

        // Valida que la actividad esté definida
        if (string.IsNullOrWhiteSpace(usuario.Actividad))
            throw new Exception("La actividad es obligatoria.");

        // Valida que el objetivo esté definido
        if (string.IsNullOrWhiteSpace(usuario.Objetivo))
            throw new Exception("El objetivo es obligatorio.");

        // Valida que el tipo de dieta esté definido
        if (string.IsNullOrWhiteSpace(usuario.TipoDieta))
            throw new Exception("El tipo de dieta es obligatorio.");
    }
}