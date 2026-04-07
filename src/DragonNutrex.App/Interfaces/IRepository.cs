/* =====================================================
   INTERFAZ REPOSITORY
   Define las operaciones básicas de acceso a datos (CRUD)
   ✔ Create
   ✔ Update
   ✔ Delete
   ✔ GetAll
   ✔ GetById
   ===================================================== */

namespace DragonNutrex.App.Interfaces;

// =====================================================
// INTERFAZ GENERICA IREPOSITORY
// =====================================================
// Define los métodos que cualquier repositorio debe implementar
public interface IRepository<T>
{
    // =====================================================
    // MÉTODO CREATE
    // =====================================================
    // Agrega una nueva entidad
    void Create(T entity);

    // =====================================================
    // MÉTODO UPDATE
    // =====================================================
    // Actualiza una entidad existente
    void Update(T entity);

    // =====================================================
    // MÉTODO DELETE
    // =====================================================
    // Elimina una entidad por su ID
    void Delete(Guid id);

    // =====================================================
    // MÉTODO GET ALL
    // =====================================================
    // Devuelve todas las entidades
    List<T> GetAll();

    // =====================================================
    // MÉTODO GET BY ID
    // =====================================================
    // Busca una entidad por su ID
    T? GetById(Guid id);
}