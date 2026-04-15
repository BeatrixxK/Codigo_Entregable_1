using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;

namespace DragonNutrex.UI;

public partial class CambiarPasswordWindow : Window
{
    private readonly UsuarioController _usuarioController;

    public CambiarPasswordWindow()
    {
        InitializeComponent();

        _usuarioController = new UsuarioController(
          new UsuarioService(
            new UsuarioRedisRepository(AppServices.Redis)
            )
        );

        var cancelarBtn = this.FindControl<Button>("CancelarButton");
        if (cancelarBtn != null)
            cancelarBtn.Click += CancelarButton_Click;

        var guardarBtn = this.FindControl<Button>("GuardarPasswordButton");
        if (guardarBtn != null)
            guardarBtn.Click += GuardarPasswordButton_Click;
    }

    private void CancelarButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void GuardarPasswordButton_Click(object? sender, RoutedEventArgs e)
    {
        var usuarioIngresado = this.FindControl<TextBox>("UsuarioRecuperarTextBox")?.Text?.Trim();
        var nuevaPass = this.FindControl<TextBox>("NuevaPasswordTextBox")?.Text;
        var confirmarPass = this.FindControl<TextBox>("ConfirmarPasswordTextBox")?.Text;
        var mensajeError = this.FindControl<TextBlock>("MensajeErrorTextBlock");

        if (string.IsNullOrWhiteSpace(usuarioIngresado) ||
            string.IsNullOrWhiteSpace(nuevaPass) ||
            string.IsNullOrWhiteSpace(confirmarPass))
        {
            MostrarError(mensajeError, "Todos los campos son obligatorios.");
            return;
        }

        if (nuevaPass != confirmarPass)
        {
            MostrarError(mensajeError, "Las contraseñas no coinciden.");
            return;
        }

        var usuario = _usuarioController.ObtenerUsuarios()
            .FirstOrDefault(u => u.Nombre.Trim().Equals(usuarioIngresado, StringComparison.OrdinalIgnoreCase));

        if (usuario == null)
        {
            MostrarError(mensajeError, "No se encontró el usuario especificado.");
            return;
        }

        try
        {
            usuario.Password = nuevaPass;
            _usuarioController.ActualizarUsuario(usuario);
            Close();
        }
        catch (Exception ex)
        {
            MostrarError(mensajeError, $"Error al actualizar: {ex.Message}");
        }
    }

    private void MostrarError(TextBlock? textBlock, string mensaje)
    {
        if (textBlock != null)
        {
            textBlock.Text = mensaje;
            textBlock.IsVisible = true;
        }
    }
}