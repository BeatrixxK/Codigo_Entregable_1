using System;
using Avalonia;
using DragonNutrex.App.Utils;

namespace DragonNutrex.UI;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // DataSeeder.GenerarDatos(); // solo si ocupas regenerar datos una vez

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR FATAL EN PROGRAM:");
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}