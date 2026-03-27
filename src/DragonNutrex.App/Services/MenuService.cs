using DragonNutrex.App.Interfaces;
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

public class MenuService
{
    private readonly IRepository<Menu> _menuRepository;
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IRepository<Producto> _productoRepository;

    public MenuService(
        IRepository<Menu> menuRepository,
        IRepository<Usuario> usuarioRepository,
        IRepository<Producto> productoRepository)
    {
        _menuRepository = menuRepository;
        _usuarioRepository = usuarioRepository;
        _productoRepository = productoRepository;
    }

    public void CrearMenu(Menu menu)
    {
        ValidarMenu(menu);
        ValidarDuplicado(menu, excluirMismoId: false);

        RecalcularTotales(menu);
        _menuRepository.Create(menu);
    }

    public void ActualizarMenu(Menu menu)
    {
        ValidarMenu(menu);
        ValidarDuplicado(menu, excluirMismoId: true);

        RecalcularTotales(menu);
        _menuRepository.Update(menu);
    }

    public void EliminarMenu(Guid id)
    {
        _menuRepository.Delete(id);
    }

    public List<Menu> ObtenerMenus()
    {
        return _menuRepository.GetAll();
    }

    public Menu? ObtenerMenu(Guid id)
    {
        return _menuRepository.GetById(id);
    }

    public void AgregarProducto(Menu menu, Guid productoId, decimal cantidad)
    {
        if (cantidad <= 0)
            throw new Exception("La cantidad debe ser mayor que cero.");

        var producto = _productoRepository.GetById(productoId);
        if (producto == null)
            throw new Exception("Producto no encontrado.");

        var registro = new RegistroComida
        {
            ProductoId = producto.Id,
            NombreProducto = producto.Nombre,
            Cantidad = cantidad,
            Calorias = producto.Calorias * cantidad,
            Proteinas = producto.Proteinas * cantidad,
            Carbohidratos = producto.Carbohidratos * cantidad,
            Grasas = producto.Grasas * cantidad
        };

        menu.Registros.Add(registro);
        RecalcularTotales(menu);
    }

    public void EliminarRegistro(Menu menu, Guid registroId)
    {
        var registro = menu.Registros.FirstOrDefault(r => r.Id == registroId);
        if (registro == null)
            throw new Exception("Registro no encontrado.");

        menu.Registros.Remove(registro);
        RecalcularTotales(menu);
    }

    public void ActualizarRegistro(Menu menu, Guid registroId, decimal nuevaCantidad)
    {
        if (nuevaCantidad <= 0)
            throw new Exception("La cantidad debe ser mayor que cero.");

        var registro = menu.Registros.FirstOrDefault(r => r.Id == registroId);
        if (registro == null)
            throw new Exception("Registro no encontrado.");

        var producto = _productoRepository.GetById(registro.ProductoId);
        if (producto == null)
            throw new Exception("Producto no encontrado.");

        registro.Cantidad = nuevaCantidad;
        registro.Calorias = producto.Calorias * nuevaCantidad;
        registro.Proteinas = producto.Proteinas * nuevaCantidad;
        registro.Carbohidratos = producto.Carbohidratos * nuevaCantidad;
        registro.Grasas = producto.Grasas * nuevaCantidad;

        RecalcularTotales(menu);
    }

    private void ValidarMenu(Menu menu)
    {
        var usuario = _usuarioRepository.GetById(menu.UsuarioId);
        if (usuario == null)
            throw new Exception("El usuario seleccionado no existe.");
    }

    private void ValidarDuplicado(Menu menu, bool excluirMismoId)
    {
        var existeDuplicado = _menuRepository.GetAll().Any(m =>
            m.UsuarioId == menu.UsuarioId &&
            m.Fecha.Date == menu.Fecha.Date &&
            (!excluirMismoId || m.Id != menu.Id));

        if (existeDuplicado)
            throw new Exception("Ya existe un menú para este usuario en esa fecha.");
    }

    private void RecalcularTotales(Menu menu)
    {
        menu.TotalCalorias = menu.Registros.Sum(r => r.Calorias);
        menu.TotalProteinas = menu.Registros.Sum(r => r.Proteinas);
        menu.TotalCarbohidratos = menu.Registros.Sum(r => r.Carbohidratos);
        menu.TotalGrasas = menu.Registros.Sum(r => r.Grasas);
    }
}