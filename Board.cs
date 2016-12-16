namespace HotelOthello
{
    internal class Board
    {
        private Tile[,] tiles;

        public Tile[,] Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        public Board()
        {
        }
    }
}