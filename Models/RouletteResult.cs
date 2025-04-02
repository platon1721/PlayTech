using Serilog;

namespace Models
{
    public class RouletteResult
    {
        public int Position { get; set; }
        public int Multiplier { get; set; }
        public string Color { get; set; }
        private RouletteResult?  PreviousResult { get; set; }
        private readonly ILogger _logger;
        
        public bool ShouldShowMultiplier => Multiplier > Position;

        public RouletteResult()
        {
            _logger = Log.Logger;
            var random = new Random();
            Position = random.Next(maxValue: 37);
            _logger.Information("New random result is: Position={Position}", 
                Position);
            
            Color = __getColor(Position);
            Multiplier = Position;
            
            _logger.Debug("New roulette result is: Position={Position}, Multiplier={Multiplier}, Color={Color}", 
                Position, Multiplier, Color);
        }
        
        
        public RouletteResult(RouletteResult previousResult)
        {
            _logger = Log.Logger;
            var random = new Random();
            Position = random.Next(maxValue: 37);
            _logger.Information("New random result is: Position={Position}", 
                Position);
            
            Color = __getColor(Position);
            PreviousResult = previousResult;
            _logger.Debug("Previous Result is: Position={Position}, Color={Color}",
                PreviousResult.Position, PreviousResult.Color);
            
            if (PreviousResult != null && PreviousResult.Color == Color)
            {
                Multiplier = (PreviousResult.Multiplier / PreviousResult.Position + 1) * Position;
                _logger.Information("The same color in a row; new multiplier is:{Multiplier}", Multiplier);
            }
            else
            {
                Multiplier = Position;
                _logger.Debug("New color; multiplier is 1.");
            }
            _logger.Debug("Roulette result is: Position={Position}, Multiplier={Multiplier}, Color={Color}", 
                Position, Multiplier, Color);
        }


        private static string __getColor(int position)
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
    }
}
        