using System;
using System.Linq; 
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using DragonNutrex.App.Repositories;
using DragonNutrex.App.Models; 

namespace DragonNutrex.UI;

public partial class LoginWindow : Window
{
    private bool _redisDisponible = false;
    private RedisConnection? _redisConnection;

    public LoginWindow()
    {
        InitializeComponent();
        this.Opened += LoginWindow_Opened;
    }

    private void LoginWindow_Opened(object? sender, EventArgs e)
    {
        _ = InicializarServiciosAsync(); 
    }

    private async Task InicializarServiciosAsync()
    {
        try
        {
            _redisConnection = AppServices.Redis;

            if (_redisConnection == null)
            {
                _redisDisponible = false;
                await MostrarErrorConexionAsync();
                return;
            }

            _redisDisponible = true;
            Console.WriteLine("Conexión a Redis inicializada correctamente.");

            // 🔥 ENCENDEMOS LA MÁQUINA CREADORA ANTES DE QUE EL USUARIO INICIE SESIÓN
            GenerarDatosMasivos();
        }
        catch (Exception ex)
        {
            _redisDisponible = false;
            Console.WriteLine("Error al inicializar servicios: " + ex.Message);
            await MostrarErrorConexionAsync();
        }
    }

    // =====================================================
    // MÁQUINA CREADORA DE DATOS (SEED) 🪄
    // =====================================================
    private void GenerarDatosMasivos()
    {
        var redis = AppServices.Redis!;
        var usuarioRepo = new UsuarioRedisRepository(redis);
        var productoRepo = new ProductoRedisRepository(redis);

        // Si ya existen usuarios, detenemos la máquina para no clonarlos cada vez que abrimos la app
        if (usuarioRepo.GetAll().Count > 0) return;

        Console.WriteLine("Base de datos vacía detectada. Generando usuarios y productos...");

        // 1. Cuenta de Admin
        usuarioRepo.Create(new Usuario { Nombre = "admin", Password = "123", Peso = 0, Altura = 0 });

        // 2. Cuentas de Prueba
        for (int i = 1; i <= 29; i++)
        {
            usuarioRepo.Create(new Usuario {
                Nombre = $"Usuario {i}", 
                Password = "Upi.2025", 
                Peso = 60 + (i % 15), 
                Altura = 160 + (i % 20), 
                Actividad = "Ligera", 
                Objetivo = "Bajar peso", 
                TipoDieta = "Vegana"
            });
        }

        // 3. Catálogo de Alimentos
        productoRepo.Create(new Producto { Nombre = "Manzana", Calorias = 52, Proteinas = 0.3m, Carbohidratos = 14, Grasas = 0.2m });
        productoRepo.Create(new Producto { Nombre = "Pollo a la plancha", Calorias = 165, Proteinas = 31, Carbohidratos = 0, Grasas = 3.6m });
        productoRepo.Create(new Producto { Nombre = "Arroz blanco", Calorias = 130, Proteinas = 2.7m, Carbohidratos = 28, Grasas = 0.3m });
        productoRepo.Create(new Producto { Nombre = "Huevo duro", Calorias = 78, Proteinas = 6.3m, Carbohidratos = 0.6m, Grasas = 5.3m });
        productoRepo.Create(new Producto { Nombre = "Brócoli", Calorias = 34, Proteinas = 2.8m, Carbohidratos = 7, Grasas = 0.4m });
        
        Console.WriteLine("¡Datos generados exitosamente!");
    }

    private async Task MostrarErrorConexionAsync()
    {
        var mensaje = new Window
        {
            Title = "Error de conexión",
            Width = 420,
            Height = 180,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new TextBlock
            {
                Text = "No se pudo conectar con Redis. Verifica la conexión a internet o la configuración del servidor.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Avalonia.Thickness(20)
            }
        };

        await mensaje.ShowDialog(this);
    }

    // ====================================================================
    // ACCIÓN DEL BOTÓN INGRESAR
    // ====================================================================
    public void BtnIngresar_Click(object? sender, RoutedEventArgs e)
    {
        ErrorTextBlock.Text = "";

        if (!_redisDisponible)
        {
            ErrorTextBlock.Text = "No hay conexión a la base de datos.";
            return; 
        }

        var usuario = UsuarioTextBox.Text?.Trim() ?? "";
        var password = PasswordTextBox.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
        {
            ErrorTextBlock.Text = "Por favor, ingresa tu usuario y contraseña.";
            return;
        }

        try
        {
            var usuarioRepo = new UsuarioRedisRepository(AppServices.Redis!);
            var todosLosUsuarios = usuarioRepo.GetAll();

            var usuarioValido = todosLosUsuarios.FirstOrDefault(u => 
                u.Nombre.Equals(usuario, StringComparison.OrdinalIgnoreCase) && 
                u.Password == password);

            if (usuarioValido != null) 
            {
                this.Hide();
                var mainWindow = new MainWindow(usuarioValido); 
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ErrorTextBlock.Text = "Usuario o contraseña incorrectos.";
            }
        }
        catch (Exception ex)
        {
            ErrorTextBlock.Text = "Error al intentar iniciar sesión.";
            Console.WriteLine("Error crítico: " + ex.Message);
        }
    }

    public void BtnRegistrarse_Click(object? sender, RoutedEventArgs e)
    {
        ErrorTextBlock.Text = "";
        var registroWindow = new RegistroUsuarioWindow();
        registroWindow.Show();
    }
}