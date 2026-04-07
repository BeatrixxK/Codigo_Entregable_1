// =====================================================
// IMPORTACIONES
// =====================================================

// Librerías base del sistema
using System;
using System.Linq;

// Librerías de Avalonia para UI y eventos
using Avalonia.Controls;
using Avalonia.Interactivity;

// Capas de la aplicación (arquitectura por capas)
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;

// Namespace de la interfaz gráfica
namespace DragonNutrex.UI;

// =====================================================
// VENTANA DE LOGIN
// =====================================================

// Clase parcial que representa la ventana de inicio de sesión
public partial class LoginWindow : Window
{
    // Controlador de usuarios (conecta UI con lógica de negocio)
    private readonly UsuarioController _usuarioController;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Inicializa la ventana y configura dependencias y eventos
    public LoginWindow()
    {
        // Inicializa los componentes gráficos (XAML)
        InitializeComponent();

        // Inyección manual de dependencias (Controller → Service → Repository)
        _usuarioController = new UsuarioController(
            new UsuarioService(new UsuarioRepository())
        );

        // Eventos de botones
        LoginButton.Click += Login; // Botón login
        RegistrarseButton.Click += AbrirRegistro; // Botón registro
        
        // Evento para recuperación de contraseña
        OlvidastePasswordButton.Click += AbrirCambiarPassword;
    }

    // =====================================================
    // MÉTODO LOGIN
    // =====================================================
    // Valida credenciales e inicia sesión
    private async void Login(object? sender, RoutedEventArgs e)
    {
        // Obtiene datos ingresados por el usuario
        var usuarioIngresado = (UsuarioTextBox.Text ?? "").Trim();
        var passwordIngresado = PasswordTextBox.Text ?? "";

        // Validación de usuario administrador (hardcodeado)
        if (usuarioIngresado.Equals("stephsg", StringComparison.OrdinalIgnoreCase)
            && passwordIngresado == "Upi.2025")
        {
            // Inicia sesión como admin
            AuthSession.IniciarAdmin();

            // Abre ventana principal
            var main = new MainWindow();
            main.Show();

            // Cierra ventana actual
            Close();
            return;
        }

        // Busca el usuario en la lista
        var usuario = _usuarioController.ObtenerUsuarios()
            .FirstOrDefault(u =>
                u.Nombre.Trim().Equals(usuarioIngresado, StringComparison.OrdinalIgnoreCase));

        // Validación: usuario no existe
        if (usuario == null)
        {
            ErrorTextBlock.Text = "Usuario no encontrado.";
            return;
        }

        // Validación: contraseña incorrecta
        if (usuario.Password != passwordIngresado)
        {
            ErrorTextBlock.Text = "Contraseña incorrecta.";
            return;
        }

        // Inicia sesión con usuario válido
        AuthSession.IniciarUsuario(usuario);

        // Abre ventana principal
        var mainWindow = new MainWindow();
        mainWindow.Show();

        // Cierra login
        Close();
    }

    // =====================================================
    // MÉTODO ABRIR REGISTRO
    // =====================================================
    // Abre la ventana para registrar un nuevo usuario
    private async void AbrirRegistro(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Crea la ventana de registro
            var ventana = new RegistroUsuarioWindow();

            // Abre como diálogo modal
            await ventana.ShowDialog(this);
        }
        catch (Exception ex)
        {
            // Muestra error en pantalla
            ErrorTextBlock.Text = $"Error al abrir registro: {ex.Message}";
        }
    }

    // =====================================================
    // MÉTODO CAMBIAR CONTRASEÑA
    // =====================================================
    // Abre la ventana de recuperación/cambio de contraseña
    private async void AbrirCambiarPassword(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Instancia la ventana de cambio de contraseña
            var ventanaPassword = new CambiarPasswordWindow();

            // Abre como diálogo modal
            await ventanaPassword.ShowDialog(this);
        }
        catch (Exception ex)
        {
            // Muestra error en pantalla
            ErrorTextBlock.Text = $"Error al abrir recuperación: {ex.Message}";
        }
    }
}