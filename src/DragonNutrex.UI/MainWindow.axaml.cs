using Avalonia.Controls;
using Avalonia.Interactivity;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Models;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;
using System;
using System.Linq;

namespace DragonNutrex.UI;

public partial class MainWindow : Window
{
    private readonly UsuarioController _usuarioController;

    public MainWindow()
    {
        InitializeComponent();

        var repo = new UsuarioRepository();
        var service = new UsuarioService(repo);
        _usuarioController = new UsuarioController(service);

        GuardarButton.Click += GuardarUsuario;

        CargarUsuarios();
    }

    private void GuardarUsuario(object? sender, RoutedEventArgs e)
    {
        try
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

            LimpiarFormulario();
            CargarUsuarios();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void CargarUsuarios()
    {
        var usuarios = _usuarioController.ObtenerUsuarios();

        UsuariosListBox.ItemsSource = usuarios
            .Select(u => $"{u.Nombre} - {u.Peso}kg - {u.Altura}m")
            .ToList();
    }

    private void LimpiarFormulario()
    {
        NombreTextBox.Text = "";
        PesoTextBox.Text = "";
        AlturaTextBox.Text = "";
        ActividadTextBox.Text = "";
        ObjetivoTextBox.Text = "";
        TipoDietaTextBox.Text = "";
    }
}