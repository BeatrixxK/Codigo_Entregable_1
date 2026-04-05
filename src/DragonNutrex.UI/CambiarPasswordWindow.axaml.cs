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
        
        // Inicializamos el controlador igual que en el LoginWindow
        _usuarioController = new UsuarioController(
            new UsuarioService(new UsuarioRepository())
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
        this.Close(); // Cierra sin guardar
    }

    private void GuardarPasswordButton_Click(object? sender, RoutedEventArgs e)
    {
        var usuarioIngresado = this.FindControl<TextBox>("UsuarioRecuperarTextBox")?.Text?.Trim();
        var nuevaPass = this.FindControl<TextBox>("NuevaPasswordTextBox")?.Text;
        var confirmarPass = this.FindControl<TextBox>("ConfirmarPasswordTextBox")?.Text;
        var mensajeError = this.FindControl<TextBlock>("MensajeErrorTextBlock");

        // 1. Validar que no estén vacíos
        if (string.IsNullOrWhiteSpace(usuarioIngresado) || string.IsNullOrWhiteSpace(nuevaPass) || string.IsNullOrWhiteSpace(confirmarPass))
        {
            MostrarError(mensajeError, "Todos los campos son obligatorios.");
            return;
        }

        // 2. Validar que las contraseñas coincidan
        if (nuevaPass != confirmarPass)
        {
            MostrarError(mensajeError, "Las contraseñas no coinciden.");
            return;
        }

        // 3. Buscar al usuario en la base de datos a través del controlador
        var usuario = _usuarioController.ObtenerUsuarios()
            .FirstOrDefault(u => u.Nombre.Trim().Equals(usuarioIngresado, StringComparison.OrdinalIgnoreCase));

        // 4. Si el usuario no existe, mostramos error
        if (usuario == null)
        {
            MostrarError(mensajeError, "No se encontró el usuario especificado.");
            return;
        }

        // 5. Actualizamos la contraseña y guardamos
        try
        {
            usuario.Password = nuevaPass;
            
            // Llama a tu método para guardar los cambios
            _usuarioController.ActualizarUsuario(usuario); 
            
            // Si todo salió bien, cerramos la ventana
            this.Close();
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