using DragonNutrex.App.Models;
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

public class ProductoController
{
    private readonly ProductoService _service;

    public ProductoController(ProductoService service)
    {
        _service = service;
    }

    public void CrearProducto(Producto producto)
    {
        _service.CrearProducto(producto);
    }

    public void ActualizarProducto(Producto producto)
    {
        _service.ActualizarProducto(producto);
    }

    public void EliminarProducto(Guid id)
    {
        _service.EliminarProducto(id);
    }

    public List<Producto> ObtenerProductos()
    {
        return _service.ObtenerProductos();
    }

    public Producto? ObtenerProducto(Guid id)
    {
        return _service.ObtenerProducto(id);
    }
}