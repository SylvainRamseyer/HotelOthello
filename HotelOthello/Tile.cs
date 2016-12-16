namespace HotelOthello
{
    public struct Tile
    {
        private bool white;

        public bool White
        {
            get { return white; }
            set { white = value; }
        }

        private bool black;

        public bool Black
        {
            get { return black; }
            set { black = value; }
        }

        public bool isTaken()
        {
            return white || black;
        }

        public override string ToString()
        {
            // si white, retourne w, sinon si blanc, retourne b, sinon retourne _
            return white ? "w" : black ? "b" : "_";
        }

    }
}