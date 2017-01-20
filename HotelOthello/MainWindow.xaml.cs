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
        OthelloGame game;

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

            game = new OthelloGame();

            // display board
            display();

        }

        private void display()
        {
            for(int y=0; y<8; y++)
            {
                for(int x=0; x<8; x++)
                {                 
                    tiles[x, y].IsPlayable = game.IsPlayable(x, y);
                    tiles[x, y].Owner = game.Tiles[x, y];
                }
            }
            this.label.Content = game.CurrentPlayer == 1 ? "Blacks turn" : "Whites turn";
        }

        internal void play(int x, int y)
        {
            // is this move valid ?
            if (game.PlayMove(x, y))
                display();
        }

        private void buttonUndo_Click(object sender, RoutedEventArgs e)
        {
            game.Undo();
            display();
        }
    }
}
