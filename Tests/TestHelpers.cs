using System;
using Services;
using ITimer = Services.ITimer;

namespace Tests
{
    /// <summary>
    /// Dispatcher for testing
    /// </summary>
    public class TestDispatcher : IDispatcher
    {
        public void Post(Action action) => action();
    }
    
    /// <summary>
    /// Timer for testing
    /// </summary>
    public class TestTimer : ITimer
    {
        public TimeSpan Interval { get; set; }
        public event EventHandler Tick;
        
        public bool IsRunning { get; private set; }
        
        public void Start() 
        { 
            IsRunning = true;
        }
        
        public void Stop() 
        { 
            IsRunning = false;
        }
        
        // Method for triggering the timer manually
        public void FireTick()
        {
            if (IsRunning)
            {
                Tick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}