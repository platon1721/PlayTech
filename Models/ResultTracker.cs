using System.Collections.ObjectModel;

namespace Models;

public class ResultTracker
{
    private readonly int maxSize;
    private ObservableCollection<RouletteResult> _results;

    public ObservableCollection<RouletteResult> Results => _results;
    public RouletteResult? LastResult { get; private set; }

    public ResultTracker(int maxSize = 10)
    {
        this.maxSize = maxSize;
        _results = new ObservableCollection<RouletteResult>();
    }

    public void AddResult()
    {
        RouletteResult newResult = LastResult == null ? 
            new RouletteResult() : 
            new RouletteResult(LastResult);
            
        _results.Add(newResult);

        if (_results.Count > maxSize)
        {
            _results.RemoveAt(0); // Remove the oldest result
        }

        LastResult = newResult; // Renew last added result info
    }
}