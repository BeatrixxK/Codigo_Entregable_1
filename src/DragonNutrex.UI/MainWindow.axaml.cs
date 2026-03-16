using System;
using System.Collections.Generic;
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

    private void GuardarUsuario(object? sender, RoutedEventArgs e)
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
            }

            LimpiarFormulario();
            CargarUsuarios();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void EliminarUsuario(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (UsuariosDataGrid.SelectedItem is Usuario usuario)
            {
                _usuarioController.EliminarUsuario(usuario.Id);
                LimpiarFormulario();
                CargarUsuarios();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
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
}