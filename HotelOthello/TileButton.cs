using System.Windows.Controls;
using System.Windows.Media;

namespace HotelOthello
{
    /// <summary>
    /// Customisation du Contrôle bouton standard.
    /// </summary>
    internal class TileButton : Button
    {
        int owner,x,y;
        MainWindow ui;
        bool isPlayable;

        // tableau qui contient les 4 backgrounds possibles pour un bouton
        Brush[] BRUSHES = { Brushes.Transparent, Brushes.White, Brushes.Black, new SolidColorBrush(Color.FromArgb(40, 0, 255, 30)) };

        public int Owner
        {
            get { return owner; }
            set
            {
                owner = value;
                // -1 -> transparent, 0 -> blanc, 1 -> noir
                Background = BRUSHES[owner+1];
            }
        }


        public bool IsPlayable
        {
            set
            {
                isPlayable = value;
                if (isPlayable)
                    Background = BRUSHES[3];
            }
        }

        public TileButton(int x, int y, MainWindow parent)
        {
            this.x = x;
            this.y = y;
            this.ui = parent;
            
            Grid.SetColumn(this, x);
            Grid.SetRow(this, y);

            owner = -1;
        }
        
        protected override void OnClick()
        {
            if(isPlayable) 
                ui.play(x,y);
        }
    }
}