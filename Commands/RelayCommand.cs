using System.Windows.Input;

namespace Commands
{
    
    /// <summary>
    /// Implements ICommand for MVVM pattern, delegating execution to provided actions.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        
        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">Action to execute when command is invoked</param>
        /// <param name="canExecute">Function determining if command can execute (optional)</param>
        /// <exception cref="ArgumentNullException">Thrown when execute is null</exception>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determines if command can execute based on provided function.
        /// </summary>
        /// <returns>True if no canExecute function provided or it returns true</returns>
        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        
        // Executes the command's action.
        public void Execute(object? parameter) => _execute();

        // Event raised when command execution status changes.
        public event EventHandler? CanExecuteChanged;

        
        // Raises CanExecuteChanged event to notify command status changed.
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}