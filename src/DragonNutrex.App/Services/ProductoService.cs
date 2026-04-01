using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

public class ProductoService
{
    private readonly IRepository<Producto> _repository;

    public ProductoService(IRepository<Producto> repository)
    {
        _repository = repository;
    }

    public void CrearProducto(Producto producto)
    {
        ValidarProducto(producto);
        _repository.Create(producto);
    }

    public void ActualizarProducto(Producto producto)
    {
        ValidarProducto(producto);
        _repository.Update(producto);
    }

    public void EliminarProducto(Guid id)
    {
        _repository.Delete(id);
    }

    public List<Producto> ObtenerProductos()
    {
        return _repository.GetAll();
    }

    public Producto? ObtenerProducto(Guid id)
    {
        return _repository.GetById(id);
    }

    private void ValidarProducto(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new Exception("El nombre del producto es obligatorio.");

        if (producto.Calorias < 0)
            throw new Exception("Las calorías no pueden ser negativas.");

        if (producto.Proteinas < 0)
            throw new Exception("Las proteínas no pueden ser negativas.");

        if (producto.Carbohidratos < 0)
            throw new Exception("Los carbohidratos no pueden ser negativos.");

        if (producto.Grasas < 0)
            throw new Exception("Las grasas no pueden ser negativas.");
    }
}