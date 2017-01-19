using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        TileButton[,] tiles = new TileButton[OthelloGame.SIZE, OthelloGame.SIZE];
        OthelloGame game;

        public MainWindow()
        {
            InitializeComponent();

            for (int y = 0; y < OthelloGame.SIZE; y++)
            {
                for (int x = 0; x < OthelloGame.SIZE; x++)
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
            for(int y=0; y< OthelloGame.SIZE; y++)
            {
                for(int x=0; x< OthelloGame.SIZE; x++)
                {
                    tiles[x, y].IsPlayable = game.IsPlayable(x, y);
                    tiles[x, y].Owner = game.Tiles[x, y];
                }
            }
            this.label.Content = $"{game.PlayerColor}s turn";
        }

        internal void play(int x, int y)
        {
            // tente de jouer le mouvement
            if (game.PlayMove(x, y))
            {
                // si le mouvement est valide, affiche la nouvelle disposition du jeu
                display();

                // si il n'y a plus de mouvement possibles pour le prochain joueur
                if (game.PossibleMoves.Count == 0)
                {
                    // change de joueur
                    game.ChangePlayer();
                    // calcul ses possibilités
                    game.ComputePossibleMoves();
                    // si lui non plus n'a pas de possibilités, c'est la fin du jeu
                    if (game.PossibleMoves.Count == 0)
                    {
                        MessageBoxResult result = MessageBox.Show(
                            "Would you like to play again ?", "GAME OVER",
                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            game = new OthelloGame();
                            display();
                        }
                        else
                        {
                            Application.Current.Shutdown();
                        }
                    }
                    else
                    {
                        // si l'autre joueur peut jouer, affiche un bouton pour passer au tour suivant
                        // todo : un autre message box ?
                        btn_pass.Opacity = 100;
                    }
                }
            }
        }

        private void btn_pass_Click(object sender, RoutedEventArgs e)
        {

            btn_pass.Opacity = 0;
            display();
        }
    }
}
