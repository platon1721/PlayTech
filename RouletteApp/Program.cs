using Avalonia;
using System;

namespace RouletteApp
{
    class Program
    {
        // Avalonia configuration, used to configure your application
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        // The entry point - this is where the application starts
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }
}