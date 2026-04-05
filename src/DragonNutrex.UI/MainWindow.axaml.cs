using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Models;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;
using MenuModel = DragonNutrex.App.Models.Menu;

namespace DragonNutrex.UI;

public partial class MainWindow : Window
{
    // =====================================================
    // CONTROLADORES
    // =====================================================
    // Estos controladores conectan la interfaz con la lógica
    // de negocio y los repositorios de datos.
    private readonly UsuarioController _usuarioController;
    private readonly ProductoController _productoController;
    private readonly MenuController _menuController;
    private readonly EstadisticasNutricionController _estadisticasNutricionController;

    // =====================================================
    // OBJETOS SELECCIONADOS EN LA UI
    // =====================================================
    // Se usan para saber qué registro está siendo editado
    // o seleccionado actualmente en cada módulo.
    private Usuario? _usuarioSeleccionado;
    private Producto? _productoSeleccionado;
    private MenuModel? _menuActual;
    private RegistroComida? _registroSeleccionado;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Inicializa la ventana, crea controladores, registra eventos
    // y carga datos iniciales.
    public MainWindow()
    {
        InitializeComponent();

        // Cargar combos del módulo de usuarios
        CargarObjetivosEnCombo();
        CargarActividadesEnCombo();

        // Inicializar controladores
        _usuarioController = new UsuarioController(new UsuarioService(new UsuarioRepository()));
        _productoController = new ProductoController(new ProductoService(new ProductoRepository()));
        _menuController = new MenuController(
            new MenuService(
                new MenuRepository(),
                new UsuarioRepository(),
                new ProductoRepository()
            )
        );
        _estadisticasNutricionController = new EstadisticasNutricionController(
            new EstadisticasNutricionService(
                new UsuarioRepository(),
                new MenuRepository()
            )
        );

        // =====================================================
        // EVENTOS UI
        // =====================================================

        // ---------- Usuarios ----------
        GuardarButton.Click += GuardarUsuario;
        EliminarButton.Click += EliminarUsuario;
        LimpiarButton.Click += (_, _) => LimpiarUsuario();

        // ---------- Productos ----------
        GuardarProductoButton.Click += GuardarProducto;
        EliminarProductoButton.Click += EliminarProducto;
        LimpiarProductoButton.Click += (_, _) => LimpiarProducto();

        // ---------- Selección en DataGrid ----------
        UsuariosDataGrid.SelectionChanged += UsuariosDataGrid_SelectionChanged;
        ProductosDataGrid.SelectionChanged += ProductosDataGrid_SelectionChanged;
        RegistrosMenuDataGrid.SelectionChanged += RegistrosMenuDataGrid_SelectionChanged;
        MenusDataGrid.SelectionChanged += MenusDataGrid_SelectionChanged;

        // ---------- Navegación entre módulos ----------
        UsuariosModuloButton.Click += (_, _) => MostrarSoloPanelUsuarios();
        ProductosModuloButton.Click += (_, _) => MostrarSoloPanelProductos();

        MenusModuloButton.Click += (_, _) =>
        {
            MostrarSoloPanelMenus();
            CargarUsuariosEnCombo();
            CargarProductosEnCombo();
            CargarMenus();
        };

        EstadisticasNutricionModuloButton.Click += (_, _) =>
        {
            MostrarSoloPanelEstadisticasNutricion();
            CargarUsuariosEnComboEstadistica();
            CargarDietasEnComboEstadistica();
            LimpiarResumenEstadisticaNutricion();
        };

        // ---------- Logout ----------
        LogoutButton.Click += Logout;
        LogoutButtonEstadisticas.Click += Logout;

        // ---------- Menús ----------
        NuevoMenuButton.Click += NuevoMenu;
        AgregarProductoMenuButton.Click += AgregarProductoAlMenu;
        GuardarMenuButton.Click += GuardarMenu;
        ActualizarRegistroMenuButton.Click += ActualizarRegistroMenu;
        EliminarRegistroMenuButton.Click += EliminarRegistroMenu;
        EliminarMenuButton.Click += EliminarMenuCompleto;
        LimpiarMenuButton.Click += (_, _) => LimpiarMenu();

        // ---------- Estadísticas ----------
        CalcularEstadisticaNutricionButton.Click += CalcularEstadisticasNutricion;
        UsuariosEstadisticaComboBox.SelectionChanged += UsuariosEstadisticaComboBox_SelectionChanged;

        // =====================================================
        // CARGA INICIAL
        // =====================================================
        CargarDietasEnComboUsuarios();

        AplicarPermisosPorSesion();
        AplicarPermisosEstadisticas();
        MostrarSesionActual();

        CargarUsuarios();
        CargarProductos();
        CargarUsuariosEnCombo();
        CargarProductosEnCombo();
        CargarMenus();
        CargarUsuariosEnComboEstadistica();
        CargarDietasEnComboEstadistica();

        LimpiarUsuario();
        LimpiarProducto();
        LimpiarMenu();
        LimpiarResumenEstadisticaNutricion();
    }

    // =====================================================
    // SESIÓN / LOGOUT
    // =====================================================

    // Cierra la sesión actual, abre la ventana de login
    // y cierra la ventana principal.
    private void Logout(object? sender, RoutedEventArgs e)
    {
        AuthSession.CerrarSesion();

        var login = new LoginWindow();
        login.Show();

        Close();
    }

    // =====================================================
    // SESIÓN / PERMISOS
    // =====================================================

    // Aplica permisos según el tipo de usuario que inició sesión.
    // Si es admin, puede ver todos los módulos.
    // Si no es admin, solo ve menús y estadísticas.
    private void AplicarPermisosPorSesion()
    {
        if (AuthSession.EsAdmin)
        {
            MostrarSoloPanelUsuarios();
            return;
        }

        UsuariosModuloButton.IsVisible = false;
        ProductosModuloButton.IsVisible = false;

        MenusModuloButton.IsVisible = true;
        EstadisticasNutricionModuloButton.IsVisible = true;

        UsuariosPanel.IsVisible = false;
        ProductosPanel.IsVisible = false;
        MenusPanel.IsVisible = true;
        EstadisticasNutricionPanel.IsVisible = false;

        GuardarButton.IsVisible = false;
        EliminarButton.IsVisible = false;
        LimpiarButton.IsVisible = false;

        GuardarProductoButton.IsVisible = false;
        EliminarProductoButton.IsVisible = false;
        LimpiarProductoButton.IsVisible = false;
    }

    // En estadísticas, el admin puede seleccionar cualquier usuario.
    // Un usuario normal solo puede ver sus propias estadísticas.
    private void AplicarPermisosEstadisticas()
    {
        UsuarioFiltroPanel.IsVisible = AuthSession.EsAdmin;
    }

    // Muestra en pantalla quién tiene la sesión activa.
    private void MostrarSesionActual()
    {
        if (AuthSession.EsAdmin)
            SesionTextBlock.Text = "Sesión: Administrador";
        else
            SesionTextBlock.Text = $"Sesión: {AuthSession.NombreUsuario}";
    }

    // Muestra solo el panel de usuarios.
    private void MostrarSoloPanelUsuarios()
    {
        if (!AuthSession.EsAdmin)
            return;

        UsuariosPanel.IsVisible = true;
        ProductosPanel.IsVisible = false;
        MenusPanel.IsVisible = false;
        EstadisticasNutricionPanel.IsVisible = false;
    }

    // Muestra solo el panel de productos.
    private void MostrarSoloPanelProductos()
    {
        if (!AuthSession.EsAdmin)
            return;

        UsuariosPanel.IsVisible = false;
        ProductosPanel.IsVisible = true;
        MenusPanel.IsVisible = false;
        EstadisticasNutricionPanel.IsVisible = false;
    }

    // Muestra solo el panel de menús.
    private void MostrarSoloPanelMenus()
    {
        UsuariosPanel.IsVisible = false;
        ProductosPanel.IsVisible = false;
        MenusPanel.IsVisible = true;
        EstadisticasNutricionPanel.IsVisible = false;
    }

    // Muestra solo el panel de estadísticas nutricionales.
    private void MostrarSoloPanelEstadisticasNutricion()
    {
        UsuariosPanel.IsVisible = false;
        ProductosPanel.IsVisible = false;
        MenusPanel.IsVisible = false;
        EstadisticasNutricionPanel.IsVisible = true;
    }

    // =====================================================
    // PANEL USUARIOS
    // =====================================================

    // Guarda un usuario nuevo o actualiza uno existente.
    private async void GuardarUsuario(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_usuarioSeleccionado == null)
            {
                var usuario = new Usuario
                {
                    Nombre = NombreTextBox.Text ?? "",
                    Peso = decimal.TryParse(PesoTextBox.Text, out var peso) ? peso : 0,
                    Altura = decimal.TryParse(AlturaTextBox.Text, out var altura) ? altura : 0,
                    Actividad = ObtenerActividadSeleccionada(),
                    Objetivo = ObtenerObjetivoSeleccionado(),
                    TipoDieta = ObtenerTipoDietaUsuarioSeleccionada(),
                    Password = "Upi.2025"
                };

                _usuarioController.CrearUsuario(usuario);
            }
            else
            {
                _usuarioSeleccionado.Nombre = NombreTextBox.Text ?? "";
                _usuarioSeleccionado.Peso = decimal.TryParse(PesoTextBox.Text, out var peso) ? peso : 0;
                _usuarioSeleccionado.Altura = decimal.TryParse(AlturaTextBox.Text, out var altura) ? altura : 0;
                _usuarioSeleccionado.Actividad = ObtenerActividadSeleccionada();
                _usuarioSeleccionado.Objetivo = ObtenerObjetivoSeleccionado();
                _usuarioSeleccionado.TipoDieta = ObtenerTipoDietaUsuarioSeleccionada();

                _usuarioController.ActualizarUsuario(_usuarioSeleccionado);
            }

            CargarUsuarios();
            CargarUsuariosEnCombo();
            CargarUsuariosEnComboEstadistica();
            LimpiarUsuario();
        }
        catch (Exception ex)
        {
            await MostrarMensaje(
                string.IsNullOrWhiteSpace(ex.Message) ? "Nada que guardar." : ex.Message
            );
        }
    }

    // Elimina el usuario seleccionado, previa confirmación.
    private async void EliminarUsuario(object? sender, RoutedEventArgs e)
    {
        if (UsuariosDataGrid.SelectedItem is Usuario usuario)
        {
            bool confirm = await Confirmar($"Eliminar a {usuario.Nombre}?");
            if (!confirm)
                return;

            _usuarioController.EliminarUsuario(usuario.Id);
            CargarUsuarios();
            CargarUsuariosEnCombo();
            CargarUsuariosEnComboEstadistica();
            LimpiarUsuario();
        }
    }

    // Cuando se selecciona un usuario en la tabla,
    // se cargan sus datos en el formulario.
    private void UsuariosDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (UsuariosDataGrid.SelectedItem is Usuario usuario)
        {
            _usuarioSeleccionado = usuario;

            NombreTextBox.Text = usuario.Nombre;
            PesoTextBox.Text = usuario.Peso.ToString();
            AlturaTextBox.Text = usuario.Altura.ToString();
            SeleccionarActividad(usuario.Actividad);
            SeleccionarObjetivo(usuario.Objetivo);
            SeleccionarDietaUsuario(usuario.TipoDieta);

            GuardarButton.Content = "Actualizar";
        }
    }

    // Carga la lista de usuarios en el DataGrid.
    private void CargarUsuarios()
    {
        var usuarios = _usuarioController.ObtenerUsuarios();

        if (!AuthSession.EsAdmin)
            usuarios = usuarios.Where(u => u.Id == AuthSession.UsuarioId).ToList();

        UsuariosDataGrid.ItemsSource = null;
        UsuariosDataGrid.ItemsSource = usuarios;
    }

    // Limpia el formulario del módulo de usuarios.
    private void LimpiarUsuario()
    {
        _usuarioSeleccionado = null;

        NombreTextBox.Text = "";
        PesoTextBox.Text = "";
        AlturaTextBox.Text = "";

        if (ActividadComboBox.ItemCount > 0)
            ActividadComboBox.SelectedIndex = 0;

        if (ObjetivoComboBox.ItemCount > 0)
            ObjetivoComboBox.SelectedIndex = 0;

        if (TipoDietaComboBox.ItemCount > 0)
            TipoDietaComboBox.SelectedIndex = 0;

        UsuariosDataGrid.SelectedItem = null;
        GuardarButton.Content = "Guardar";
    }

    // Carga las dietas disponibles en el ComboBox del formulario de usuarios.
    private void CargarDietasEnComboUsuarios()
    {
        TipoDietaComboBox.Items.Clear();

        TipoDietaComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Seleccione dieta",
            Tag = "",
            IsEnabled = false
        });

        var dietas = _estadisticasNutricionController.ObtenerDietasDisponibles();

        foreach (var dieta in dietas)
        {
            TipoDietaComboBox.Items.Add(new ComboBoxItem
            {
                Content = dieta.Nombre,
                Tag = dieta.Nombre
            });
        }

        TipoDietaComboBox.SelectedIndex = 0;
    }

    // Devuelve la dieta seleccionada en el formulario de usuarios.
    private string ObtenerTipoDietaUsuarioSeleccionada()
    {
        if (TipoDietaComboBox.SelectedItem is ComboBoxItem item && item.Tag is string nombre)
            return nombre;

        return string.Empty;
    }

    // Selecciona en el ComboBox la dieta del usuario.
    private void SeleccionarDietaUsuario(string tipoDieta)
    {
        foreach (var item in TipoDietaComboBox.Items)
        {
            if (item is ComboBoxItem comboItem &&
                comboItem.Tag is string nombre &&
                nombre.Equals(tipoDieta, StringComparison.OrdinalIgnoreCase))
            {
                TipoDietaComboBox.SelectedItem = comboItem;
                break;
            }
        }
    }

    // Carga las actividades en el ComboBox.
    private void CargarActividadesEnCombo()
    {
        ActividadComboBox.Items.Clear();

        ActividadComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Seleccione actividad",
            Tag = "",
            IsEnabled = false
        });

        ActividadComboBox.Items.Add(new ComboBoxItem { Content = "Sedentaria", Tag = "Sedentaria" });
        ActividadComboBox.Items.Add(new ComboBoxItem { Content = "Ligera", Tag = "Ligera" });
        ActividadComboBox.Items.Add(new ComboBoxItem { Content = "Moderada", Tag = "Moderada" });
        ActividadComboBox.Items.Add(new ComboBoxItem { Content = "Alta", Tag = "Alta" });
        ActividadComboBox.Items.Add(new ComboBoxItem { Content = "Muy alta", Tag = "Muy alta" });

        ActividadComboBox.SelectedIndex = 0;
    }

    // Devuelve la actividad seleccionada en el ComboBox.
    private string ObtenerActividadSeleccionada()
    {
        if (ActividadComboBox.SelectedItem is ComboBoxItem item && item.Tag is string actividad)
            return actividad;

        return string.Empty;
    }

    // Selecciona la actividad del usuario en el ComboBox.
    private void SeleccionarActividad(string actividad)
    {
        foreach (var item in ActividadComboBox.Items)
        {
            if (item is ComboBoxItem comboItem &&
                comboItem.Tag is string valor &&
                valor.Equals(actividad, StringComparison.OrdinalIgnoreCase))
            {
                ActividadComboBox.SelectedItem = comboItem;
                break;
            }
        }
    }

    // Carga los objetivos en el ComboBox.
    private void CargarObjetivosEnCombo()
    {
        ObjetivoComboBox.Items.Clear();

        ObjetivoComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Seleccione objetivo",
            Tag = "",
            IsEnabled = false
        });

        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Perder peso", Tag = "Perder peso" });
        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Quemar grasa", Tag = "Quemar grasa" });
        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Mantener peso", Tag = "Mantener peso" });
        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Ganar masa muscular", Tag = "Ganar masa muscular" });
        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Mejorar resistencia", Tag = "Mejorar resistencia" });
        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Mejorar salud", Tag = "Mejorar salud" });

        ObjetivoComboBox.SelectedIndex = 0;
    }

    // Devuelve el objetivo seleccionado.
    private string ObtenerObjetivoSeleccionado()
    {
        if (ObjetivoComboBox.SelectedItem is ComboBoxItem item && item.Tag is string objetivo)
            return objetivo;

        return string.Empty;
    }

    // Selecciona el objetivo del usuario en el ComboBox.
    private void SeleccionarObjetivo(string objetivo)
    {
        foreach (var item in ObjetivoComboBox.Items)
        {
            if (item is ComboBoxItem comboItem &&
                comboItem.Tag is string valor &&
                valor.Equals(objetivo, StringComparison.OrdinalIgnoreCase))
            {
                ObjetivoComboBox.SelectedItem = comboItem;
                break;
            }
        }
    }

    // =====================================================
    // PANEL PRODUCTOS
    // =====================================================

    // Guarda un producto nuevo o actualiza uno existente.
    private async void GuardarProducto(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_productoSeleccionado == null)
            {
                var producto = new Producto
                {
                    Nombre = ProductoNombreTextBox.Text ?? "",
                    Calorias = decimal.TryParse(CaloriasTextBox.Text, out var calorias) ? calorias : 0,
                    Proteinas = decimal.TryParse(ProteinasTextBox.Text, out var proteinas) ? proteinas : 0,
                    Carbohidratos = decimal.TryParse(CarbohidratosTextBox.Text, out var carbohidratos) ? carbohidratos : 0,
                    Grasas = decimal.TryParse(GrasasTextBox.Text, out var grasas) ? grasas : 0
                };

                _productoController.CrearProducto(producto);
            }
            else
            {
                _productoSeleccionado.Nombre = ProductoNombreTextBox.Text ?? "";
                _productoSeleccionado.Calorias = decimal.TryParse(CaloriasTextBox.Text, out var calorias) ? calorias : 0;
                _productoSeleccionado.Proteinas = decimal.TryParse(ProteinasTextBox.Text, out var proteinas) ? proteinas : 0;
                _productoSeleccionado.Carbohidratos = decimal.TryParse(CarbohidratosTextBox.Text, out var carbohidratos) ? carbohidratos : 0;
                _productoSeleccionado.Grasas = decimal.TryParse(GrasasTextBox.Text, out var grasas) ? grasas : 0;

                _productoController.ActualizarProducto(_productoSeleccionado);
            }

            CargarProductos();
            CargarProductosEnCombo();
            LimpiarProducto();
        }
        catch (Exception ex)
        {
            await MostrarMensaje(
                string.IsNullOrWhiteSpace(ex.Message) ? "Nada que guardar." : ex.Message
            );
        }
    }

    // Elimina el producto seleccionado.
    private async void EliminarProducto(object? sender, RoutedEventArgs e)
    {
        if (ProductosDataGrid.SelectedItem is Producto producto)
        {
            bool confirm = await Confirmar($"Eliminar {producto.Nombre}?");
            if (!confirm)
                return;

            _productoController.EliminarProducto(producto.Id);
            CargarProductos();
            CargarProductosEnCombo();
            LimpiarProducto();
        }
    }

    // Carga el producto seleccionado en el formulario.
    private void ProductosDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ProductosDataGrid.SelectedItem is Producto producto)
        {
            _productoSeleccionado = producto;

            ProductoNombreTextBox.Text = producto.Nombre;
            CaloriasTextBox.Text = producto.Calorias.ToString();
            ProteinasTextBox.Text = producto.Proteinas.ToString();
            CarbohidratosTextBox.Text = producto.Carbohidratos.ToString();
            GrasasTextBox.Text = producto.Grasas.ToString();

            GuardarProductoButton.Content = "Actualizar Producto";
        }
    }

    // Carga los productos en el DataGrid.
    private void CargarProductos()
    {
        ProductosDataGrid.ItemsSource = null;
        ProductosDataGrid.ItemsSource = _productoController.ObtenerProductos();
    }

    // Limpia el formulario del módulo productos.
    private void LimpiarProducto()
    {
        _productoSeleccionado = null;

        ProductoNombreTextBox.Text = "";
        CaloriasTextBox.Text = "";
        ProteinasTextBox.Text = "";
        CarbohidratosTextBox.Text = "";
        GrasasTextBox.Text = "";

        ProductosDataGrid.SelectedItem = null;
        GuardarProductoButton.Content = "Guardar Producto";
    }

    // =====================================================
    // PANEL MENÚS
    // =====================================================

    // Crea un nuevo menú en memoria.
    private async void NuevoMenu(object? sender, RoutedEventArgs e)
    {
        try
        {
            var usuarioId = ObtenerUsuarioIdSeleccionado();
            if (usuarioId == Guid.Empty)
            {
                await MostrarMensaje("Seleccione un usuario para el menú.");
                return;
            }

            var fecha = FechaMenuDatePicker.SelectedDate?.Date ?? DateTime.Today;

            _menuActual = new MenuModel
            {
                UsuarioId = usuarioId,
                Fecha = fecha,
                Registros = new List<RegistroComida>()
            };

            RegistrosMenuDataGrid.ItemsSource = null;
            RegistrosMenuDataGrid.ItemsSource = _menuActual.Registros.ToList();
            ActualizarTotalesMenu();

            await MostrarMensaje("Nuevo menú creado en memoria. Ahora puede agregar productos.");
        }
        catch (Exception ex)
        {
            await MostrarMensaje(ex.Message);
        }
    }

    // Agrega un producto al menú actual.
    private async void AgregarProductoAlMenu(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_menuActual == null)
            {
                var usuarioId = ObtenerUsuarioIdSeleccionado();
                if (usuarioId == Guid.Empty)
                {
                    await MostrarMensaje("Seleccione un usuario antes de agregar productos.");
                    return;
                }

                _menuActual = new MenuModel
                {
                    UsuarioId = usuarioId,
                    Fecha = FechaMenuDatePicker.SelectedDate?.Date ?? DateTime.Today,
                    Registros = new List<RegistroComida>()
                };
            }

            var productoId = ObtenerProductoIdSeleccionado();
            if (productoId == Guid.Empty)
            {
                await MostrarMensaje("Seleccione un producto.");
                return;
            }

            var cantidad = decimal.TryParse(CantidadProductoTextBox.Text, out var cantidadParseada)
                ? cantidadParseada
                : 0;

            _menuController.AgregarProducto(_menuActual, productoId, cantidad);

            RegistrosMenuDataGrid.ItemsSource = null;
            RegistrosMenuDataGrid.ItemsSource = _menuActual.Registros.ToList();
            ActualizarTotalesMenu();

            CantidadProductoTextBox.Text = "";
            ProductosMenuComboBox.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            await MostrarMensaje(
                string.IsNullOrWhiteSpace(ex.Message) ? "Nada que guardar." : ex.Message
            );
        }
    }

    // Guarda el menú actual en la base de datos o repositorio.
    private async void GuardarMenu(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_menuActual == null)
            {
                await MostrarMensaje("No hay ningún menú para guardar.");
                return;
            }

            _menuActual.UsuarioId = ObtenerUsuarioIdSeleccionado();
            _menuActual.Fecha = FechaMenuDatePicker.SelectedDate?.Date ?? DateTime.Today;

            var existe = _menuController.ObtenerMenu(_menuActual.Id);

            if (existe == null)
                _menuController.CrearMenu(_menuActual);
            else
                _menuController.ActualizarMenu(_menuActual);

            CargarMenus();
            await MostrarMensaje("Menú guardado correctamente.");
            LimpiarMenu();
        }
        catch (Exception ex)
        {
            await MostrarMensaje(ex.Message);
        }
    }

    // Actualiza la cantidad de un registro dentro del menú.
    private async void ActualizarRegistroMenu(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_menuActual == null)
            {
                await MostrarMensaje("No hay menú activo.");
                return;
            }

            if (_registroSeleccionado == null)
            {
                await MostrarMensaje("Seleccione un registro del menú.");
                return;
            }

            var nuevaCantidad = decimal.TryParse(CantidadProductoTextBox.Text, out var cantidad)
                ? cantidad
                : 0;

            _menuController.ActualizarRegistro(_menuActual, _registroSeleccionado.Id, nuevaCantidad);

            RegistrosMenuDataGrid.ItemsSource = null;
            RegistrosMenuDataGrid.ItemsSource = _menuActual.Registros.ToList();

            ActualizarTotalesMenu();

            CantidadProductoTextBox.Text = "";
            _registroSeleccionado = null;
            ProductosMenuComboBox.SelectedIndex = 0;

            await MostrarMensaje("Cantidad actualizada correctamente.");
        }
        catch (Exception ex)
        {
            await MostrarMensaje(ex.Message);
        }
    }

    // Elimina un registro del menú actual.
    private async void EliminarRegistroMenu(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_menuActual == null)
            {
                await MostrarMensaje("No hay menú activo.");
                return;
            }

            if (_registroSeleccionado == null)
            {
                await MostrarMensaje("Seleccione un registro del menú.");
                return;
            }

            bool confirm = await Confirmar($"Eliminar {_registroSeleccionado.NombreProducto} del menú?");
            if (!confirm)
                return;

            _menuController.EliminarRegistro(_menuActual, _registroSeleccionado.Id);

            RegistrosMenuDataGrid.ItemsSource = null;
            RegistrosMenuDataGrid.ItemsSource = _menuActual.Registros.ToList();
            ActualizarTotalesMenu();

            CantidadProductoTextBox.Text = "";
            _registroSeleccionado = null;
            ProductosMenuComboBox.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            await MostrarMensaje(ex.Message);
        }
    }

    // Elimina el menú completo.
    private async void EliminarMenuCompleto(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_menuActual == null)
            {
                await MostrarMensaje("Seleccione un menú para eliminar.");
                return;
            }

            bool confirm = await Confirmar("¿Desea eliminar este menú completo?");
            if (!confirm)
                return;

            _menuController.EliminarMenu(_menuActual.Id);

            CargarMenus();
            LimpiarMenu();

            await MostrarMensaje("Menú eliminado correctamente.");
        }
        catch (Exception ex)
        {
            await MostrarMensaje(ex.Message);
        }
    }

    // Carga en pantalla el registro seleccionado del menú.
    private void RegistrosMenuDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (RegistrosMenuDataGrid.SelectedItem is RegistroComida registro)
        {
            _registroSeleccionado = registro;
            CantidadProductoTextBox.Text = registro.Cantidad.ToString();
        }
    }

    // Carga en pantalla un menú seleccionado del historial.
    private void MenusDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (MenusDataGrid.SelectedItem is MenuVista menuVista)
        {
            var menu = _menuController.ObtenerMenu(menuVista.Id);
            if (menu == null)
                return;

            if (!AuthSession.EsAdmin && menu.UsuarioId != AuthSession.UsuarioId)
                return;

            _menuActual = menu;

            SeleccionarUsuarioEnCombo(menu.UsuarioId);
            FechaMenuDatePicker.SelectedDate = menu.Fecha;

            RegistrosMenuDataGrid.ItemsSource = null;
            RegistrosMenuDataGrid.ItemsSource = menu.Registros.ToList();

            CantidadProductoTextBox.Text = "";
            _registroSeleccionado = null;
            ProductosMenuComboBox.SelectedIndex = 0;

            ActualizarTotalesMenu();
        }
    }

    // Carga los menús guardados en el DataGrid resumen.
    private void CargarMenus()
    {
        var usuarios = _usuarioController.ObtenerUsuarios();
        var menus = _menuController.ObtenerMenus();

        if (!AuthSession.EsAdmin)
            menus = menus.Where(m => m.UsuarioId == AuthSession.UsuarioId).ToList();

        var vista = menus.Select(m =>
        {
            var usuario = usuarios.FirstOrDefault(u => u.Id == m.UsuarioId);

            return new MenuVista
            {
                Id = m.Id,
                UsuarioNombre = usuario?.Nombre ?? "Usuario no encontrado",
                Fecha = m.Fecha,
                TotalCalorias = m.TotalCalorias,
                TotalProteinas = m.TotalProteinas,
                TotalCarbohidratos = m.TotalCarbohidratos,
                TotalGrasas = m.TotalGrasas
            };
        }).ToList();

        MenusDataGrid.ItemsSource = null;
        MenusDataGrid.ItemsSource = vista;
    }

    // Carga usuarios en el ComboBox del módulo menús.
    private void CargarUsuariosEnCombo()
    {
        var usuarios = _usuarioController.ObtenerUsuarios();

        if (!AuthSession.EsAdmin)
            usuarios = usuarios.Where(u => u.Id == AuthSession.UsuarioId).ToList();

        UsuariosMenuComboBox.Items.Clear();

        foreach (var usuario in usuarios)
        {
            UsuariosMenuComboBox.Items.Add(new ComboBoxItem
            {
                Content = usuario.Nombre,
                Tag = usuario.Id
            });
        }

        if (UsuariosMenuComboBox.ItemCount > 0)
            UsuariosMenuComboBox.SelectedIndex = 0;
    }

    // Carga productos en el ComboBox del módulo menús.
    private void CargarProductosEnCombo()
    {
        var productos = _productoController.ObtenerProductos();

        ProductosMenuComboBox.Items.Clear();

        ProductosMenuComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Producto",
            Tag = Guid.Empty,
            IsEnabled = false
        });

        foreach (var producto in productos)
        {
            ProductosMenuComboBox.Items.Add(new ComboBoxItem
            {
                Content = producto.Nombre,
                Tag = producto.Id
            });
        }

        ProductosMenuComboBox.SelectedIndex = 0;
    }

    // Devuelve el usuario seleccionado para trabajar con menús.
    private Guid ObtenerUsuarioIdSeleccionado()
    {
        if (!AuthSession.EsAdmin)
            return AuthSession.UsuarioId;

        if (UsuariosMenuComboBox.SelectedItem is ComboBoxItem item && item.Tag is Guid id)
            return id;

        return Guid.Empty;
    }

    // Devuelve el producto seleccionado para el menú.
    private Guid ObtenerProductoIdSeleccionado()
    {
        if (ProductosMenuComboBox.SelectedItem is ComboBoxItem item && item.Tag is Guid id)
            return id;

        return Guid.Empty;
    }

    // Selecciona el usuario del menú en el ComboBox.
    private void SeleccionarUsuarioEnCombo(Guid usuarioId)
    {
        foreach (var item in UsuariosMenuComboBox.Items)
        {
            if (item is ComboBoxItem comboItem && comboItem.Tag is Guid id && id == usuarioId)
            {
                UsuariosMenuComboBox.SelectedItem = comboItem;
                break;
            }
        }
    }

    // Actualiza en pantalla los totales nutricionales del menú actual.
    private void ActualizarTotalesMenu()
    {
        if (_menuActual == null)
        {
            TotalCaloriasTextBlock.Text = "Calorías: 0";
            TotalProteinasTextBlock.Text = "Proteínas: 0";
            TotalCarbohidratosTextBlock.Text = "Carbohidratos: 0";
            TotalGrasasTextBlock.Text = "Grasas: 0";
            return;
        }

        TotalCaloriasTextBlock.Text = $"Calorías: {_menuActual.TotalCalorias}";
        TotalProteinasTextBlock.Text = $"Proteínas: {_menuActual.TotalProteinas}";
        TotalCarbohidratosTextBlock.Text = $"Carbohidratos: {_menuActual.TotalCarbohidratos}";
        TotalGrasasTextBlock.Text = $"Grasas: {_menuActual.TotalGrasas}";
    }

    // Limpia el estado del módulo menús.
    private void LimpiarMenu()
    {
        _menuActual = null;
        _registroSeleccionado = null;

        UsuariosMenuComboBox.SelectedItem = null;
        CantidadProductoTextBox.Text = "";
        FechaMenuDatePicker.SelectedDate = DateTime.Today;

        RegistrosMenuDataGrid.ItemsSource = null;
        MenusDataGrid.SelectedItem = null;

        if (ProductosMenuComboBox.ItemCount > 0)
            ProductosMenuComboBox.SelectedIndex = 0;

        ActualizarTotalesMenu();

        if (UsuariosMenuComboBox.ItemCount > 0)
            UsuariosMenuComboBox.SelectedIndex = 0;
    }

    // =====================================================
    // PANEL ESTADÍSTICAS DE NUTRICIÓN
    // =====================================================

    // Carga usuarios en el ComboBox de estadísticas.
    private void CargarUsuariosEnComboEstadistica()
    {
        var usuarios = _usuarioController.ObtenerUsuarios();

        if (!AuthSession.EsAdmin)
            usuarios = usuarios.Where(u => u.Id == AuthSession.UsuarioId).ToList();

        UsuariosEstadisticaComboBox.Items.Clear();

        foreach (var usuario in usuarios)
        {
            UsuariosEstadisticaComboBox.Items.Add(new ComboBoxItem
            {
                Content = usuario.Nombre,
                Tag = usuario.Id
            });
        }

        if (UsuariosEstadisticaComboBox.ItemCount > 0)
            UsuariosEstadisticaComboBox.SelectedIndex = 0;

        FechaInicioEstadisticaDatePicker.SelectedDate = DateTime.Today;
        FechaFinEstadisticaDatePicker.SelectedDate = DateTime.Today;
    }

    // Carga dietas en el ComboBox de estadísticas.
    private void CargarDietasEnComboEstadistica()
    {
        var dietas = _estadisticasNutricionController.ObtenerDietasDisponibles();

        DietaEstadisticaComboBox.Items.Clear();

        foreach (var dieta in dietas)
        {
            DietaEstadisticaComboBox.Items.Add(new ComboBoxItem
            {
                Content = dieta.Nombre,
                Tag = dieta.Nombre
            });
        }

        if (!AuthSession.EsAdmin)
        {
            var usuario = _usuarioController.ObtenerUsuarios()
                .FirstOrDefault(u => u.Id == AuthSession.UsuarioId);

            if (usuario != null)
                SeleccionarDietaEstadistica(usuario.TipoDieta);
        }
        else if (DietaEstadisticaComboBox.ItemCount > 0)
        {
            DietaEstadisticaComboBox.SelectedIndex = 0;
        }
    }

    // Devuelve el usuario seleccionado para calcular estadísticas.
    private Guid ObtenerUsuarioIdEstadisticaSeleccionado()
    {
        if (!AuthSession.EsAdmin)
            return AuthSession.UsuarioId;

        if (UsuariosEstadisticaComboBox.SelectedItem is ComboBoxItem item && item.Tag is Guid id)
            return id;

        return Guid.Empty;
    }

    // Devuelve la dieta seleccionada para estadísticas.
    private string ObtenerDietaEstadisticaSeleccionada()
    {
        if (DietaEstadisticaComboBox.SelectedItem is ComboBoxItem item && item.Tag is string nombre)
            return nombre;

        return string.Empty;
    }

    // Selecciona una dieta en el ComboBox de estadísticas.
    private void SeleccionarDietaEstadistica(string tipoDieta)
    {
        foreach (var item in DietaEstadisticaComboBox.Items)
        {
            if (item is ComboBoxItem comboItem &&
                comboItem.Tag is string nombre &&
                nombre.Equals(tipoDieta, StringComparison.OrdinalIgnoreCase))
            {
                DietaEstadisticaComboBox.SelectedItem = comboItem;
                break;
            }
        }
    }

    // Si cambia el usuario en estadísticas, actualiza la dieta mostrada.
    private void UsuariosEstadisticaComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!AuthSession.EsAdmin)
            return;

        var usuarioId = ObtenerUsuarioIdEstadisticaSeleccionado();
        if (usuarioId == Guid.Empty)
            return;

        var usuario = _usuarioController.ObtenerUsuarios()
            .FirstOrDefault(u => u.Id == usuarioId);

        if (usuario == null || string.IsNullOrWhiteSpace(usuario.TipoDieta))
            return;

        SeleccionarDietaEstadistica(usuario.TipoDieta);
    }

    // Calcula y muestra el resumen nutricional del usuario
    // en el rango de fechas seleccionado.
    private async void CalcularEstadisticasNutricion(object? sender, RoutedEventArgs e)
    {
        try
        {
            var usuarioId = ObtenerUsuarioIdEstadisticaSeleccionado();
            if (usuarioId == Guid.Empty)
            {
                await MostrarMensaje("Seleccione un usuario.");
                return;
            }

            if (!AuthSession.EsAdmin && usuarioId != AuthSession.UsuarioId)
            {
                await MostrarMensaje("No tiene permiso para consultar estadísticas de otro usuario.");
                return;
            }

            var dieta = ObtenerDietaEstadisticaSeleccionada();
            if (string.IsNullOrWhiteSpace(dieta))
            {
                await MostrarMensaje("Seleccione un tipo de dieta.");
                return;
            }

            var fechaInicio = FechaInicioEstadisticaDatePicker.SelectedDate?.Date ?? DateTime.Today;
            var fechaFin = FechaFinEstadisticaDatePicker.SelectedDate?.Date ?? DateTime.Today;

            var resumen = _estadisticasNutricionController.ObtenerResumen(
                usuarioId,
                fechaInicio,
                fechaFin,
                dieta
            );

            EstadisticaNombreUsuarioTextBlock.Text = $"Usuario: {resumen.NombreUsuario}";
            EstadisticaTipoDietaTextBlock.Text = $"Dieta seleccionada: {resumen.TipoDieta}";
            EstadisticaFechaInicioTextBlock.Text = $"Fecha inicio: {resumen.FechaInicio:dd/MM/yyyy}";
            EstadisticaFechaFinTextBlock.Text = $"Fecha fin: {resumen.FechaFin:dd/MM/yyyy}";
            EstadisticaCantidadMenusTextBlock.Text = $"Cantidad de menús: {resumen.CantidadMenus}";

            EstadisticaImcTextBlock.Text = $"IMC: {resumen.Imc:F2}";
            EstadisticaCategoriaImcTextBlock.Text = $"Categoría IMC: {resumen.CategoriaImc}";

            EstadisticaCaloriasTotalesTextBlock.Text = $"Calorías totales: {resumen.CaloriasTotalesConsumidas:F2}";
            EstadisticaProteinasTotalesTextBlock.Text = $"Proteínas totales: {resumen.ProteinasTotalesConsumidas:F2}";
            EstadisticaCarbohidratosTotalesTextBlock.Text = $"Carbohidratos totales: {resumen.CarbohidratosTotalesConsumidos:F2}";
            EstadisticaGrasasTotalesTextBlock.Text = $"Grasas totales: {resumen.GrasasTotalesConsumidas:F2}";

            EstadisticaCaloriasPromedioTextBlock.Text = $"Calorías promedio: {resumen.CaloriasPromedio:F2}";
            EstadisticaProteinasPromedioTextBlock.Text = $"Proteínas promedio: {resumen.ProteinasPromedio:F2}";
            EstadisticaCarbohidratosPromedioTextBlock.Text = $"Carbohidratos promedio: {resumen.CarbohidratosPromedio:F2}";
            EstadisticaGrasasPromedioTextBlock.Text = $"Grasas promedio: {resumen.GrasasPromedio:F2}";

            EstadisticaCaloriasObjetivoTextBlock.Text = $"Calorías objetivo dieta: {resumen.CaloriasObjetivoDieta:F2}";
            EstadisticaProteinasObjetivoTextBlock.Text = $"Proteínas objetivo dieta: {resumen.ProteinasObjetivoDieta:F2}";
            EstadisticaCarbohidratosObjetivoTextBlock.Text = $"Carbohidratos objetivo dieta: {resumen.CarbohidratosObjetivoDieta:F2}";
            EstadisticaGrasasObjetivoTextBlock.Text = $"Grasas objetivo dieta: {resumen.GrasasObjetivoDieta:F2}";

            EstadisticaDiferenciaCaloriasTextBlock.Text = $"Diferencia calorías: {resumen.DiferenciaCalorias:F2}";
            EstadisticaDiferenciaProteinasTextBlock.Text = $"Diferencia proteínas: {resumen.DiferenciaProteinas:F2}";
            EstadisticaDiferenciaCarbohidratosTextBlock.Text = $"Diferencia carbohidratos: {resumen.DiferenciaCarbohidratos:F2}";
            EstadisticaDiferenciaGrasasTextBlock.Text = $"Diferencia grasas: {resumen.DiferenciaGrasas:F2}";

            EstadisticaEstadoCaloricoTextBlock.Text = $"Estado calórico: {resumen.EstadoCalorico}";
            EstadisticaRecomendacionTextBlock.Text = $"Recomendación: {resumen.Recomendacion}";

            ColorTextoMacro(EstadisticaDiferenciaCaloriasTextBlock, resumen.DiferenciaCalorias);
            ColorTextoMacro(EstadisticaDiferenciaProteinasTextBlock, resumen.DiferenciaProteinas);
            ColorTextoMacro(EstadisticaDiferenciaCarbohidratosTextBlock, resumen.DiferenciaCarbohidratos);
            ColorTextoMacro(EstadisticaDiferenciaGrasasTextBlock, resumen.DiferenciaGrasas);

            ColorTextoImc(EstadisticaImcTextBlock, resumen.CategoriaImc);
            ColorTextoImc(EstadisticaCategoriaImcTextBlock, resumen.CategoriaImc);

            if (resumen.EstadoCalorico.Contains("Por debajo", StringComparison.OrdinalIgnoreCase))
                EstadisticaEstadoCaloricoTextBlock.Foreground = Brushes.Orange;
            else if (resumen.EstadoCalorico.Contains("Dentro", StringComparison.OrdinalIgnoreCase))
                EstadisticaEstadoCaloricoTextBlock.Foreground = Brushes.Green;
            else
                EstadisticaEstadoCaloricoTextBlock.Foreground = Brushes.Red;
        }
        catch (Exception ex)
        {
            await MostrarMensaje(ex.Message);
        }
    }

    // Colorea el texto según la diferencia contra el objetivo.
    private void ColorTextoMacro(TextBlock textBlock, decimal diferencia)
    {
        if (Math.Abs(diferencia) <= 10m)
            textBlock.Foreground = Brushes.Green;
        else if (diferencia > 0)
            textBlock.Foreground = Brushes.Red;
        else
            textBlock.Foreground = Brushes.Orange;
    }

    // Colorea el texto del IMC según la categoría.
    private void ColorTextoImc(TextBlock textBlock, string categoriaImc)
    {
        switch ((categoriaImc ?? "").Trim().ToLowerInvariant())
        {
            case "normal":
                textBlock.Foreground = Brushes.Green;
                break;
            case "bajo peso":
            case "sobrepeso":
                textBlock.Foreground = Brushes.Orange;
                break;
            case "obesidad":
                textBlock.Foreground = Brushes.Red;
                break;
            default:
                textBlock.Foreground = Brushes.White;
                break;
        }
    }

    // Limpia todos los labels del resumen de estadísticas.
    private void LimpiarResumenEstadisticaNutricion()
    {
        EstadisticaNombreUsuarioTextBlock.Text = "Usuario: -";
        EstadisticaTipoDietaTextBlock.Text = "Dieta seleccionada: -";
        EstadisticaFechaInicioTextBlock.Text = "Fecha inicio: -";
        EstadisticaFechaFinTextBlock.Text = "Fecha fin: -";
        EstadisticaCantidadMenusTextBlock.Text = "Cantidad de menús: -";

        EstadisticaImcTextBlock.Text = "IMC: -";
        EstadisticaCategoriaImcTextBlock.Text = "Categoría IMC: -";

        EstadisticaCaloriasTotalesTextBlock.Text = "Calorías totales: -";
        EstadisticaProteinasTotalesTextBlock.Text = "Proteínas totales: -";
        EstadisticaCarbohidratosTotalesTextBlock.Text = "Carbohidratos totales: -";
        EstadisticaGrasasTotalesTextBlock.Text = "Grasas totales: -";

        EstadisticaCaloriasPromedioTextBlock.Text = "Calorías promedio: -";
        EstadisticaProteinasPromedioTextBlock.Text = "Proteínas promedio: -";
        EstadisticaCarbohidratosPromedioTextBlock.Text = "Carbohidratos promedio: -";
        EstadisticaGrasasPromedioTextBlock.Text = "Grasas promedio: -";

        EstadisticaCaloriasObjetivoTextBlock.Text = "Calorías objetivo dieta: -";
        EstadisticaProteinasObjetivoTextBlock.Text = "Proteínas objetivo dieta: -";
        EstadisticaCarbohidratosObjetivoTextBlock.Text = "Carbohidratos objetivo dieta: -";
        EstadisticaGrasasObjetivoTextBlock.Text = "Grasas objetivo dieta: -";

        EstadisticaDiferenciaCaloriasTextBlock.Text = "Diferencia calorías: -";
        EstadisticaDiferenciaProteinasTextBlock.Text = "Diferencia proteínas: -";
        EstadisticaDiferenciaCarbohidratosTextBlock.Text = "Diferencia carbohidratos: -";
        EstadisticaDiferenciaGrasasTextBlock.Text = "Diferencia grasas: -";

        EstadisticaEstadoCaloricoTextBlock.Text = "Estado calórico: -";
        EstadisticaRecomendacionTextBlock.Text = "Recomendación: -";

        EstadisticaEstadoCaloricoTextBlock.Foreground = Brushes.White;
        EstadisticaDiferenciaCaloriasTextBlock.Foreground = Brushes.White;
        EstadisticaDiferenciaProteinasTextBlock.Foreground = Brushes.White;
        EstadisticaDiferenciaCarbohidratosTextBlock.Foreground = Brushes.White;
        EstadisticaDiferenciaGrasasTextBlock.Foreground = Brushes.White;
        EstadisticaImcTextBlock.Foreground = Brushes.White;
        EstadisticaCategoriaImcTextBlock.Foreground = Brushes.White;
    }

    // =====================================================
    // UTILIDADES UI
    // =====================================================

    // Muestra una ventana modal simple con un mensaje.
    private async Task MostrarMensaje(string mensaje)
    {
        var ventana = new Window
        {
            Title = "Mensaje",
            Width = 380,
            Height = 180
        };

        var okButton = new Button
        {
            Content = "OK",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Width = 100
        };

        okButton.Click += (_, _) => ventana.Close();

        ventana.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 12,
            Children =
            {
                new TextBlock
                {
                    Text = mensaje,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                },
                okButton
            }
        };

        await ventana.ShowDialog(this);
    }

    // Muestra una ventana de confirmación y devuelve true o false.
    private async Task<bool> Confirmar(string mensaje)
    {
        var tcs = new TaskCompletionSource<bool>();

        var ventana = new Window
        {
            Title = "Confirmar",
            Width = 350,
            Height = 180
        };

        var si = new Button { Content = "Sí", Width = 100 };
        var no = new Button { Content = "No", Width = 100 };

        si.Click += (_, _) =>
        {
            tcs.TrySetResult(true);
            ventana.Close();
        };

        no.Click += (_, _) =>
        {
            tcs.TrySetResult(false);
            ventana.Close();
        };

        ventana.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 12,
            Children =
            {
                new TextBlock
                {
                    Text = mensaje,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                },
                new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    Spacing = 10,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    Children = { si, no }
                }
            }
        };

        ventana.Show();
        return await tcs.Task;
    }

    // =====================================================
    // CLASE AUXILIAR
    // =====================================================
    // Se usa para mostrar una vista resumida de los menús
    // en el DataGrid del historial.
    private class MenuVista
    {
        public Guid Id { get; set; }
        public string UsuarioNombre { get; set; } = "";
        public DateTime Fecha { get; set; }
        public decimal TotalCalorias { get; set; }
        public decimal TotalProteinas { get; set; }
        public decimal TotalCarbohidratos { get; set; }
        public decimal TotalGrasas { get; set; }
    }
}