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

        public override string ToString()
        {
            // si white, retourne w, sinon si blanc, retourne b, sinon retourne _
            return white ? "w" : black ? "b" : "_";
        }

    }
}