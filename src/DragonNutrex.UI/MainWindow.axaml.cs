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
    private Usuario? _usuarioSeleccionado;

    public MainWindow()
    {
        InitializeComponent();

        var repo = new UsuarioRepository();
        var service = new UsuarioService(repo);
        _usuarioController = new UsuarioController(service);

        GuardarButton.Click += GuardarUsuario;
        EliminarButton.Click += EliminarUsuario;
        LimpiarButton.Click += LimpiarFormularioClick;
        UsuariosDataGrid.SelectionChanged += UsuariosDataGrid_SelectionChanged;

        UsuariosModuloButton.Click += UsuariosModuloClick;
        ProductosModuloButton.Click += ProductosModuloClick;
        MenusModuloButton.Click += MenusModuloClick;
        NutricionModuloButton.Click += NutricionModuloClick;
        EstadisticasModuloButton.Click += EstadisticasModuloClick;

        CargarUsuarios();
    }

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
            await MostrarMensaje($"Error al guardar: {ex.Message}");
        }
    }

    private async void EliminarUsuario(object? sender, RoutedEventArgs e)
    {
        if (UsuariosDataGrid.SelectedItem is not Usuario usuario)
        {
            await MostrarMensaje("Seleccione un usuario para eliminar.");
            return;
        }

        bool confirmar = await MostrarConfirmacion($"¿Seguro que deseas eliminar a {usuario.Nombre}?");

        if (!confirmar)
            return;

        try
        {
            _usuarioController.EliminarUsuario(usuario.Id);
            LimpiarFormulario();
            CargarUsuarios();
            await MostrarMensaje("Usuario eliminado correctamente.");
        }
        catch (Exception ex)
        {
            await MostrarMensaje($"Error al eliminar: {ex.Message}");
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
        List<Usuario> usuarios = _usuarioController.ObtenerUsuarios();
        UsuariosDataGrid.ItemsSource = null;
        UsuariosDataGrid.ItemsSource = usuarios;
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

        UsuariosDataGrid.SelectedItem = null;
        GuardarButton.Content = "Guardar";
    }

    private void LimpiarFormularioClick(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
    }

    private async void UsuariosModuloClick(object? sender, RoutedEventArgs e)
    {
        await MostrarMensaje("Módulo de Usuarios activo.");
    }

    private async void ProductosModuloClick(object? sender, RoutedEventArgs e)
    {
        await MostrarMensaje("Placeholder: aquí irá el módulo de Productos.");
    }

    private async void MenusModuloClick(object? sender, RoutedEventArgs e)
    {
        await MostrarMensaje("Placeholder: aquí irá el módulo de Menús.");
    }

    private async void NutricionModuloClick(object? sender, RoutedEventArgs e)
    {
        await MostrarMensaje("Placeholder: aquí irá el módulo de Nutrición.");
    }

    private async void EstadisticasModuloClick(object? sender, RoutedEventArgs e)
    {
        await MostrarMensaje("Placeholder: aquí irá el módulo de Estadísticas.");
    }

    private async Task MostrarMensaje(string mensaje)
    {
        var ventana = new Window
        {
            Title = "Mensaje",
            Width = 400,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var texto = new TextBlock
        {
            Text = mensaje,
            TextWrapping = TextWrapping.Wrap
        };

        var botonCerrar = new Button
        {
            Content = "Cerrar",
            Width = 100,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        botonCerrar.Click += (_, _) => ventana.Close();

        ventana.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 15,
            Children =
            {
                texto,
                botonCerrar
            }
        };

        await ventana.ShowDialog(this);
    }

    private async Task<bool> MostrarConfirmacion(string mensaje)
    {
        bool resultado = false;

        var ventana = new Window
        {
            Title = "Confirmación",
            Width = 420,
            Height = 190,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var texto = new TextBlock
        {
            Text = mensaje,
            TextWrapping = TextWrapping.Wrap
        };

        var botonSi = new Button
        {
            Content = "Sí",
            Width = 100
        };

        var botonNo = new Button
        {
            Content = "No",
            Width = 100
        };

        botonSi.Click += (_, _) =>
        {
            resultado = true;
            ventana.Close();
        };

        botonNo.Click += (_, _) =>
        {
            resultado = false;
            ventana.Close();
        };

        ventana.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 15,
            Children =
            {
                texto,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        botonSi,
                        botonNo
                    }
                }
            }
        };

        await ventana.ShowDialog(this);
        return resultado;
    }
}