using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Models;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;

namespace DragonNutrex.UI;

public partial class MainWindow : Window
{
    private readonly UsuarioController _usuarioController;
    private readonly ProductoController _productoController;

    private Usuario? _usuarioSeleccionado;
    private Producto? _productoSeleccionado;

    public MainWindow()
    {
        InitializeComponent();

        var usuarioRepo = new UsuarioRepository();
        var usuarioService = new UsuarioService(usuarioRepo);
        _usuarioController = new UsuarioController(usuarioService);

        var productoRepo = new ProductoRepository();
        var productoService = new ProductoService(productoRepo);
        _productoController = new ProductoController(productoService);

        // Usuarios
        GuardarButton.Click += GuardarUsuario;
        EliminarButton.Click += EliminarUsuario;
        LimpiarButton.Click += LimpiarFormularioClick;
        UsuariosDataGrid.SelectionChanged += UsuariosDataGrid_SelectionChanged;

        // Productos
        GuardarProductoButton.Click += GuardarProducto;
        EliminarProductoButton.Click += EliminarProducto;
        LimpiarProductoButton.Click += LimpiarProductoFormularioClick;
        ProductosDataGrid.SelectionChanged += ProductosDataGrid_SelectionChanged;

        // Navegación
        UsuariosModuloButton.Click += UsuariosModuloClick;
        ProductosModuloButton.Click += ProductosModuloClick;
        MenusModuloButton.Click += MenusModuloClick;
        NutricionModuloButton.Click += NutricionModuloClick;
        EstadisticasModuloButton.Click += EstadisticasModuloClick;

        CargarUsuarios();
        CargarProductos();
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
                var nuevoUsuario = new Usuario
                {
                    Nombre = NombreTextBox.Text ?? "",
                    Peso = decimal.Parse(PesoTextBox.Text ?? "0"),
                    Altura = decimal.Parse(AlturaTextBox.Text ?? "0"),
                    Actividad = ActividadTextBox.Text ?? "",
                    Objetivo = ObjetivoTextBox.Text ?? "",
                    TipoDieta = TipoDietaTextBox.Text ?? ""
                };

                _usuarioController.CrearUsuario(nuevoUsuario);
                await MostrarMensaje("Usuario guardado correctamente.");
            }
            else
            {
                _usuarioSeleccionado.Nombre = NombreTextBox.Text ?? "";
                _usuarioSeleccionado.Peso = decimal.Parse(PesoTextBox.Text ?? "0");
                _usuarioSeleccionado.Altura = decimal.Parse(AlturaTextBox.Text ?? "0");
                _usuarioSeleccionado.Actividad = ActividadTextBox.Text ?? "";
                _usuarioSeleccionado.Objetivo = ObjetivoTextBox.Text ?? "";
                _usuarioSeleccionado.TipoDieta = TipoDietaTextBox.Text ?? "";

                _usuarioController.ActualizarUsuario(_usuarioSeleccionado);
                await MostrarMensaje("Usuario actualizado correctamente.");
            }

            LimpiarFormulario();
            CargarUsuarios();
        }
        catch (Exception ex)
        {
            await MostrarMensaje($"Error: {ex.Message}");
        }
    }

    private async void EliminarUsuario(object? sender, RoutedEventArgs e)
    {
        if (UsuariosDataGrid.SelectedItem is not Usuario usuario)
        {
            await MostrarMensaje("Seleccione un usuario.");
            return;
        }

        bool confirmar = await MostrarConfirmacion($"¿Eliminar a {usuario.Nombre}?");

        if (!confirmar)
            return;

        _usuarioController.EliminarUsuario(usuario.Id);
        LimpiarFormulario();
        CargarUsuarios();
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

    private void LimpiarFormulario()
    {
        _usuarioSeleccionado = null;

        NombreTextBox.Text = "";
        PesoTextBox.Text = "";
        AlturaTextBox.Text = "";
        ActividadTextBox.Text = "";
        ObjetivoTextBox.Text = "";
        TipoDietaTextBox.Text = "";

        GuardarButton.Content = "Guardar";
    }

    private void LimpiarFormularioClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
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
                    Calorias = decimal.Parse(CaloriasTextBox.Text ?? "0"),
                    Proteinas = decimal.Parse(ProteinasTextBox.Text ?? "0"),
                    Carbohidratos = decimal.Parse(CarbohidratosTextBox.Text ?? "0"),
                    Grasas = decimal.Parse(GrasasTextBox.Text ?? "0")
                };

                _productoController.CrearProducto(producto);
                await MostrarMensaje("Producto guardado.");
            }
            else
            {
                _productoSeleccionado.Nombre = ProductoNombreTextBox.Text ?? "";
                _productoSeleccionado.Calorias = decimal.Parse(CaloriasTextBox.Text ?? "0");
                _productoSeleccionado.Proteinas = decimal.Parse(ProteinasTextBox.Text ?? "0");
                _productoSeleccionado.Carbohidratos = decimal.Parse(CarbohidratosTextBox.Text ?? "0");
                _productoSeleccionado.Grasas = decimal.Parse(GrasasTextBox.Text ?? "0");

                _productoController.ActualizarProducto(_productoSeleccionado);
                await MostrarMensaje("Producto actualizado.");
            }

            LimpiarProductoFormulario();
            CargarProductos();
        }
        catch (Exception ex)
        {
            await MostrarMensaje($"Error: {ex.Message}");
        }
    }

    private async void EliminarProducto(object? sender, RoutedEventArgs e)
    {
        if (ProductosDataGrid.SelectedItem is not Producto producto)
        {
            await MostrarMensaje("Seleccione un producto.");
            return;
        }

        bool confirmar = await MostrarConfirmacion($"¿Eliminar {producto.Nombre}?");

        if (!confirmar)
            return;

        _productoController.EliminarProducto(producto.Id);
        LimpiarProductoFormulario();
        CargarProductos();
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

    private void LimpiarProductoFormulario()
    {
        _productoSeleccionado = null;

        ProductoNombreTextBox.Text = "";
        CaloriasTextBox.Text = "";
        ProteinasTextBox.Text = "";
        CarbohidratosTextBox.Text = "";
        GrasasTextBox.Text = "";

        GuardarProductoButton.Content = "Guardar Producto";
    }

    private void LimpiarProductoFormularioClick(object? sender, RoutedEventArgs e)
    {
        LimpiarProductoFormulario();
    }

    // =========================
    // NAVEGACIÓN
    // =========================

    private void UsuariosModuloClick(object? sender, RoutedEventArgs e)
    {
        UsuariosPanel.IsVisible = true;
        ProductosPanel.IsVisible = false;
    }

    private void ProductosModuloClick(object? sender, RoutedEventArgs e)
    {
        UsuariosPanel.IsVisible = false;
        ProductosPanel.IsVisible = true;
    }

    private async void MenusModuloClick(object? sender, RoutedEventArgs e)
    {
        await MostrarMensaje("Módulo Menús próximamente.");
    }

    private async void NutricionModuloClick(object? sender, RoutedEventArgs e)
    {
        await MostrarMensaje("Módulo Nutrición próximamente.");
    }

    private async void EstadisticasModuloClick(object? sender, RoutedEventArgs e)
    {
        await MostrarMensaje("Módulo Estadísticas próximamente.");
    }

    // =========================
    // UTILIDADES UI
    // =========================

    private async Task MostrarMensaje(string mensaje)
    {
        var ventana = new Window
        {
            Title = "Mensaje",
            Width = 300,
            Height = 150,
            Content = new TextBlock
            {
                Text = mensaje,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        await ventana.ShowDialog(this);
    }

    private async Task<bool> MostrarConfirmacion(string mensaje)
    {
        var tcs = new TaskCompletionSource<bool>();

        var ventana = new Window
        {
            Title = "Confirmar",
            Width = 350,
            Height = 180
        };

        var si = new Button { Content = "Sí", Width = 80 };
        var no = new Button { Content = "No", Width = 80 };

        si.Click += (_, _) => { tcs.SetResult(true); ventana.Close(); };
        no.Click += (_, _) => { tcs.SetResult(false); ventana.Close(); };

        ventana.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Children =
            {
                new TextBlock { Text = mensaje },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10,
                    Children = { si, no }
                }
            }
        };

        ventana.Show();
        return await tcs.Task;
    }
}