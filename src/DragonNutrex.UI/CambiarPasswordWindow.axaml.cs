// =====================================================
// IMPORTACIONES
// =====================================================

// Librerías base
using System;
using System.Linq;

// Librerías de Avalonia para UI y eventos
using Avalonia.Controls;
using Avalonia.Interactivity;

// Capas de la aplicación (arquitectura en capas)
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;

// Namespace de la interfaz
namespace DragonNutrex.UI;

// =====================================================
// VENTANA CAMBIAR CONTRASEÑA
// =====================================================

// Clase que representa la ventana para cambiar contraseña
public partial class CambiarPasswordWindow : Window
{
    // Controlador de usuarios (conecta UI con lógica de negocio)
    private readonly UsuarioController _usuarioController;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Inicializa la ventana, dependencias y eventos
    public CambiarPasswordWindow()
    {
        // Inicializa componentes del XAML
        InitializeComponent();
        
        // Inicializa el controlador (Controller → Service → Repository)
        _usuarioController = new UsuarioController(
            new UsuarioService(new UsuarioRepository())
        );
        
        // Busca el botón Cancelar por nombre en la UI
        var cancelarBtn = this.FindControl<Button>("CancelarButton");
        
        // Si existe, le asigna el evento click
        if (cancelarBtn != null)
            cancelarBtn.Click += CancelarButton_Click;

        // Busca el botón Guardar contraseña
        var guardarBtn = this.FindControl<Button>("GuardarPasswordButton");
        
        // Si existe, le asigna el evento click
        if (guardarBtn != null)
            guardarBtn.Click += GuardarPasswordButton_Click;
    }

    // =====================================================
    // MÉTODO CANCELAR
    // =====================================================
    // Cierra la ventana sin guardar cambios
    private void CancelarButton_Click(object? sender, RoutedEventArgs e)
    {
        this.Close(); // Cierra la ventana
    }

    // =====================================================
    // MÉTODO GUARDAR PASSWORD
    // =====================================================
    // Valida y actualiza la contraseña del usuario
    private void GuardarPasswordButton_Click(object? sender, RoutedEventArgs e)
    {
        // Obtiene valores ingresados en los TextBox
        var usuarioIngresado = this.FindControl<TextBox>("UsuarioRecuperarTextBox")?.Text?.Trim();
        var nuevaPass = this.FindControl<TextBox>("NuevaPasswordTextBox")?.Text;
        var confirmarPass = this.FindControl<TextBox>("ConfirmarPasswordTextBox")?.Text;
        
        // Obtiene el TextBlock donde se muestran errores
        var mensajeError = this.FindControl<TextBlock>("MensajeErrorTextBlock");

        // =====================================================
        // VALIDACIÓN 1: CAMPOS VACÍOS
        // =====================================================
        if (string.IsNullOrWhiteSpace(usuarioIngresado) || string.IsNullOrWhiteSpace(nuevaPass) || string.IsNullOrWhiteSpace(confirmarPass))
        {
            MostrarError(mensajeError, "Todos los campos son obligatorios.");
            return;
        }

        // =====================================================
        // VALIDACIÓN 2: CONTRASEÑAS COINCIDEN
        // =====================================================
        if (nuevaPass != confirmarPass)
        {
            MostrarError(mensajeError, "Las contraseñas no coinciden.");
            return;
        }

        // =====================================================
        // BÚSQUEDA DEL USUARIO
        // =====================================================
        // Busca el usuario en la base de datos (ignorando mayúsculas/minúsculas)
        var usuario = _usuarioController.ObtenerUsuarios()
            .FirstOrDefault(u => u.Nombre.Trim().Equals(usuarioIngresado, StringComparison.OrdinalIgnoreCase));

        // =====================================================
        // VALIDACIÓN 3: USUARIO EXISTE
        // =====================================================
        if (usuario == null)
        {
            MostrarError(mensajeError, "No se encontró el usuario especificado.");
            return;
        }

        // =====================================================
        // ACTUALIZACIÓN DE CONTRASEÑA
        // =====================================================
        try
        {
            // Asigna la nueva contraseña
            usuario.Password = nuevaPass;
            
            // Guarda los cambios en la base de datos
            _usuarioController.ActualizarUsuario(usuario); 
            
            // Cierra la ventana si todo fue exitoso
            this.Close();
        }
        catch (Exception ex)
        {
            // Muestra error si falla la actualización
            MostrarError(mensajeError, $"Error al actualizar: {ex.Message}");
        }
    }

    // =====================================================
    // MÉTODO MOSTRAR ERROR
    // =====================================================
    // Muestra un mensaje de error en pantalla
    private void MostrarError(TextBlock? textBlock, string mensaje)
    {
        if (textBlock != null)
        {
            textBlock.Text = mensaje; // Asigna el mensaje
            textBlock.IsVisible = true; // Hace visible el error
        }
    }
}