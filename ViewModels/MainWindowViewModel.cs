
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Threading;
using Commands;
using Models;
using ReactiveUI;
using RouletteApp.ViewModels;
using Serilog;
using Services;

namespace ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isNotificationVisible;
        private string _notificationText = string.Empty;
        private Statistics _statistics;
        private ResultTracker _resultTracker;
        public ObservableCollection<RouletteResult> Results => _resultTracker.Results;
        
        private readonly TcpListenerService _tcpListenerService;
        private readonly ILogger _logger;
        
        public bool IsNotificationVisible
        {
            get => _isNotificationVisible;
            set => this.RaiseAndSetIfChanged(ref _isNotificationVisible, value);
        }

        public string NotificationText
        {
            get => _notificationText;
            set => this.RaiseAndSetIfChanged(ref _notificationText, value);
        }

        public Statistics Statistics
        {
            get => _statistics;
            set => this.RaiseAndSetIfChanged(ref _statistics, value);
        }

        public ICommand AddRandomResultCommand { get; }
        public ICommand ShowNotificationCommand { get; }

        public MainWindowViewModel()
        {
            
            _logger = Log.Logger;
            _logger.Information("MainWindowViewModel starting...");
            _statistics = new Statistics();
            _resultTracker = new ResultTracker();
            
            // RelayCommand
            AddRandomResultCommand = new RelayCommand(() => 
                Dispatcher.UIThread.Post(AddRandomResult));
                
            ShowNotificationCommand = new RelayCommand(() => 
                Dispatcher.UIThread.Post(ShowNotification));
            
            // TCP start
            _tcpListenerService = new TcpListenerService();
            _tcpListenerService.StatisticsReceived += OnStatisticsReceived;
            _tcpListenerService.Start();
            _logger.Information("TcpListenerService started.");
            _logger.Information("MainWindowViewModel started.");
        }

        private void AddRandomResult()
        {
            _logger.Information("Adding random result...");
            _resultTracker.AddResult();
            _logger.Information("New random result is added: Position={Position}, Multiplier={Multiplier}, Color={Color}", 
                _resultTracker.LastResult?.Position, 
                _resultTracker.LastResult?.Multiplier, 
                _resultTracker.LastResult?.Color);
        }

        private void ShowNotification()
        {
            _logger.Information("Showing notification...");
            NotificationText = "Player VIP PlayerName has joined the table.";
            IsNotificationVisible = true;
    
            // After 5sec remove
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            
            timer.Tick += (s, e) =>
            {
                IsNotificationVisible = false;
                timer.Stop();
                _logger.Information("Notification timed out.");
            };
            
            timer.Start();
        }

        private void OnStatisticsReceived(object? sender, Statistics statistics)
        {
            _logger.Information("New statistics received.");
            Statistics = new Statistics
            {
                ActivePlayers = statistics.ActivePlayers > 0 ? statistics.ActivePlayers : Statistics.ActivePlayers,
                BiggestMultiplier = statistics.BiggestMultiplier > 0 ? statistics.BiggestMultiplier : Statistics.BiggestMultiplier
            };
    
            _logger.Information("Statistics updated in ViewModel");
        }
    }
}