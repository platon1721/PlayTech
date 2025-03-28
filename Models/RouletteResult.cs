namespace RouletteApp.Models
{
    public class RouletteResult
    {
        public int Position { get; set; }
        public int Multiplier { get; set; }
        public string Color { get; set; }
        
        public RouletteResult(int position, int streak)
        {
            Position = position;
            // Arvuta multiplier valemi M = R * N järgi
            Multiplier = position * streak;
            
            // Määra värv vastavalt positsioonile
            if (position == 0)
            {
                Color = "Green";
            }
            else if (position == 1 || position == 3 || position == 5 || 
                     position == 7 || position == 9 || position == 12 || 
                     position == 14 || position == 16 || position == 18 || 
                     position == 19 || position == 21 || position == 23 || 
                     position == 25 || position == 27 || position == 30 || 
                     position == 32 || position == 34 || position == 36)
            {
                Color = "Red";
            }
            else
            {
                Color = "Black";
            }
        }
    }
}