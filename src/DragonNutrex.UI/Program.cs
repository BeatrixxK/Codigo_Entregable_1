// =====================================================
// LIBRERÍAS E IMPORTACIONES
// =====================================================
// Importación de espacios de nombres base del sistema, el framework Avalonia 
// y las utilidades internas de tu proyecto.
using System;
using Avalonia;
using DragonNutrex.App.Utils;

// Espacio de nombres correspondiente a la capa de Interfaz de Usuario (UI)
namespace DragonNutrex.UI;

// =====================================================
// CLASE PRINCIPAL
// =====================================================
// Clase estática que sirve como el corazón y punto de entrada de toda la aplicación.
internal static class Program
{
    // Atributo esencial en apps de escritorio; asegura que la UI corra en un solo hilo (Single-Threaded Apartment)
    [STAThread]
    
    // Método principal que se ejecuta al compilar y arrancar el programa
    public static void Main(string[] args)
    {
        // Bloque try-catch para capturar fallos catastróficos antes de que cargue la pantalla
        try
        {
            // =====================================================
            // INICIALIZACIÓN DE DATOS (OPCIONAL)
            // =====================================================
            // Genera datos semilla en la base de datos o almacenamiento. 
            // DataSeeder.GenerarDatos(); // solo si ocupas regenerar datos una vez

            // =====================================================
            // ARRANQUE DE LA APP
            // =====================================================
            // Llama al constructor de Avalonia y lanza la ventana principal de escritorio
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            // =====================================================
            // MANEJO DE ERRORES FATALES
            // =====================================================
            // Si la app no puede arrancar, imprime el error en la terminal/consola
            Console.WriteLine("ERROR FATAL EN PROGRAM:");
            Console.WriteLine(ex.ToString());
            
            // Relanza la excepción para detener el proceso por completo
            throw;
        }
    }

    // =====================================================
    // CONFIGURACIÓN DEL FRAMEWORK
    // =====================================================
    // Prepara la aplicación Avalonia vinculándola con tu clase `App`, 
    // autodetecta el sistema operativo (Windows, macOS, Linux) y activa los logs.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}