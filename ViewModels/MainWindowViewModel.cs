using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using ReactiveUI;
using RouletteApp.Models;
using RouletteApp.Services;
using RouletteApp.ViewModels;

namespace ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<RouletteResult> _results;
        private bool _isNotificationVisible;
        private string _notificationText = string.Empty;
        private Statistics _statistics;
        private Random _random;
        private int _currentStreak;
        private string _lastColor = string.Empty;
        private readonly TcpListenerService _tcpListenerService;

        public ObservableCollection<RouletteResult> Results
        {
            get => _results;
            set => this.RaiseAndSetIfChanged(ref _results, value);
        }

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

        public ReactiveCommand<Unit, Unit> AddRandomResultCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowNotificationCommand { get; }

        public MainWindowViewModel()
        {
            _random = new Random();
            _results = new ObservableCollection<RouletteResult>();
            _statistics = new Statistics();
            _currentStreak = 0;
            
            AddRandomResultCommand = ReactiveCommand.Create(AddRandomResult);
            ShowNotificationCommand = ReactiveCommand.Create(ShowNotification);
            
            // Käivita TCP kuulaja
            _tcpListenerService = new TcpListenerService();
            _tcpListenerService.StatisticsReceived += OnStatisticsReceived;
            _tcpListenerService.Start();
        }

        private void AddRandomResult()
        {
            // Genereeri juhuslik tulemus 0-36
            int position = _random.Next(37);
            
            // Määra värv
            string color = GetRouletteColor(position);
            
            // Arvuta streaki
            if (_results.Count > 0 && color == _lastColor)
            {
                _currentStreak++;
            }
            else
            {
                _currentStreak = 1;
                _lastColor = color;
            }
            
            // Loo uus tulemus
            var result = new RouletteResult(position, _currentStreak);
            
            // Lisa tulemuste nimekirja
            Results.Add(result);
            
            // Kui tulemusi on rohkem kui 10, eemalda vanim
            if (Results.Count > 10)
            {
                Results.RemoveAt(0);
            }
        }

        private string GetRouletteColor(int position)
        {
            if (position == 0)
            {
                return "Green";
            }
            
            bool isRed = position == 1 || position == 3 || position == 5 || 
                position == 7 || position == 9 || position == 12 || 
                position == 14 || position == 16 || position == 18 || 
                position == 19 || position == 21 || position == 23 || 
                position == 25 || position == 27 || position == 30 || 
                position == 32 || position == 34 || position == 36;
                
            return isRed ? "Red" : "Black";
        }

        private void ShowNotification()
        {
            NotificationText = "Player VIP PlayerName has joined the table.";
            IsNotificationVisible = true;
    
            // Pärast 5 sekundit peida teade
            var timer = new System.Timers.Timer(5000); // Täpsusta, et kasutad System.Timers.Timer
            timer.Elapsed += (s, e) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    IsNotificationVisible = false;
                });
                timer.Stop();
            };
    
            timer.AutoReset = false; // Veendumaks, et taimer käivitub ainult ühe korra
            timer.Start();
        }

        private void OnStatisticsReceived(object? sender, Statistics statistics)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (statistics.ActivePlayers > 0)
                {
                    Statistics.ActivePlayers = statistics.ActivePlayers;
                }
                
                if (statistics.BiggestMultiplier > 0)
                {
                    Statistics.BiggestMultiplier = statistics.BiggestMultiplier;
                }
            });
        }
    }
}