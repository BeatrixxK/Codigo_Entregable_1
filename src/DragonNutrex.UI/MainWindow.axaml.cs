using Avalonia.Controls;
using Avalonia.Interactivity;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Models;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;
using System;
using System.Collections.Generic;

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
        try
        {
            if (UsuariosDataGrid.SelectedItem is Usuario usuario)
            {
                _usuarioController.EliminarUsuario(usuario.Id);
                LimpiarFormulario();
                CargarUsuarios();
                await MostrarMensaje("Usuario eliminado correctamente.");
            }
            else
            {
                await MostrarMensaje("Seleccione un usuario para eliminar.");
            }
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

    private async System.Threading.Tasks.Task MostrarMensaje(string mensaje)
    {
        var ventana = new Window
        {
            Title = "Mensaje",
            Width = 420,
            Height = 180,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 15,
                Children =
                {
                    new TextBlock
                    {
                        Text = mensaje,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    },
                    new Button
                    {
                        Name = "CerrarButton",
                        Content = "Cerrar",
                        Width = 100,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                    }
                }
            }
        };

        if (ventana.Content is StackPanel panel && panel.Children[1] is Button boton)
        {
            boton.Click += (_, _) => ventana.Close();
        }

        await ventana.ShowDialog(this);
    }
}