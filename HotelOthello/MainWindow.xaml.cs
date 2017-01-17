using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HotelOthello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TileButton[,] tiles = new TileButton[8, 8];
        int currentPlayer = 1;

        public MainWindow()
        {
            InitializeComponent();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    TileButton newBtn = new TileButton(x, y, this);
                    this.grid.Children.Add(newBtn);
                    tiles[x, y] = newBtn;
                }
            }

            tiles[3, 4].B(); tiles[4, 3].B();
            tiles[3, 3].W(); tiles[4, 4].W();

        }

        internal void play(int x, int y)
        {
            tiles[x,y].Owner = currentPlayer;
            currentPlayer = 1 - currentPlayer;
            this.label.Content = currentPlayer == 1 ? "Blacks turn" : "Whites turn";
        }
    }
}
