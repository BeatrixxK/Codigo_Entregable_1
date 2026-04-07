// =====================================================
// IMPORTACIONES
// =====================================================

// Interfaz del repositorio genérico
using DragonNutrex.App.Interfaces;

// Modelo Menu
using DragonNutrex.App.Models;

// Utilidad para manejo de archivos JSON
using DragonNutrex.App.Utils;

namespace DragonNutrex.App.Repositories;

// =====================================================
// CLASE MENU REPOSITORY
// =====================================================
// Implementa el acceso a datos para menús usando archivos JSON
// ✔ CRUD completo
// ✔ persistencia en archivo
public class MenuRepository : IRepository<Menu>
{
    // Ruta del archivo JSON donde se almacenan los menús
    private readonly string _filePath = Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "DragonNutrex.App",
            "Data",
            "menus.json"
        )
    );

    // =====================================================
    // MÉTODO CREATE
    // =====================================================
    // Agrega un nuevo menú al archivo
    public void Create(Menu entity)
    {
        // Obtiene la lista actual
        var menus = GetAll();

        // Agrega el nuevo menú
        menus.Add(entity);

        // Guarda en JSON
        FileStorage.WriteList(_filePath, menus);
    }

    // =====================================================
    // MÉTODO UPDATE
    // =====================================================
    // Actualiza un menú existente
    public void Update(Menu entity)
    {
        // Obtiene la lista actual
        var menus = GetAll();

        // Busca el índice del menú por ID
        var index = menus.FindIndex(m => m.Id == entity.Id);

        // Si no existe, lanza error
        if (index == -1)
            throw new Exception("Menú no encontrado.");

        // Reemplaza el menú en la lista
        menus[index] = entity;

        // Guarda cambios
        FileStorage.WriteList(_filePath, menus);
    }

    // =====================================================
    // MÉTODO DELETE
    // =====================================================
    // Elimina un menú por ID
    public void Delete(Guid id)
    {
        // Obtiene la lista actual
        var menus = GetAll();

        // Busca el menú
        var menu = menus.FirstOrDefault(m => m.Id == id);

        // Si no existe, lanza error
        if (menu == null)
            throw new Exception("Menú no encontrado.");

        // Elimina el menú
        menus.Remove(menu);

        // Guarda cambios
        FileStorage.WriteList(_filePath, menus);
    }

    // =====================================================
    // MÉTODO GET ALL
    // =====================================================
    // Devuelve todos los menús
    public List<Menu> GetAll()
    {
        return FileStorage.ReadList<Menu>(_filePath);
    }

    // =====================================================
    // MÉTODO GET BY ID
    // =====================================================
    // Busca un menú por su ID
    public Menu? GetById(Guid id)
    {
        return GetAll().FirstOrDefault(m => m.Id == id);
    }
}