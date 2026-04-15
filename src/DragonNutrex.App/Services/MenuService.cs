// =====================================================
// IMPORTACIONES
// =====================================================

// Interfaz de repositorio
using DragonNutrex.App.Interfaces;

// Modelos (Menu, Usuario, Producto, etc.)
using DragonNutrex.App.Models;

namespace DragonNutrex.App.Services;

// =====================================================
// CLASE MENU SERVICE
// =====================================================
// Maneja la lógica de negocio de los menús
// ✔ validaciones
// ✔ relación usuario-producto
// ✔ cálculos nutricionales
public class MenuService
{
    // Repositorios necesarios
    private readonly IRepository<Menu> _menuRepository;
    private readonly IRepository<Usuario> _usuarioRepository;
    private readonly IRepository<Producto> _productoRepository; // ✨ Una sola variable correcta

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Recibe los repositorios para trabajar con los datos
    public MenuService(
        IRepository<Menu> menuRepository,
        IRepository<Usuario> usuarioRepository,
        IRepository<Producto> productoRepository) // ✨ Nombre en minúscula por convención
    {
        _menuRepository = menuRepository;
        _usuarioRepository = usuarioRepository;
        _productoRepository = productoRepository; // ✨ Asignación correcta
    }

    // =====================================================
    // MÉTODO CREAR MENU
    // =====================================================
    // Valida, evita duplicados y guarda el menú
    public void CrearMenu(Menu menu)
    {
        ValidarMenu(menu); // valida usuario
        ValidarDuplicado(menu, excluirMismoId: false); // evita duplicados

        RecalcularTotales(menu); // calcula totales nutricionales
        _menuRepository.Create(menu); // guarda en repositorio
    }

    // =====================================================
    // MÉTODO ACTUALIZAR MENU
    // =====================================================
    // Valida, evita duplicados y actualiza el menú
    public void ActualizarMenu(Menu menu)
    {
        ValidarMenu(menu);
        ValidarDuplicado(menu, excluirMismoId: true);

        RecalcularTotales(menu);
        _menuRepository.Update(menu);
    }

    // =====================================================
    // MÉTODO ELIMINAR MENU
    // =====================================================
    // Elimina un menú por ID
    public void EliminarMenu(Guid id)
    {
        _menuRepository.Delete(id);
    }

    // =====================================================
    // MÉTODO OBTENER MENUS
    // =====================================================
    // Devuelve todos los menús
    public List<Menu> ObtenerMenus()
    {
        return _menuRepository.GetAll();
    }

    // =====================================================
    // MÉTODO OBTENER MENU
    // =====================================================
    // Busca un menú por ID
    public Menu? ObtenerMenu(Guid id)
    {
        return _menuRepository.GetById(id);
    }

    // =====================================================
    // MÉTODO AGREGAR PRODUCTO
    // =====================================================
    // Agrega un producto al menú con su cálculo nutricional
    public void AgregarProducto(Menu menu, Guid productoId, decimal cantidad)
    {
        // Valida cantidad
        if (cantidad <= 0)
            throw new Exception("La cantidad debe ser mayor que cero.");

        // Busca el producto usando la variable correcta ✨
        var producto = _productoRepository.GetById(productoId);
        if (producto == null)
            throw new Exception("Producto no encontrado.");

        // Crea registro de comida
        var registro = new RegistroComida
        {
            ProductoId = producto.Id,
            NombreProducto = producto.Nombre,
            Cantidad = cantidad,

            // Cálculo nutricional
            Calorias = producto.Calorias * cantidad,
            Proteinas = producto.Proteinas * cantidad,
            Carbohidratos = producto.Carbohidratos * cantidad,
            Grasas = producto.Grasas * cantidad
        };

        // Agrega al menú
        menu.Registros.Add(registro);

        // Recalcula totales
        RecalcularTotales(menu);
    }

    // =====================================================
    // MÉTODO ELIMINAR REGISTRO
    // =====================================================
    // Elimina un producto del menú
    public void EliminarRegistro(Menu menu, Guid registroId)
    {
        var registro = menu.Registros.FirstOrDefault(r => r.Id == registroId);

        if (registro == null)
            throw new Exception("Registro no encontrado.");

        menu.Registros.Remove(registro);

        RecalcularTotales(menu);
    }

    // =====================================================
    // MÉTODO ACTUALIZAR REGISTRO
    // =====================================================
    // Modifica la cantidad de un producto en el menú
    public void ActualizarRegistro(Menu menu, Guid registroId, decimal nuevaCantidad)
    {
        // Valida cantidad
        if (nuevaCantidad <= 0)
            throw new Exception("La cantidad debe ser mayor que cero.");

        var registro = menu.Registros.FirstOrDefault(r => r.Id == registroId);
        if (registro == null)
            throw new Exception("Registro no encontrado.");

        // Usa la variable correcta ✨
        var producto = _productoRepository.GetById(registro.ProductoId);
        if (producto == null)
            throw new Exception("Producto no encontrado.");

        // Actualiza valores
        registro.Cantidad = nuevaCantidad;
        registro.Calorias = producto.Calorias * nuevaCantidad;
        registro.Proteinas = producto.Proteinas * nuevaCantidad;
        registro.Carbohidratos = producto.Carbohidratos * nuevaCantidad;
        registro.Grasas = producto.Grasas * nuevaCantidad;

        RecalcularTotales(menu);
    }

    // =====================================================
    // VALIDACIÓN DE MENÚ
    // =====================================================
    // Verifica que el usuario exista
    private void ValidarMenu(Menu menu)
    {
        var usuario = _usuarioRepository.GetById(menu.UsuarioId);
        if (usuario == null)
            throw new Exception("El usuario seleccionado no existe.");
    }

    // =====================================================
    // VALIDACIÓN DE DUPLICADOS
    // =====================================================
    // Evita que un usuario tenga más de un menú por día
    private void ValidarDuplicado(Menu menu, bool excluirMismoId)
    {
        var existeDuplicado = _menuRepository.GetAll().Any(m =>
            m.UsuarioId == menu.UsuarioId &&
            m.Fecha.Date == menu.Fecha.Date &&
            (!excluirMismoId || m.Id != menu.Id));

        if (existeDuplicado)
            throw new Exception("Ya existe un menú para este usuario en esa fecha.");
    }

    // =====================================================
    // RECÁLCULO DE TOTALES
    // =====================================================
    // Suma todos los valores nutricionales del menú
    private void RecalcularTotales(Menu menu)
    {
        menu.TotalCalorias = menu.Registros.Sum(r => r.Calorias);
        menu.TotalProteinas = menu.Registros.Sum(r => r.Proteinas);
        menu.TotalCarbohidratos = menu.Registros.Sum(r => r.Carbohidratos);
        menu.TotalGrasas = menu.Registros.Sum(r => r.Grasas);
    }
}