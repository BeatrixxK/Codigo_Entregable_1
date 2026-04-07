// =====================================================
// IMPORTACIONES
// =====================================================

// Interfaz del repositorio genérico
using DragonNutrex.App.Interfaces;

// Modelo Producto
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

// =====================================================
// CLASE PRODUCTO SERVICE
// =====================================================
// Maneja la lógica de negocio para productos
// ✔ validaciones
// ✔ conexión con repositorio
public class ProductoService
{
    // Repositorio de productos
    private readonly IRepository<Producto> _repository;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe el repositorio para acceder a los datos
    public ProductoService(IRepository<Producto> repository)
    {
        _repository = repository;
    }

    // =====================================================
    // MÉTODO CREAR PRODUCTO
    // =====================================================
    // Valida los datos y guarda el producto
    public void CrearProducto(Producto producto)
    {
        ValidarProducto(producto);
        _repository.Create(producto);
    }

    // =====================================================
    // MÉTODO ACTUALIZAR PRODUCTO
    // =====================================================
    // Valida los datos y actualiza el producto
    public void ActualizarProducto(Producto producto)
    {
        ValidarProducto(producto);
        _repository.Update(producto);
    }

    // =====================================================
    // MÉTODO ELIMINAR PRODUCTO
    // =====================================================
    // Elimina un producto por su ID
    public void EliminarProducto(Guid id)
    {
        _repository.Delete(id);
    }

    // =====================================================
    // MÉTODO OBTENER PRODUCTOS
    // =====================================================
    // Devuelve la lista de todos los productos
    public List<Producto> ObtenerProductos()
    {
        return _repository.GetAll();
    }

    // =====================================================
    // MÉTODO OBTENER PRODUCTO
    // =====================================================
    // Busca un producto por su ID
    public Producto? ObtenerProducto(Guid id)
    {
        return _repository.GetById(id);
    }

    // =====================================================
    // MÉTODO VALIDAR PRODUCTO
    // =====================================================
    // Verifica que los datos del producto sean válidos
    private void ValidarProducto(Producto producto)
    {
        // Valida que el nombre no esté vacío
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new Exception("El nombre del producto es obligatorio.");

        // Valida que las calorías no sean negativas
        if (producto.Calorias < 0)
            throw new Exception("Las calorías no pueden ser negativas.");

        // Valida que las proteínas no sean negativas
        if (producto.Proteinas < 0)
            throw new Exception("Las proteínas no pueden ser negativas.");

        // Valida que los carbohidratos no sean negativos
        if (producto.Carbohidratos < 0)
            throw new Exception("Los carbohidratos no pueden ser negativos.");

        // Valida que las grasas no sean negativas
        if (producto.Grasas < 0)
            throw new Exception("Las grasas no pueden ser negativas.");
    }
}