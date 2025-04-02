using System.Collections.ObjectModel;
using Serilog;

namespace Models;

public class ResultTracker
{
    private readonly int maxSize;
    private ObservableCollection<RouletteResult> _results;
    private readonly ILogger _logger;

    public ObservableCollection<RouletteResult> Results => _results;
    public RouletteResult? LastResult { get; private set; }

    public ResultTracker(int maxSize = 10)
    {
        this.maxSize = maxSize;
        _results = new ObservableCollection<RouletteResult>();
        _logger = Log.Logger;
        _logger.Information("ResultTracker initialized");
    }

    public void AddResult()
    {
        
        _logger.Information("Adding new random result");
        
        RouletteResult newResult = LastResult == null ? 
            new RouletteResult() : 
            new RouletteResult(LastResult);
            
        _results.Add(newResult);
        _logger.Information("New result is added: Position={Position}, Multiplier={Multiplier}, Color={Color}", 
            newResult.Position, newResult.Multiplier, newResult.Color);

        if (_results.Count > maxSize)
        {
            var removedResult = _results[0];
            _results.RemoveAt(0); // Remove the oldest result
            _logger.Debug("The oldest result is removed: Position={Position}, Multiplier={Multiplier}, Color={Color}", 
                removedResult.Position, removedResult.Multiplier, removedResult.Color);
        }

        LastResult = newResult; // Renew last added result info
    }
}