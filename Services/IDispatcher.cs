using Avalonia.Threading;

namespace Services
{
    // Dispatcher interface
    public interface IDispatcher
    {
        void Post(Action action);
    }
    
    // Avalonia Dispatcher implementation
    public class AvaloniaDispatcher : IDispatcher
    {
        public void Post(Action action) => Dispatcher.UIThread.Post(action);
    }
}