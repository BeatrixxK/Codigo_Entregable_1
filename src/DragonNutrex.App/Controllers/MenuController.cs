using DragonNutrex.App.Models;
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

public class MenuController
{
    private readonly MenuService _service;

    public MenuController(MenuService service)
    {
        _service = service;
    }

    public void CrearMenu(Menu menu)
    {
        _service.CrearMenu(menu);
    }

    public void ActualizarMenu(Menu menu)
    {
        _service.ActualizarMenu(menu);
    }

    public void EliminarMenu(Guid id)
    {
        _service.EliminarMenu(id);
    }

    public List<Menu> ObtenerMenus()
    {
        return _service.ObtenerMenus();
    }

    public Menu? ObtenerMenu(Guid id)
    {
        return _service.ObtenerMenu(id);
    }

    public void AgregarProducto(Menu menu, Guid productoId, decimal cantidad)
    {
        _service.AgregarProducto(menu, productoId, cantidad);
    }

    public void EliminarRegistro(Menu menu, Guid registroId)
    {
        _service.EliminarRegistro(menu, registroId);
    }

    public void ActualizarRegistro(Menu menu, Guid registroId, decimal nuevaCantidad)
    {
        _service.ActualizarRegistro(menu, registroId, nuevaCantidad);
    }
}