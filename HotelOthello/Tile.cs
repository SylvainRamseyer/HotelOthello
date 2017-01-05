using System;

namespace HotelOthello
{
    public struct Tile
    {
        private bool white;
        private bool black;

        public bool White
        {
            get { return white; }
            set { white = value; black = !value; }
        }

        public bool Black
        {
            get { return black; }
            set { black = value; white = !value; }
        }

        public bool IsTaken
        {
            get { return white || black; }
        }

        // juste des raccourcis
        public void B() { Black = true; }
        public void W() { White = true; }

        public void set(string player)
        {
            if (player == "white") W();
            else if (player == "black") B();
            else throw new ArgumentException();
        }

        public override string ToString()
        {
            // si white, retourne w, sinon si blanc, retourne b, sinon retourne _
            return white ? "w" : black ? "b" : "_";
        }

        /*
        private int x;
        private int y;

        public static bool operator ==(Tile t1, Tile t2)
        {
            return t1.i == t2.i && t1.j == t2.j;
        }

        public static bool operator !=(Tile t1, Tile t2)
        {
            return !(t1 == t2);
        }

        public override bool Equals(object obj)
        {
            return this == (Tile)obj;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public Move(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }
        */

    }
}
 