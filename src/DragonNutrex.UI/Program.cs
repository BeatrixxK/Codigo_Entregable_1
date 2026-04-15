using System;
using Avalonia;

namespace DragonNutrex.UI;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                Console.WriteLine("ERROR NO CONTROLADO:");
                Console.WriteLine(e.ExceptionObject?.ToString());
            };

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR FATAL EN PROGRAM:");
            Console.WriteLine(ex.ToString());

            if (ex.InnerException != null)
            {
                Console.WriteLine("INNER EXCEPTION:");
                Console.WriteLine(ex.InnerException);
            }

            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}