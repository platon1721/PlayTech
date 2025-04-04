using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Serilog;
using Views;
using ViewModels;

namespace RouletteApp
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            
            Log.Information("Roulette app started. Avalonia xaml loaded.");
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                
                desktop.Exit += (s, e) => {
                    Log.Information("Application closed.");
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}