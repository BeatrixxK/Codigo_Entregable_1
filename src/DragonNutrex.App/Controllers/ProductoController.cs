// =====================================================
// IMPORTACIONES
// =====================================================

// Modelo Producto
using DragonNutrex.App.Models;

// Servicio de productos (lógica de negocio)
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

// =====================================================
// CLASE PRODUCTO CONTROLLER
// =====================================================
// Actúa como intermediario entre la UI y el servicio
// ✔ conecta servicio con UI
public class ProductoController
{
    // Servicio de productos
    private readonly ProductoService _service;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe el servicio para usar la lógica de negocio
    public ProductoController(ProductoService service)
    {
        _service = service;
    }

    // =====================================================
    // MÉTODO CREAR PRODUCTO
    // =====================================================
    // Envía el producto al servicio para validación y guardado
    public void CrearProducto(Producto producto)
    {
        _service.CrearProducto(producto);
    }

    // =====================================================
    // MÉTODO ACTUALIZAR PRODUCTO
    // =====================================================
    // Envía el producto al servicio para actualización
    public void ActualizarProducto(Producto producto)
    {
        _service.ActualizarProducto(producto);
    }

    // =====================================================
    // MÉTODO ELIMINAR PRODUCTO
    // =====================================================
    // Envía el ID al servicio para eliminar el producto
    public void EliminarProducto(Guid id)
    {
        _service.EliminarProducto(id);
    }

    // =====================================================
    // MÉTODO OBTENER PRODUCTOS
    // =====================================================
    // Solicita al servicio la lista de productos
    public List<Producto> ObtenerProductos()
    {
        return _service.ObtenerProductos();
    }

    // =====================================================
    // MÉTODO OBTENER PRODUCTO
    // =====================================================
    // Solicita al servicio un producto específico por ID
    public Producto? ObtenerProducto(Guid id)
    {
        return _service.ObtenerProducto(id);
    }
}