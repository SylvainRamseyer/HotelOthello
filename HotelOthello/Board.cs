using System.Text;

namespace HotelOthello
{
    public class Board
    {
        private Tile[,] tiles;
        public enum Pawn { WHITE, BLACK};

        public Tile[,] Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        public Board()
        {
            tiles = new Tile[8, 8];
            tiles[3, 4].Black = tiles[4, 3].Black = true;
            tiles[3, 3].White = tiles[4, 4].White = true;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("  0 1 2 3 4 5 6 7\n");
            for (int i=0; i<8; i++)
            {
                str.Append($"{i} ");
                for (int j=0; j<8; j++)
                {
                    str.Append($"{tiles[i, j]} ");
                }
                str.Append("\n");
            }
            return str.ToString();
        }

        public bool IsPlayable(int x, int y, Pawn player)
        {

            if (tiles[x,y].IsTaken)
                return false;

            // TODO regarder si les conditions fonctionne pour joué là

            return true;
        }

        public void Play(int x, int y, Pawn player)
        {
            // TODO changer de face les pions encadré
        }
    }
}