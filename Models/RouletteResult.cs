namespace Models
{
    public class RouletteResult
    {
        public int Position { get; set; }
        public int Multiplier { get; set; }
        public string Color { get; set; }
        public RouletteResult?  PreviousResult { get; set; }

        public RouletteResult()
        {
            var random = new Random();
            Position = random.Next(maxValue: 37);
            Color = __getColor(Position);
            Multiplier = Position;
        }

        public RouletteResult(RouletteResult previousResult)
        {
            var random = new Random();
            Position = random.Next(maxValue: 37);
            Color = __getColor(Position);
            PreviousResult = previousResult;
            if (PreviousResult != null && PreviousResult.Color == Color)
            {
                Multiplier = (PreviousResult.Multiplier / PreviousResult.Position + 1) * Position;
            }
            else
            {
                Multiplier = Position;
            }
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
        