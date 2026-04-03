using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using DragonNutrex.App.Controllers;
using DragonNutrex.App.Models;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Services;

namespace DragonNutrex.UI;

public partial class RegistroUsuarioWindow : Window
{
    private readonly UsuarioController _usuarioController;

    public RegistroUsuarioWindow()
    {
        InitializeComponent();

        _usuarioController = new UsuarioController(
            new UsuarioService(new UsuarioRepository())
        );

        CargarDietas();
        GuardarRegistroButton.Click += GuardarUsuario;
    }

    private void CargarDietas()
    {
        TipoDietaComboBox.Items.Clear();

        string[] dietas =
        {
            "Balanceada",
            "Vegetariana",
            "Vegana",
            "Alta proteína",
            "Keto"
        };

        foreach (var dieta in dietas)
        {
            TipoDietaComboBox.Items.Add(new ComboBoxItem
            {
                Content = dieta,
                Tag = dieta
            });
        }
    }

    private void GuardarUsuario(object? sender, RoutedEventArgs e)
    {
        var tipoDieta = "";

        if (TipoDietaComboBox.SelectedItem is ComboBoxItem item && item.Tag is string dieta)
            tipoDieta = dieta;

        var password = PasswordTextBox.Text ?? "";

        if (string.IsNullOrWhiteSpace(NombreTextBox.Text))
        {
            MensajeTextBlock.Text = "Debe ingresar un nombre.";
            MensajeTextBlock.Foreground = Brushes.Red;
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            MensajeTextBlock.Text = "Debe ingresar una contraseña.";
            MensajeTextBlock.Foreground = Brushes.Red;
            return;
        }

        var usuario = new Usuario
        {
            Nombre = NombreTextBox.Text ?? "",
            Peso = decimal.TryParse(PesoTextBox.Text, out var peso) ? peso : 0,
            Altura = decimal.TryParse(AlturaTextBox.Text, out var altura) ? altura : 0,
            Actividad = ActividadTextBox.Text ?? "",
            Objetivo = ObjetivoTextBox.Text ?? "",
            TipoDieta = tipoDieta,
            Password = password
        };

        _usuarioController.CrearUsuario(usuario);

        MensajeTextBlock.Text = "Usuario registrado correctamente.";
        MensajeTextBlock.Foreground = Brushes.Green;

        NombreTextBox.Text = "";
        PesoTextBox.Text = "";
        AlturaTextBox.Text = "";
        ActividadTextBox.Text = "";
        ObjetivoTextBox.Text = "";
        PasswordTextBox.Text = "";
        TipoDietaComboBox.SelectedItem = null;
    }
}