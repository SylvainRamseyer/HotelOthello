namespace HotelOthello
{
    internal class Tile
    {
        private bool isTaken;

        public bool IsTaken
        {
            get { return isTaken; }
            set { isTaken = value; }
        }

        private bool isWhite;

        public bool IsWhite
        {
            get { return isWhite; }
            set { isWhite = value; }
        }
        
    }
}