// =====================================================
// IMPORTACIONES
// =====================================================

// Librerías base de Avalonia (framework UI)
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

// Namespace de la interfaz
namespace DragonNutrex.UI;

// =====================================================
// CLASE PRINCIPAL DE LA APLICACIÓN
// =====================================================
// Representa el punto de inicio de la UI en Avalonia
public partial class App : Application
{
    // =====================================================
    // MÉTODO INITIALIZE
    // =====================================================
    // Carga los componentes definidos en XAML
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // =====================================================
    // MÉTODO ON FRAMEWORK INITIALIZATION COMPLETED
    // =====================================================
    // Se ejecuta cuando la app termina de inicializarse
    public override void OnFrameworkInitializationCompleted()
    {
        // Verifica si la app es de tipo escritorio
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Define la ventana principal (Login)
            desktop.MainWindow = new LoginWindow();
        }

        // Llama a la implementación base
        base.OnFrameworkInitializationCompleted();
    }
}