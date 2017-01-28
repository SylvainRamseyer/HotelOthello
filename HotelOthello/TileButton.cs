using System.Windows.Controls;
using System.Windows.Media;

namespace HotelOthello
{
    internal class TileButton : Button
    {
        int owner,x,y;
        MainWindow ui;
        bool isPlayable;

        public int Owner
        {
            get { return owner; }
            set {
                if (value == 1)
                    B();
                else if (value == 0)
                    W();
            }
        }


        public bool IsPlayable
        {
            set {
                isPlayable = value;
                if (isPlayable)
                    Background = new SolidColorBrush(Color.FromArgb(40, 0, 255, 30));
                else
                    Background = Brushes.Transparent;
            }
        }

        public TileButton(int x, int y, MainWindow parent)
        {
            this.x = x;
            this.y = y;
            this.ui = parent;

            Name = $"Button{x}{y}";

            Grid.SetColumn(this, x);
            Grid.SetRow(this, y);

            owner = -1;
        }

        public void B()
        {
            owner = 1;
            Background = Brushes.Black;
        }

        public void W()
        {
            owner = 0;
            Background = Brushes.White;
        }
        
        protected override void OnClick()
        {
            if(isPlayable) 
                ui.play(x,y);
        }
    }
}