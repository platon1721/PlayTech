using System.Collections.ObjectModel;
using Serilog;

namespace Models;


/// <summary>
/// Manages a collection of roulette results with size limitations.
/// </summary>
public class ResultTracker
{
    private readonly int maxSize;
    private ObservableCollection<RouletteResult> _results;
    private readonly ILogger _logger;

    
    // Gets the collection of tracked roulette results.
    public ObservableCollection<RouletteResult> Results => _results;
    
    // Gets the most recently added result.
    public RouletteResult? LastResult { get; private set; }

    /// <summary>
    /// Initializes a new tracker with specified maximum size.
    /// </summary>
    /// <param name="maxSize">Maximum number of results to maintain</param>
    public ResultTracker(int maxSize = 10)
    {
        this.maxSize = maxSize;
        _results = new ObservableCollection<RouletteResult>();
        _logger = Log.Logger;
        _logger.Information("ResultTracker initialized");
    }

    
    /// <summary>
    /// Adds a new random result to the collection, removing oldest result when size exceeds maximum.
    /// </summary>
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