using System.Text;

namespace HotelOthello
{
    public class Board
    {
        private Tile[,] tiles;

        public Tile[,] Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        public Board()
        {
            tiles = new Tile[8, 8];
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            for (int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++)
                {
                    str.Append(tiles[i, j].ToString());
                    str.Append(" ");
                }
                str.Append("\n");
            }
            return str.ToString();
        }

    }
}