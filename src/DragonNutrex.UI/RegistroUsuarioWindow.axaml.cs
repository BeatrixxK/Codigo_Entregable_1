// =====================================================
// LIBRERÍAS E IMPORTACIONES
// =====================================================
// Importación de librerías del sistema, utilidades de Avalonia (UI)
// y los controladores/modelos/servicios de DragonNutrex.
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

// =====================================================
// CLASE PRINCIPAL DE LA VENTANA
// =====================================================
// Lógica trasera (code-behind) de la interfaz de Registro de Usuario.
public partial class RegistroUsuarioWindow : Window
{
    // =====================================================
    // CONTROLADORES
    // =====================================================
    // Permite guardar y leer usuarios de la base de datos
    private readonly UsuarioController _usuarioController;

    // Permite consultar las dietas disponibles en el sistema
    private readonly EstadisticasNutricionController _estadisticasNutricionController;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    // Inicializa la ventana, sus dependencias y carga los datos iniciales
    public RegistroUsuarioWindow()
    {
        InitializeComponent();

        // Inyección manual de dependencias para los controladores
        _usuarioController = new UsuarioController(
            new UsuarioService(new UsuarioRepository())
        );

        _estadisticasNutricionController = new EstadisticasNutricionController(
            new EstadisticasNutricionService(
                new UsuarioRepository(),
                new MenuRepository()
            )
        );

        // Pre-carga los datos de los menús desplegables (ComboBox)
        CargarActividades();
        CargarObjetivos();
        CargarDietas();

        // Asocia el evento clic del botón a la función que guarda el usuario
        CrearUsuarioButton.Click += CrearUsuario;
    }

    // =========================================================
    // CARGA DE LISTAS DESPLEGABLES (COMBOBOX)
    // =========================================================

    // Llena el ComboBox de niveles de actividad física
    private void CargarActividades()
    {
        ActividadComboBox.Items.Clear();

        // Opción por defecto (Placeholder)
        ActividadComboBox.Items.Add(new ComboBoxItem { Content = "Seleccione actividad", Tag = "" });
        
        // Opciones reales
        ActividadComboBox.Items.Add(new ComboBoxItem { Content = "Sedentaria", Tag = "Sedentaria" });
        ActividadComboBox.Items.Add(new ComboBoxItem { Content = "Ligera", Tag = "Ligera" });
        ActividadComboBox.Items.Add(new ComboBoxItem { Content = "Moderada", Tag = "Moderada" });
        ActividadComboBox.Items.Add(new ComboBoxItem { Content = "Alta", Tag = "Alta" });

        ActividadComboBox.SelectedIndex = 0;
    }

    // Llena el ComboBox de metas nutricionales
    private void CargarObjetivos()
    {
        ObjetivoComboBox.Items.Clear();

        // Opción por defecto (Placeholder)
        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Seleccione objetivo", Tag = "" });
        
        // Opciones reales
        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Bajar peso", Tag = "Bajar peso" });
        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Mantener", Tag = "Mantener" });
        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Ganar masa", Tag = "Ganar masa" });
        ObjetivoComboBox.Items.Add(new ComboBoxItem { Content = "Resistencia", Tag = "Resistencia" });

        ObjetivoComboBox.SelectedIndex = 0;
    }

    // Consulta la base de datos y llena el ComboBox con las dietas existentes
    private void CargarDietas()
    {
        TipoDietaComboBox.Items.Clear();

        // Opción por defecto (Placeholder)
        TipoDietaComboBox.Items.Add(new ComboBoxItem { Content = "Seleccione tipo de dieta", Tag = "" });

        // Recupera las dietas y las agrega una por una
        var dietas = _estadisticasNutricionController.ObtenerDietasDisponibles();
        foreach (var dieta in dietas)
        {
            TipoDietaComboBox.Items.Add(new ComboBoxItem { Content = dieta.Nombre, Tag = dieta.Nombre });
        }

        TipoDietaComboBox.SelectedIndex = 0;
    }

    // =========================================================
    // LÓGICA DE REGISTRO
    // =========================================================

    // Recopila datos, los valida, crea el usuario y lo redirige a la app
    private async void CrearUsuario(object? sender, RoutedEventArgs e)
    {
        try
        {
            MensajeTextBlock.Text = string.Empty; // Limpia errores previos

            // Captura el texto de los campos limpiando espacios en blanco
            var nombre = (NombreTextBox.Text ?? string.Empty).Trim();
            var correo = (CorreoTextBox.Text ?? string.Empty).Trim();
            var password = PasswordTextBox.Text ?? string.Empty;
            var confirmarPassword = ConfirmarPasswordTextBox.Text ?? string.Empty;

            // =========================
            // VALIDACIONES DE TEXTO Y DUPLICADOS
            // =========================
            
            // Verifica que los campos básicos no estén vacíos
            if (string.IsNullOrWhiteSpace(nombre)) { await MostrarMensaje("Ingrese el nombre completo."); return; }
            if (string.IsNullOrWhiteSpace(correo)) { await MostrarMensaje("Ingrese un correo electrónico."); return; }
            if (!EsCorreoValido(correo)) { await MostrarMensaje("Ingrese un correo electrónico válido."); return; }

            // Verifica que el correo no pertenezca a un usuario ya registrado
            var usuariosExistentes = _usuarioController.ObtenerUsuarios();
            if (usuariosExistentes.Any(u => !string.IsNullOrWhiteSpace(u.Correo) && u.Correo.Trim().Equals(correo, StringComparison.OrdinalIgnoreCase)))
            {
                await MostrarMensaje("Ese correo ya está registrado.");
                return;
            }

            // =========================
            // VALIDACIONES NUMÉRICAS
            // =========================

            // Convierte a decimal y valida rangos lógicos para peso (0-500kg)
            if (!decimal.TryParse(PesoTextBox.Text, out var peso) || peso <= 0 || peso > 500)
            {
                await MostrarMensaje("Ingrese un peso válido en kilogramos. Ejemplo: 70");
                return;
            }

            // Convierte a decimal y valida rangos lógicos para altura (1.00m - 2.50m)
            if (!decimal.TryParse(AlturaTextBox.Text, out var altura) || altura <= 0 || altura > 3)
            {
                await MostrarMensaje("Ingrese una altura válida en metros. Ejemplo: 1.75");
                return;
            }
            if (altura < 1.00m || altura > 2.50m)
            {
                await MostrarMensaje("La altura debe ingresarse en metros y en un rango realista. Ejemplo: 1.75");
                return;
            }

            // =========================
            // VALIDACIONES DE SELECCIÓN (COMBOBOX)
            // =========================

            // Asegura que el usuario no haya dejado las opciones en el "Placeholder"
            var actividad = ObtenerValorCombo(ActividadComboBox);
            if (string.IsNullOrWhiteSpace(actividad)) { await MostrarMensaje("Seleccione una actividad."); return; }

            var objetivo = ObtenerValorCombo(ObjetivoComboBox);
            if (string.IsNullOrWhiteSpace(objetivo)) { await MostrarMensaje("Seleccione un objetivo."); return; }

            var tipoDieta = ObtenerValorCombo(TipoDietaComboBox);
            if (string.IsNullOrWhiteSpace(tipoDieta)) { await MostrarMensaje("Seleccione un tipo de dieta."); return; }

            // =========================
            // VALIDACIONES DE SEGURIDAD (CONTRASEÑA)
            // =========================

            // Verifica longitud mínima y que ambas contraseñas digitadas coincidan
            if (string.IsNullOrWhiteSpace(password)) { await MostrarMensaje("Ingrese una contraseña."); return; }
            if (password.Length < 4) { await MostrarMensaje("La contraseña debe tener al menos 4 caracteres."); return; }
            if (password != confirmarPassword) { await MostrarMensaje("La confirmación de contraseña no coincide."); return; }

            // =========================
            // CREACIÓN Y GUARDADO
            // =========================

            // Empaqueta los datos limpios en un nuevo objeto de modelo
            var nuevoUsuario = new Usuario
            {
                Nombre = nombre, Correo = correo, Peso = peso, Altura = altura,
                Actividad = actividad, Objetivo = objetivo, TipoDieta = tipoDieta, Password = password
            };

            // Envía el objeto a la base de datos a través del controlador
            _usuarioController.CrearUsuario(nuevoUsuario);

            MensajeTextBlock.Text = "Usuario registrado correctamente. Iniciando sesión...";

            // =========================
            // REDIRECCIÓN Y AUTENTICACIÓN
            // =========================

            // Marca al usuario como logueado globalmente en la sesión actual
            AuthSession.IniciarUsuario(nuevoUsuario);

            // Carga y muestra la pantalla principal (Dashboard)
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Cierra la ventana actual de registro
            Close();

            // Busca y cierra la ventana de Login si quedó abierta en el fondo
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
            // Captura errores no previstos (ej. caída de base de datos)
            await MostrarMensaje($"Error al registrar usuario: {ex.Message}");
        }
    }

    // =========================================================
    // MÉTODOS DE SOPORTE (HELPERS)
    // =========================================================

    // Extrae de forma segura el valor (Tag) seleccionado de un ComboBox
    private string ObtenerValorCombo(ComboBox comboBox)
    {
        if (comboBox.SelectedItem is ComboBoxItem item && item.Tag is string valor)
        {
            return valor;
        }
        return string.Empty;
    }

    // Usa una Expresión Regular (Regex) para confirmar que el texto tiene formato "usuario@dominio.com"
    private bool EsCorreoValido(string correo)
    {
        return Regex.IsMatch(
            correo,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase
        );
    }

    // Construye dinámicamente una mini-ventana flotante (Modal) para mostrar alertas al usuario
    private async Task MostrarMensaje(string mensaje)
    {
        var ventana = new Window { Title = "Mensaje", Width = 380, Height = 170 };
        var okButton = new Button { Content = "OK", Width = 100, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center };
        
        okButton.Click += (_, _) => ventana.Close();

        ventana.Content = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 12,
            Children =
            {
                new TextBlock { Text = mensaje, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                okButton
            }
        };

        // Muestra la ventana y bloquea la interacción con la pantalla de atrás hasta que se cierre
        await ventana.ShowDialog(this);
    }
}