using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;

namespace DragonNutrex.UI;

public partial class LoginWindow : Window
{
    private readonly UsuarioController _usuarioController;

    public LoginWindow()
    {
        InitializeComponent();

        _usuarioController = new UsuarioController(
            new UsuarioService(new UsuarioRepository())
        );

        LoginButton.Click += Login;
        RegistrarseButton.Click += AbrirRegistro;
        
        // Conectamos el nuevo botón a su método
        OlvidastePasswordButton.Click += AbrirCambiarPassword;
    }

    private async void Login(object? sender, RoutedEventArgs e)
    {
        var usuarioIngresado = (UsuarioTextBox.Text ?? "").Trim();
        var passwordIngresado = PasswordTextBox.Text ?? "";

        if (usuarioIngresado.Equals("stephsg", StringComparison.OrdinalIgnoreCase)
            && passwordIngresado == "Upi.2025")
        {
            AuthSession.IniciarAdmin();

            var main = new MainWindow();
            main.Show();
            Close();
            return;
        }

        var usuario = _usuarioController.ObtenerUsuarios()
            .FirstOrDefault(u =>
                u.Nombre.Trim().Equals(usuarioIngresado, StringComparison.OrdinalIgnoreCase));

        if (usuario == null)
        {
            ErrorTextBlock.Text = "Usuario no encontrado.";
            return;
        }

        if (usuario.Password != passwordIngresado)
        {
            ErrorTextBlock.Text = "Contraseña incorrecta.";
            return;
        }

        AuthSession.IniciarUsuario(usuario);

        var mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }

    private async void AbrirRegistro(object? sender, RoutedEventArgs e)
    {
        try
        {
            var ventana = new RegistroUsuarioWindow();
            await ventana.ShowDialog(this);
        }
        catch (Exception ex)
        {
            ErrorTextBlock.Text = $"Error al abrir registro: {ex.Message}";
        }
    }

    // NUEVO: Método para abrir tu ventana de Cambiar Contraseña
    private async void AbrirCambiarPassword(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Instanciamos la ventana que ya tenías creada
            var ventanaPassword = new CambiarPasswordWindow();
            await ventanaPassword.ShowDialog(this);
        }
        catch (Exception ex)
        {
            ErrorTextBlock.Text = $"Error al abrir recuperación: {ex.Message}";
        }
    }
}