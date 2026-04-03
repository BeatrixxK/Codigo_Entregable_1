using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Models;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;

namespace DragonNutrex.UI;

public partial class RegistroUsuarioWindow : Window
{
    // Controlador para operaciones CRUD de usuarios
    private readonly UsuarioController _usuarioController;

    // Controlador para consultar dietas disponibles
    private readonly EstadisticasNutricionController _estadisticasNutricionController;

    public RegistroUsuarioWindow()
    {
        InitializeComponent();

        // Inicializa controlador de usuarios
        _usuarioController = new UsuarioController(
            new UsuarioService(new UsuarioRepository())
        );

        // Inicializa controlador de estadísticas para obtener dietas
        _estadisticasNutricionController = new EstadisticasNutricionController(
            new EstadisticasNutricionService(
                new UsuarioRepository(),
                new MenuRepository()
            )
        );

        // Carga opciones en los ComboBox
        CargarActividades();
        CargarObjetivos();
        CargarDietas();

        // Asocia el botón al método de creación de usuario
        CrearUsuarioButton.Click += CrearUsuario;
    }

    // =========================================================
    // CARGA DE COMBOBOX
    // =========================================================

    // Carga las opciones del nivel de actividad
    private void CargarActividades()
    {
        ActividadComboBox.Items.Clear();

        // Opción inicial tipo placeholder
        ActividadComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Seleccione actividad",
            Tag = ""
        });

        ActividadComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Sedentaria",
            Tag = "Sedentaria"
        });

        ActividadComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Ligera",
            Tag = "Ligera"
        });

        ActividadComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Moderada",
            Tag = "Moderada"
        });

        ActividadComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Alta",
            Tag = "Alta"
        });

        ActividadComboBox.SelectedIndex = 0;
    }

    // Carga las opciones del objetivo nutricional
    private void CargarObjetivos()
    {
        ObjetivoComboBox.Items.Clear();

        // Opción inicial tipo placeholder
        ObjetivoComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Seleccione objetivo",
            Tag = ""
        });

        ObjetivoComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Bajar peso",
            Tag = "Bajar peso"
        });

        ObjetivoComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Mantener",
            Tag = "Mantener"
        });

        ObjetivoComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Ganar masa",
            Tag = "Ganar masa"
        });

        ObjetivoComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Resistencia",
            Tag = "Resistencia"
        });

        ObjetivoComboBox.SelectedIndex = 0;
    }

    // Carga las dietas disponibles desde el sistema
    private void CargarDietas()
    {
        TipoDietaComboBox.Items.Clear();

        // Opción inicial tipo placeholder
        TipoDietaComboBox.Items.Add(new ComboBoxItem
        {
            Content = "Seleccione tipo de dieta",
            Tag = ""
        });

        var dietas = _estadisticasNutricionController.ObtenerDietasDisponibles();

        foreach (var dieta in dietas)
        {
            TipoDietaComboBox.Items.Add(new ComboBoxItem
            {
                Content = dieta.Nombre,
                Tag = dieta.Nombre
            });
        }

        TipoDietaComboBox.SelectedIndex = 0;
    }

    // =========================================================
    // CREACIÓN DE USUARIO
    // =========================================================

    // Se ejecuta al presionar el botón "Crear Usuario"
    private async void CrearUsuario(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Limpia mensaje anterior
            MensajeTextBlock.Text = string.Empty;

            // Lee los datos del formulario
            var nombre = (NombreTextBox.Text ?? string.Empty).Trim();
            var correo = (CorreoTextBox.Text ?? string.Empty).Trim();
            var password = PasswordTextBox.Text ?? string.Empty;
            var confirmarPassword = ConfirmarPasswordTextBox.Text ?? string.Empty;

            // =========================
            // VALIDACIONES DE TEXTO
            // =========================

            if (string.IsNullOrWhiteSpace(nombre))
            {
                await MostrarMensaje("Ingrese el nombre completo.");
                return;
            }

            if (string.IsNullOrWhiteSpace(correo))
            {
                await MostrarMensaje("Ingrese un correo electrónico.");
                return;
            }

            if (!EsCorreoValido(correo))
            {
                await MostrarMensaje("Ingrese un correo electrónico válido.");
                return;
            }

            // Verifica si ya existe un usuario con el mismo correo
            var usuariosExistentes = _usuarioController.ObtenerUsuarios();

            if (usuariosExistentes.Any(u =>
                    !string.IsNullOrWhiteSpace(u.Correo) &&
                    u.Correo.Trim().Equals(correo, StringComparison.OrdinalIgnoreCase)))
            {
                await MostrarMensaje("Ese correo ya está registrado.");
                return;
            }

            // =========================
            // VALIDACIONES NUMÉRICAS
            // =========================

            // Valida peso en kilogramos
            if (!decimal.TryParse(PesoTextBox.Text, out var peso) || peso <= 0 || peso > 500)
            {
                await MostrarMensaje("Ingrese un peso válido en kilogramos. Ejemplo: 70");
                return;
            }

            // Valida altura en metros
            if (!decimal.TryParse(AlturaTextBox.Text, out var altura) || altura <= 0 || altura > 3)
            {
                await MostrarMensaje("Ingrese una altura válida en metros. Ejemplo: 1.75");
                return;
            }

            // Validación adicional para rango realista
            if (altura < 1.00m || altura > 2.50m)
            {
                await MostrarMensaje("La altura debe ingresarse en metros y en un rango realista. Ejemplo: 1.75");
                return;
            }

            // =========================
            // VALIDACIONES DE COMBOBOX
            // =========================

            var actividad = ObtenerValorCombo(ActividadComboBox);
            if (string.IsNullOrWhiteSpace(actividad))
            {
                await MostrarMensaje("Seleccione una actividad.");
                return;
            }

            var objetivo = ObtenerValorCombo(ObjetivoComboBox);
            if (string.IsNullOrWhiteSpace(objetivo))
            {
                await MostrarMensaje("Seleccione un objetivo.");
                return;
            }

            var tipoDieta = ObtenerValorCombo(TipoDietaComboBox);
            if (string.IsNullOrWhiteSpace(tipoDieta))
            {
                await MostrarMensaje("Seleccione un tipo de dieta.");
                return;
            }

            // =========================
            // VALIDACIONES DE CONTRASEÑA
            // =========================

            if (string.IsNullOrWhiteSpace(password))
            {
                await MostrarMensaje("Ingrese una contraseña.");
                return;
            }

            if (password.Length < 4)
            {
                await MostrarMensaje("La contraseña debe tener al menos 4 caracteres.");
                return;
            }

            if (password != confirmarPassword)
            {
                await MostrarMensaje("La confirmación de contraseña no coincide.");
                return;
            }

            // =========================
            // CREACIÓN DEL OBJETO USUARIO
            // =========================

            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Peso = peso,
                Altura = altura,
                Actividad = actividad,
                Objetivo = objetivo,
                TipoDieta = tipoDieta,
                Password = password
            };

            // Guarda el usuario
            _usuarioController.CrearUsuario(nuevoUsuario);

            // Mensaje visual de éxito
            MensajeTextBlock.Text = "Usuario registrado correctamente. Iniciando sesión...";

            // =========================
            // LOGIN AUTOMÁTICO
            // =========================

            // Inicia sesión automáticamente con el usuario recién creado
            AuthSession.IniciarUsuario(nuevoUsuario);

            // Abre la ventana principal
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Cierra esta ventana de registro
            Close();

            // Si la ventana de login sigue abierta, la cierra
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                foreach (var window in desktop.Windows.ToList())
                {
                    if (window is LoginWindow)
                    {
                        window.Close();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await MostrarMensaje($"Error al registrar usuario: {ex.Message}");
        }
    }

    // =========================================================
    // MÉTODOS AUXILIARES
    // =========================================================

    // Obtiene el Tag del ComboBox seleccionado
    private string ObtenerValorCombo(ComboBox comboBox)
    {
        if (comboBox.SelectedItem is ComboBoxItem item && item.Tag is string valor)
        {
            return valor;
        }

        return string.Empty;
    }

    // Valida que el correo tenga un formato básico correcto
    private bool EsCorreoValido(string correo)
    {
        return Regex.IsMatch(
            correo,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase
        );
    }

    // Muestra una ventana modal simple con mensajes de validación o error
    private async Task MostrarMensaje(string mensaje)
    {
        var ventana = new Window
        {
            Title = "Mensaje",
            Width = 380,
            Height = 170
        };

        var okButton = new Button
        {
            Content = "OK",
            Width = 100,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };

        okButton.Click += (_, _) => ventana.Close();

        ventana.Content = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 12,
            Children =
            {
                new TextBlock
                {
                    Text = mensaje,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                },
                okButton
            }
        };

        await ventana.ShowDialog(this);
    }
}