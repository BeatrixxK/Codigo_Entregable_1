<<<<<<< Updated upstream
﻿using System;
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
=======
﻿using Avalonia;
using System;

namespace DragonNutrex.UI;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
>>>>>>> Stashed changes
