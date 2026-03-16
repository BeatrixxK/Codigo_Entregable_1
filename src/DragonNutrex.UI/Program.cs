/* archivo donde empieza a ejecutarse la aplicación.
✔ crea usuario
✔ guarda JSON
✔ lista usuarios 
CRUD FUNCIONAL*/

using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace DragonNutrex.UI;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}