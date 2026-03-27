using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Models;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;
using MenuModel = DragonNutrex.App.Models.Menu;

namespace DragonNutrex.UI;

public partial class MainWindow : Window
{
    private readonly UsuarioController _usuarioController;
    private readonly ProductoController _productoController;
    private readonly MenuController _menuController;

    private Usuario? _usuarioSeleccionado;
    private Producto? _productoSeleccionado;
    private MenuModel? _menuActual;
    private RegistroComida? _registroSeleccionado;

    public MainWindow()
    {
        InitializeComponent();

        _usuarioController = new UsuarioController(new UsuarioService(new UsuarioRepository()));
        _productoController = new ProductoController(new ProductoService(new ProductoRepository()));
        _menuController = new MenuController(
            new MenuService(
                new MenuRepository(),
                new UsuarioRepository(),
                new ProductoRepository()
            )
        );

        GuardarButton.Click += GuardarUsuario;
        EliminarButton.Click += EliminarUsuario;
        LimpiarButton.Click += (_, _) => LimpiarUsuario();

        GuardarProductoButton.Click += GuardarProducto;
        EliminarProductoButton.Click += EliminarProducto;
        LimpiarProductoButton.Click += (_, _) => LimpiarProducto();

        UsuariosDataGrid.SelectionChanged += UsuariosDataGrid_SelectionChanged;
        ProductosDataGrid.SelectionChanged += ProductosDataGrid_SelectionChanged;

        UsuariosModuloButton.Click += (_, _) =>
        {
            UsuariosPanel.IsVisible = true;
            ProductosPanel.IsVisible = false;
            MenusPanel.IsVisible = false;
        };

        ProductosModuloButton.Click += (_, _) =>
        {
            UsuariosPanel.IsVisible = false;
            ProductosPanel.IsVisible = true;
            MenusPanel.IsVisible = false;
        };

        MenusModuloButton.Click += (_, _) =>
        {
            UsuariosPanel.IsVisible = false;
            ProductosPanel.IsVisible = false;
            MenusPanel.IsVisible = true;

            CargarUsuariosEnCombo();
            CargarProductosEnCombo();
            CargarMenus();
        };

        NutricionModuloButton.Click += async (_, _) => await MostrarMensaje("Módulo Nutrición próximamente.");
        EstadisticasModuloButton.Click += async (_, _) => await MostrarMensaje("Módulo Estadísticas próximamente.");

        NuevoMenuButton.Click += NuevoMenu;
        AgregarProductoMenuButton.Click += AgregarProductoAlMenu;
        GuardarMenuButton.Click += GuardarMenu;
        ActualizarRegistroMenuButton.Click += ActualizarRegistroMenu;
        EliminarRegistroMenuButton.Click += EliminarRegistroMenu;
        EliminarMenuButton.Click += EliminarMenuCompleto;
        LimpiarMenuButton.Click += (_, _) => LimpiarMenu();

        RegistrosMenuDataGrid.SelectionChanged += RegistrosMenuDataGrid_SelectionChanged;
        MenusDataGrid.SelectionChanged += MenusDataGrid_SelectionChanged;

        CargarUsuarios();
        CargarProductos();
        CargarUsuariosEnCombo();
        CargarProductosEnCombo();
        CargarMenus();
        LimpiarMenu();
    }

    // =========================
    // USUARIOS
    // =========================

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
                    Actividad = ActividadTextBox.Text ?? "",
                    Objetivo = ObjetivoTextBox.Text ?? "",
                    TipoDieta = TipoDietaTextBox.Text ?? ""
                };

                _usuarioController.CrearUsuario(usuario);
            }
            else
            {
                _usuarioSeleccionado.Nombre = NombreTextBox.Text ?? "";
                _usuarioSeleccionado.Peso = decimal.TryParse(PesoTextBox.Text, out var peso) ? peso : 0;
                _usuarioSeleccionado.Altura = decimal.TryParse(AlturaTextBox.Text, out var altura) ? altura : 0;
                _usuarioSeleccionado.Actividad = ActividadTextBox.Text ?? "";
                _usuarioSeleccionado.Objetivo = ObjetivoTextBox.Text ?? "";
                _usuarioSeleccionado.TipoDieta = TipoDietaTextBox.Text ?? "";

                _usuarioController.ActualizarUsuario(_usuarioSeleccionado);
            }

            CargarUsuarios();
            CargarUsuariosEnCombo();
            LimpiarUsuario();
        }
        catch (Exception ex)
        {
            await MostrarMensaje(string.IsNullOrWhiteSpace(ex.Message) ? "Nada que guardar." : ex.Message);
        }
    }

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
            LimpiarUsuario();
        }
    }

    private void UsuariosDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (UsuariosDataGrid.SelectedItem is Usuario usuario)
        {
            _usuarioSeleccionado = usuario;

            NombreTextBox.Text = usuario.Nombre;
            PesoTextBox.Text = usuario.Peso.ToString();
            AlturaTextBox.Text = usuario.Altura.ToString();
            ActividadTextBox.Text = usuario.Actividad;
            ObjetivoTextBox.Text = usuario.Objetivo;
            TipoDietaTextBox.Text = usuario.TipoDieta;

            GuardarButton.Content = "Actualizar";
        }
    }

    private void CargarUsuarios()
    {
        UsuariosDataGrid.ItemsSource = null;
        UsuariosDataGrid.ItemsSource = _usuarioController.ObtenerUsuarios();
    }

    private void LimpiarUsuario()
    {
        _usuarioSeleccionado = null;

        NombreTextBox.Text = "";
        PesoTextBox.Text = "";
        AlturaTextBox.Text = "";
        ActividadTextBox.Text = "";
        ObjetivoTextBox.Text = "";
        TipoDietaTextBox.Text = "";

        UsuariosDataGrid.SelectedItem = null;
        GuardarButton.Content = "Guardar";
    }

    // =========================
    // PRODUCTOS
    // =========================

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
            await MostrarMensaje(string.IsNullOrWhiteSpace(ex.Message) ? "Nada que guardar." : ex.Message);
        }
    }

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

    private void CargarProductos()
    {
        ProductosDataGrid.ItemsSource = null;
        ProductosDataGrid.ItemsSource = _productoController.ObtenerProductos();
    }

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

    // =========================
    // MENUS
    // =========================

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
        }
        catch (Exception ex)
        {
            await MostrarMensaje(string.IsNullOrWhiteSpace(ex.Message) ? "Nada que guardar." : ex.Message);
        }
    }

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

            await MostrarMensaje("Cantidad actualizada correctamente.");
        }
        catch (Exception ex)
        {
            await MostrarMensaje(ex.Message);
        }
    }

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
        }
        catch (Exception ex)
        {
            await MostrarMensaje(ex.Message);
        }
    }

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

    private void RegistrosMenuDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (RegistrosMenuDataGrid.SelectedItem is RegistroComida registro)
        {
            _registroSeleccionado = registro;
            CantidadProductoTextBox.Text = registro.Cantidad.ToString();
        }
    }

    private void MenusDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (MenusDataGrid.SelectedItem is MenuVista menuVista)
        {
            var menu = _menuController.ObtenerMenu(menuVista.Id);
            if (menu == null)
                return;

            _menuActual = menu;

            SeleccionarUsuarioEnCombo(menu.UsuarioId);
            FechaMenuDatePicker.SelectedDate = menu.Fecha;

            RegistrosMenuDataGrid.ItemsSource = null;
            RegistrosMenuDataGrid.ItemsSource = menu.Registros.ToList();

            CantidadProductoTextBox.Text = "";
            _registroSeleccionado = null;

            ActualizarTotalesMenu();
        }
    }

    private void CargarMenus()
    {
        var usuarios = _usuarioController.ObtenerUsuarios();
        var menus = _menuController.ObtenerMenus();

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

    private void CargarUsuariosEnCombo()
    {
        var usuarios = _usuarioController.ObtenerUsuarios();

        UsuariosMenuComboBox.Items.Clear();

        foreach (var usuario in usuarios)
        {
            UsuariosMenuComboBox.Items.Add(new ComboBoxItem
            {
                Content = usuario.Nombre,
                Tag = usuario.Id
            });
        }
    }

    private void CargarProductosEnCombo()
    {
        var productos = _productoController.ObtenerProductos();

        ProductosMenuComboBox.Items.Clear();

        foreach (var producto in productos)
        {
            ProductosMenuComboBox.Items.Add(new ComboBoxItem
            {
                Content = producto.Nombre,
                Tag = producto.Id
            });
        }
    }

    private Guid ObtenerUsuarioIdSeleccionado()
    {
        if (UsuariosMenuComboBox.SelectedItem is ComboBoxItem item && item.Tag is Guid id)
            return id;

        return Guid.Empty;
    }

    private Guid ObtenerProductoIdSeleccionado()
    {
        if (ProductosMenuComboBox.SelectedItem is ComboBoxItem item && item.Tag is Guid id)
            return id;

        return Guid.Empty;
    }

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

    private void LimpiarMenu()
    {
        _menuActual = null;
        _registroSeleccionado = null;

        UsuariosMenuComboBox.SelectedItem = null;
        ProductosMenuComboBox.SelectedItem = null;
        CantidadProductoTextBox.Text = "";
        FechaMenuDatePicker.SelectedDate = DateTime.Today;

        RegistrosMenuDataGrid.ItemsSource = null;
        MenusDataGrid.SelectedItem = null;

        ActualizarTotalesMenu();
    }

    // =========================
    // UTILIDADES UI
    // =========================

    private async Task MostrarMensaje(string mensaje)
    {
        var ventana = new Window
        {
            Title = "Mensaje",
            Width = 320,
            Height = 160,
            Content = new TextBlock
            {
                Text = mensaje,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            }
        };

        await ventana.ShowDialog(this);
    }

    private async Task<bool> Confirmar(string mensaje)
    {
        var tcs = new TaskCompletionSource<bool>();

        var ventana = new Window
        {
            Title = "Confirmar",
            Width = 350,
            Height = 180
        };

        var si = new Button { Content = "Sí" };
        var no = new Button { Content = "No" };

        si.Click += (_, _) =>
        {
            tcs.SetResult(true);
            ventana.Close();
        };

        no.Click += (_, _) =>
        {
            tcs.SetResult(false);
            ventana.Close();
        };

        ventana.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
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
                    Children = { si, no }
                }
            }
        };

        ventana.Show();
        return await tcs.Task;
    }

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