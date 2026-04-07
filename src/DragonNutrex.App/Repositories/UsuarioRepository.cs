// =====================================================
// IMPORTACIONES
// =====================================================

// Interfaz del repositorio genérico
using DragonNutrex.App.Interfaces;

// Modelo Usuario
using DragonNutrex.App.Models;

// Utilidad para manejar archivos JSON
using DragonNutrex.App.Utils;

namespace DragonNutrex.App.Repositories;

// =====================================================
// CLASE USUARIO REPOSITORY
// =====================================================
// Implementa el acceso a datos para usuarios (persistencia en JSON)
// ✔ CRUD completo
// ✔ lectura/escritura en archivo
public class UsuarioRepository : IRepository<Usuario>
{
    // Ruta del archivo JSON donde se guardan los usuarios
    private readonly string _filePath = Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "DragonNutrex.App",
            "Data",
            "usuarios.json"
        )
    );

    // =====================================================
    // MÉTODO CREATE
    // =====================================================
    // Agrega un nuevo usuario al archivo
    public void Create(Usuario entity)
    {
        // Obtiene la lista actual
        var usuarios = GetAll();

        // Agrega el nuevo usuario
        usuarios.Add(entity);

        // Guarda los cambios en JSON
        FileStorage.WriteList(_filePath, usuarios);
    }

    // =====================================================
    // MÉTODO UPDATE
    // =====================================================
    // Actualiza un usuario existente
    public void Update(Usuario entity)
    {
        // Obtiene la lista actual
        var usuarios = GetAll();

        // Busca la posición del usuario por ID
        var index = usuarios.FindIndex(u => u.Id == entity.Id);

        // Si no existe, lanza error
        if (index == -1)
            throw new Exception("Usuario no encontrado.");

        // Reemplaza el usuario en la lista
        usuarios[index] = entity;

        // Guarda cambios
        FileStorage.WriteList(_filePath, usuarios);
    }

    // =====================================================
    // MÉTODO DELETE
    // =====================================================
    // Elimina un usuario por ID
    public void Delete(Guid id)
    {
        // Obtiene la lista actual
        var usuarios = GetAll();

        // Busca el usuario
        var usuario = usuarios.FirstOrDefault(u => u.Id == id);

        // Si no existe, lanza error
        if (usuario == null)
            throw new Exception("Usuario no encontrado.");

        // Elimina el usuario
        usuarios.Remove(usuario);

        // Guarda cambios
        FileStorage.WriteList(_filePath, usuarios);
    }

    // =====================================================
    // MÉTODO GET ALL
    // =====================================================
    // Devuelve todos los usuarios
    public List<Usuario> GetAll()
    {
        return FileStorage.ReadList<Usuario>(_filePath);
    }

    // =====================================================
    // MÉTODO GET BY ID
    // =====================================================
    // Busca un usuario por su ID
    public Usuario? GetById(Guid id)
    {
        return GetAll().FirstOrDefault(u => u.Id == id);
    }
}