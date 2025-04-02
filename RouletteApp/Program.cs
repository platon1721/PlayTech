using Avalonia;
using Serilog;

namespace RouletteApp
{
    class Program
    {
        // Avalonia config
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
        
        public static void Main(string[] args)
        {
            
            Console.WriteLine($"Rakenduse töökaust: {Directory.GetCurrentDirectory()}");
            Directory.CreateDirectory("logs");
            
            // Setting Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/roulette-app.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Application started");
                
                // Run Avalonia app
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                // End Serilog
                Log.CloseAndFlush();
            }
        }
    }
}