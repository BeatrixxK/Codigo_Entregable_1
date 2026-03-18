using System;
using Avalonia;
using DragonNutrex.App.Utils;

namespace DragonNutrex.UI;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
      //  DataSeeder.GenerarDatos();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}