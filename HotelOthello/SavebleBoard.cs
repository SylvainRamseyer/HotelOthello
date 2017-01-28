
using System;

namespace HotelOthello
{
    class SavebleBoard
    {
        public int CurrentPlayer { get; set; }
        public TimeSpan BlackTimer { get; set; }
        public TimeSpan WhiteTimer { get; set; }
        public int[,] Tiles { get; set; }
    }
}
