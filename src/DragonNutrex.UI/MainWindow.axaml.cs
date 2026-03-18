using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
<<<<<<< Updated upstream
        try
        {
            InitializeComponent();

            _usuarioController = new UsuarioController(new UsuarioService(new UsuarioRepository()));
            _productoController = new ProductoController(new ProductoService(new ProductoRepository()));

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
            };

            ProductosModuloButton.Click += (_, _) =>
            {
                UsuariosPanel.IsVisible = false;
                ProductosPanel.IsVisible = true;
            };

            MenusModuloButton.Click += async (_, _) => await MostrarMensaje("Módulo Menús próximamente.");
            NutricionModuloButton.Click += async (_, _) => await MostrarMensaje("Módulo Nutrición próximamente.");
            EstadisticasModuloButton.Click += async (_, _) => await MostrarMensaje("Módulo Estadísticas próximamente.");

            CargarUsuarios();
            CargarProductos();
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR AL INICIAR:");
            Console.WriteLine(ex.ToString());
            throw;
        }
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
        LimpiarUsuario();
    }
    catch (Exception ex)
    {
        await MostrarMensaje(ex.Message);
    }
}
=======
        InitializeComponent();

        _usuarioController = new UsuarioController(new UsuarioService(new UsuarioRepository()));
        _productoController = new ProductoController(new ProductoService(new ProductoRepository()));

        GuardarButton.Click += GuardarUsuario;
        EliminarButton.Click += EliminarUsuario;
        LimpiarButton.Click += (_, _) => LimpiarUsuario();

        GuardarProductoButton.Click += GuardarProducto;
        EliminarProductoButton.Click += EliminarProducto;
        LimpiarProductoButton.Click += (_, _) => LimpiarProducto();

        UsuariosModuloButton.Click += (_, _) =>
        {
            UsuariosPanel.IsVisible = true;
            ProductosPanel.IsVisible = false;
        };

        ProductosModuloButton.Click += (_, _) =>
        {
            UsuariosPanel.IsVisible = false;
            ProductosPanel.IsVisible = true;
        };

        CargarUsuarios();
        CargarProductos();
    }

    // ================= USUARIOS =================

    private void GuardarUsuario(object? sender, RoutedEventArgs e)
    {
        var usuario = new Usuario
        {
            Nombre = NombreTextBox.Text ?? "",
            Peso = decimal.Parse(PesoTextBox.Text ?? "0"),
            Altura = decimal.Parse(AlturaTextBox.Text ?? "0"),
            Actividad = ActividadTextBox.Text ?? "",
            Objetivo = ObjetivoTextBox.Text ?? "",
            TipoDieta = TipoDietaTextBox.Text ?? ""
        };

        _usuarioController.CrearUsuario(usuario);
        CargarUsuarios();
        LimpiarUsuario();
    }
>>>>>>> Stashed changes

    private async void EliminarUsuario(object? sender, RoutedEventArgs e)
    {
        if (UsuariosDataGrid.SelectedItem is Usuario usuario)
        {
            bool confirm = await Confirmar($"Eliminar a {usuario.Nombre}?");
            if (!confirm) return;

            _usuarioController.EliminarUsuario(usuario.Id);
            CargarUsuarios();
<<<<<<< Updated upstream
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
=======
>>>>>>> Stashed changes
        }
    }

    private void CargarUsuarios()
    {
<<<<<<< Updated upstream
        UsuariosDataGrid.ItemsSource = null;
=======
>>>>>>> Stashed changes
        UsuariosDataGrid.ItemsSource = _usuarioController.ObtenerUsuarios();
    }

    private void LimpiarUsuario()
    {
<<<<<<< Updated upstream
        _usuarioSeleccionado = null;

=======
>>>>>>> Stashed changes
        NombreTextBox.Text = "";
        PesoTextBox.Text = "";
        AlturaTextBox.Text = "";
        ActividadTextBox.Text = "";
        ObjetivoTextBox.Text = "";
        TipoDietaTextBox.Text = "";
<<<<<<< Updated upstream

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
        LimpiarProducto();
    }
    catch (Exception ex)
    {
        await MostrarMensaje(ex.Message);
    }
}
=======
    }

    // ================= PRODUCTOS =================

    private void GuardarProducto(object? sender, RoutedEventArgs e)
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
        CargarProductos();
        LimpiarProducto();
    }
>>>>>>> Stashed changes

    private async void EliminarProducto(object? sender, RoutedEventArgs e)
    {
        if (ProductosDataGrid.SelectedItem is Producto producto)
        {
            bool confirm = await Confirmar($"Eliminar {producto.Nombre}?");
            if (!confirm) return;

            _productoController.EliminarProducto(producto.Id);
            CargarProductos();
<<<<<<< Updated upstream
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
=======
>>>>>>> Stashed changes
        }
    }

    private void CargarProductos()
    {
<<<<<<< Updated upstream
        ProductosDataGrid.ItemsSource = null;
=======
>>>>>>> Stashed changes
        ProductosDataGrid.ItemsSource = _productoController.ObtenerProductos();
    }

    private void LimpiarProducto()
    {
<<<<<<< Updated upstream
        _productoSeleccionado = null;

=======
>>>>>>> Stashed changes
        ProductoNombreTextBox.Text = "";
        CaloriasTextBox.Text = "";
        ProteinasTextBox.Text = "";
        CarbohidratosTextBox.Text = "";
        GrasasTextBox.Text = "";
<<<<<<< Updated upstream

        ProductosDataGrid.SelectedItem = null;
        GuardarProductoButton.Content = "Guardar Producto";
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
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
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
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    Spacing = 10,
                    Children = { si, no }
                }
            }
        };

        ventana.Show();
        return await tcs.Task;
=======
    }

    // ================= CONFIRMACIÓN =================

    private async Task<bool> Confirmar(string mensaje)
    {
        var dialog = new Window
        {
            Title = "Confirmación",
            Width = 300,
            Height = 150
        };

        bool resultado = false;

        var si = new Button { Content = "Sí" };
        var no = new Button { Content = "No" };

        si.Click += (_, _) => { resultado = true; dialog.Close(); };
        no.Click += (_, _) => { resultado = false; dialog.Close(); };

        dialog.Content = new StackPanel
        {
            Children =
            {
                new TextBlock { Text = mensaje },
                new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Children = { si, no } }
            }
        };

        await dialog.ShowDialog(this);
        return resultado;
>>>>>>> Stashed changes
    }
}