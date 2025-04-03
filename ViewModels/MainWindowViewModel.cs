
using System.Collections.ObjectModel;
using System.Windows.Input;
using Commands;
using Models;
using ReactiveUI;
using Serilog;
using Services;
using IDispatcher = Services.IDispatcher;
using ITimer = Services.ITimer;

namespace ViewModels
{
    
    /// <summary>
    /// Main view model that manages roulette results, statistics, and notifications.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isNotificationVisible;
        private string _notificationText = string.Empty;
        private Statistics _statistics;
        private ResultTracker _resultTracker;
        private ITimer _notificationTimer;
        
        
        // Gets the collection of roulette results.
        public ObservableCollection<RouletteResult> Results => _resultTracker.Results;
        
        private readonly TcpListenerService _tcpListenerService;
        private readonly ILogger _logger;
        private readonly IDispatcher _dispatcher;
        private readonly Func<ITimer> _timerFactory;
        
        // For testing
        public ITimer CurrentNotificationTimer => _notificationTimer;
        
        /// <summary>
        /// Gets or sets whether the notification is currently visible.
        /// </summary>
        public bool IsNotificationVisible
        {
            get => _isNotificationVisible;
            set => this.RaiseAndSetIfChanged(ref _isNotificationVisible, value);
        }

        /// <summary>
        /// Gets or sets the notification text to display.
        /// </summary>
        public string NotificationText
        {
            get => _notificationText;
            set => this.RaiseAndSetIfChanged(ref _notificationText, value);
        }

        
        /// <summary>
        /// Gets or sets the current game statistics.
        /// </summary>
        public Statistics Statistics
        {
            get => _statistics;
            set => this.RaiseAndSetIfChanged(ref _statistics, value);
        }

        // Command to add a random roulette result.
        public ICommand AddRandomResultCommand { get; }
        
        // Command to display a notification message.
        public ICommand ShowNotificationCommand { get; }

        
        /// <summary>
        /// Initializes a new view model with optional dependencies.
        /// </summary>
        /// <param name="logger">Logger for tracking operations</param>
        /// <param name="dispatcher">UI thread dispatcher</param>
        /// <param name="timerFactory">Factory for creating timers</param>
        public MainWindowViewModel(ILogger? logger = null, IDispatcher? dispatcher = null, Func<ITimer>? timerFactory = null)
        {
            
            _logger = logger ?? Log.Logger;
            _dispatcher = dispatcher ?? new AvaloniaDispatcher();
            _timerFactory = timerFactory ?? (() => new AvaloniaTimer());
            
            _logger.Information("MainWindowViewModel starting...");
            _statistics = new Statistics
            {
                ActivePlayers = 0,
                BiggestMultiplier = 0
            };
            _resultTracker = new ResultTracker();
            
            // RelayCommand
            AddRandomResultCommand = new RelayCommand(() => 
                _dispatcher.Post(AddRandomResult));
                
            ShowNotificationCommand = new RelayCommand(() => 
                _dispatcher.Post(ShowNotification));
            
            // TCP start
            _tcpListenerService = new TcpListenerService();
            _tcpListenerService.StatisticsReceived += OnStatisticsReceived;
            _tcpListenerService.Start();
            _logger.Information("TcpListenerService started.");
            _logger.Information("MainWindowViewModel started.");
        }

        
        // Adds a new random roulette result to the tracker.
        private void AddRandomResult()
        {
            _logger.Information("Adding random result...");
            _resultTracker.AddResult();
            _logger.Information("New random result is added: Position={Position}, Multiplier={Multiplier}, Color={Color}", 
                _resultTracker.LastResult?.Position, 
                _resultTracker.LastResult?.Multiplier, 
                _resultTracker.LastResult?.Color);
            Console.WriteLine(_resultTracker.Results.Count);
        }

        
        // Displays a player notification with an auto-dismiss timer.
        private void ShowNotification()
        {
            _logger.Information("Showing notification...");
            NotificationText = "Player VIP PlayerName has joined the table.";
            IsNotificationVisible = true;
            
            // New timer
            _notificationTimer = _timerFactory();
            _notificationTimer.Interval = TimeSpan.FromSeconds(5);
            
            
            _notificationTimer.Tick += (s, e) =>
            {
                _dispatcher.Post(() => {
                    IsNotificationVisible = false;
                    _notificationTimer.Stop();
                    _logger.Information("Notification timed out.");
                });
            };
            
            _notificationTimer.Start();
        }
        
        
        /// <summary>
        /// Handles statistics updates received from the TCP service.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="statistics">New statistics data</param>
        private void OnStatisticsReceived(object? sender, Statistics statistics)
        {
            _logger.Information("New statistics received.");
    
            Statistics = new Statistics
            {
                // If active players statistics null, uses old statistics
                ActivePlayers = statistics.ActivePlayers.HasValue ? statistics.ActivePlayers : Statistics.ActivePlayers,
        
                // If BiggestMultiplier statistics is null, uses old statistics
                BiggestMultiplier = statistics.BiggestMultiplier.HasValue ? statistics.BiggestMultiplier : Statistics.BiggestMultiplier
            };

            _logger.Information("Statistics updated in ViewModel");
        }
    }
}