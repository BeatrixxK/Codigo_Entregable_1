// =====================================================
// IMPORTACIONES
// =====================================================

// Interfaz del repositorio genérico
using DragonNutrex.App.Interfaces;

// Modelo Producto
using DragonNutrex.App.Models;

// Utilidad para manejo de archivos JSON
using DragonNutrex.App.Utils;

namespace DragonNutrex.App.Repositories;

// =====================================================
// CLASE PRODUCTO REPOSITORY
// =====================================================
// Implementa el acceso a datos para productos usando archivos JSON
// ✔ CRUD completo
// ✔ persistencia en archivo
public class ProductoRepository : IRepository<Producto>
{
    // Ruta del archivo JSON donde se almacenan los productos
    private readonly string _filePath = Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "DragonNutrex.App",
            "Data",
            "productos.json"
        )
    );

    // =====================================================
    // MÉTODO CREATE
    // =====================================================
    // Agrega un nuevo producto al archivo
    public void Create(Producto entity)
    {
        // Obtiene la lista actual
        var productos = GetAll();

        // Agrega el nuevo producto
        productos.Add(entity);

        // Guarda en JSON
        FileStorage.WriteList(_filePath, productos);
    }

    // =====================================================
    // MÉTODO UPDATE
    // =====================================================
    // Actualiza un producto existente
    public void Update(Producto entity)
    {
        // Obtiene la lista actual
        var productos = GetAll();

        // Busca el índice del producto por ID
        var index = productos.FindIndex(p => p.Id == entity.Id);

        // Si no existe, lanza error
        if (index == -1)
            throw new Exception("Producto no encontrado.");

        // Reemplaza el producto en la lista
        productos[index] = entity;

        // Guarda cambios
        FileStorage.WriteList(_filePath, productos);
    }

    // =====================================================
    // MÉTODO DELETE
    // =====================================================
    // Elimina un producto por ID
    public void Delete(Guid id)
    {
        // Obtiene la lista actual
        var productos = GetAll();

        // Busca el producto
        var producto = productos.FirstOrDefault(p => p.Id == id);

        // Si no existe, lanza error
        if (producto == null)
            throw new Exception("Producto no encontrado.");

        // Elimina el producto
        productos.Remove(producto);

        // Guarda cambios
        FileStorage.WriteList(_filePath, productos);
    }

    // =====================================================
    // MÉTODO GET ALL
    // =====================================================
    // Devuelve todos los productos
    public List<Producto> GetAll()
    {
        return FileStorage.ReadList<Producto>(_filePath);
    }

    // =====================================================
    // MÉTODO GET BY ID
    // =====================================================
    // Busca un producto por su ID
    public Producto? GetById(Guid id)
    {
        return GetAll().FirstOrDefault(p => p.Id == id);
    }
}