namespace HotelOthello
{
    internal class Board
    {
        private int[,] tiles;

        public int[,] Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        public Board()
        {
        }
    }
}