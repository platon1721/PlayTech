namespace Services
{
    // Timer interface for dependency injection
    public interface ITimer
    {
        TimeSpan Interval { get; set; }
        event EventHandler Tick;
        void Start();
        void Stop();
    }
    
    // Avalonia DispatcherTimer implementation
    public class AvaloniaTimer : ITimer
    {
        private readonly Avalonia.Threading.DispatcherTimer _timer;
        
        public AvaloniaTimer()
        {
            _timer = new Avalonia.Threading.DispatcherTimer();
            _timer.Tick += (s, e) => Tick?.Invoke(this, e);
        }
        
        public TimeSpan Interval 
        { 
            get => _timer.Interval;
            set => _timer.Interval = value;
        }
        
        public event EventHandler Tick;
        
        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
    }
}