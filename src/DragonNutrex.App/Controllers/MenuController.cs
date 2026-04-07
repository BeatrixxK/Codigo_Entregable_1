// =====================================================
// IMPORTACIONES
// =====================================================

// Modelos (Menu, RegistroComida, etc.)
using DragonNutrex.App.Models;

// Servicio de menús (lógica de negocio)
using DragonNutrex.App.Services;

namespace DragonNutrex.App.Controllers;

// =====================================================
// CLASE MENU CONTROLLER
// =====================================================
// Actúa como intermediario entre la UI y el servicio de menús
// ✔ conecta servicio con UI
public class MenuController
{
    // Servicio de menús
    private readonly MenuService _service;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe el servicio para usar la lógica de negocio
    public MenuController(MenuService service)
    {
        _service = service;
    }

    // =====================================================
    // MÉTODO CREAR MENU
    // =====================================================
    // Envía el menú al servicio para validación y guardado
    public void CrearMenu(Menu menu)
    {
        _service.CrearMenu(menu);
    }

    // =====================================================
    // MÉTODO ACTUALIZAR MENU
    // =====================================================
    // Envía el menú al servicio para actualización
    public void ActualizarMenu(Menu menu)
    {
        _service.ActualizarMenu(menu);
    }

    // =====================================================
    // MÉTODO ELIMINAR MENU
    // =====================================================
    // Envía el ID al servicio para eliminar el menú
    public void EliminarMenu(Guid id)
    {
        _service.EliminarMenu(id);
    }

    // =====================================================
    // MÉTODO OBTENER MENUS
    // =====================================================
    // Solicita al servicio la lista de menús
    public List<Menu> ObtenerMenus()
    {
        return _service.ObtenerMenus();
    }

    // =====================================================
    // MÉTODO OBTENER MENU
    // =====================================================
    // Solicita un menú específico por ID
    public Menu? ObtenerMenu(Guid id)
    {
        return _service.ObtenerMenu(id);
    }

    // =====================================================
    // MÉTODO AGREGAR PRODUCTO
    // =====================================================
    // Agrega un producto a un menú con cantidad específica
    public void AgregarProducto(Menu menu, Guid productoId, decimal cantidad)
    {
        _service.AgregarProducto(menu, productoId, cantidad);
    }

    // =====================================================
    // MÉTODO ELIMINAR REGISTRO
    // =====================================================
    // Elimina un producto (registro) dentro del menú
    public void EliminarRegistro(Menu menu, Guid registroId)
    {
        _service.EliminarRegistro(menu, registroId);
    }

    // =====================================================
    // MÉTODO ACTUALIZAR REGISTRO
    // =====================================================
    // Actualiza la cantidad de un producto dentro del menú
    public void ActualizarRegistro(Menu menu, Guid registroId, decimal nuevaCantidad)
    {
        _service.ActualizarRegistro(menu, registroId, nuevaCantidad);
    }
}