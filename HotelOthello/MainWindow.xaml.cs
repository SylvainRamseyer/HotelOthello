using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;

namespace HotelOthello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TileButton[,] tiles = new TileButton[OthelloGame.SIZE_GRID, OthelloGame.SIZE_GRID];
        OthelloGame game;

        public MainWindow()
        {
            InitializeComponent();

            // Crée et place les tuiles dans l'interface graphique
            for (int y = 0; y < OthelloGame.SIZE_GRID; y++)
            {
                for (int x = 0; x < OthelloGame.SIZE_GRID; x++)
                {
                    TileButton newBtn = new TileButton(x, y, this);
                    // Ajoute le bouton à la grille
                    gridOthello.Children.Add(newBtn);
                    tiles[x, y] = newBtn;
                }
            }

            // Initialise le jeu
            game = new OthelloGame();
            // Défini le contexte de données pour le databinding
            DataContext = game;
            
            // Actualise les tuiles en fonction du jeu
            display();

        }

        private void display()
        {
            for(int y=0; y < OthelloGame.SIZE_GRID; y++)
            {
                for(int x=0; x < OthelloGame.SIZE_GRID; x++)
                {
                    // Change la couleur de la tuile en fonction du jeu
                    tiles[x, y].Owner = game.Tiles[x, y];
                    // Met en évidence les tuiles jouables
                    tiles[x, y].IsPlayable = game.IsPlayable(x, y);
                }
            }
            // Actualise le cercle qui indique quelle couleur doit jouer
            circle.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(game.PlayerColor);
        }

        internal void play(int x, int y)
        {
            // tente de jouer le mouvement
            if (game.PlayMove(x, y))
            {
                // si le mouvement est valide, affiche la nouvelle disposition du jeu
                display();

                // si il n'y a plus de mouvement possibles pour le prochain joueur
                if (! game.CanMove)
                {
                    // change de joueur
                    game.ChangePlayer();
                    // calcul ses possibilités
                    game.ComputePossibleMoves();
                    // si lui non plus n'a pas de possibilités, c'est la fin du jeu
                    if (!game.CanMove)
                    {
                        game.StopTimer();
                        
                        MessageBoxResult result = MessageBox.Show(
                            $"{game.GetWinnerString()}\nWould you like to play again ?", "GAME OVER",
                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            game = new OthelloGame();
                            DataContext = game;
                            display();
                        }
                    }
                    else
                    {
                        // si l'autre joueur peut jouer, affiche un bouton pour passer au tour suivant
                        btn_pass.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void btn_pass_Click(object sender, RoutedEventArgs e)
        {
            // le joueur bloqué a cliqué sur Pass, on rafraichit simplement l'affichage en fonction de l'état de jeu
            btn_pass.Visibility = Visibility.Hidden;
            display();
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            game.StopTimer();
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Json files (*.json)|*.json";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog.FileName != "")
            {
                game.Save(saveFileDialog.FileName);
            }
            game.RestartTimer();
        }

        private void menuLoad_Click(object sender, RoutedEventArgs e)
        {
            game.StopTimer();
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json files (*.json)|*.json";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            openFileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (openFileDialog.FileName != "")
            {
                game.Load(openFileDialog.FileName);
                display();
            }
            game.RestartTimer();
        }

        private void menuUndo_Click(object sender, RoutedEventArgs e)
        {
            game.Undo();
            display();
        }

        private void menuPause_Click(object sender, RoutedEventArgs e)
        {
            game.StopTimer();
            // cache le pllateau de jeu pour éviter l'abus du bouton pour du rab de temps de reflexion
            gridOthello.Visibility = Visibility.Hidden;
            MessageBoxResult result = MessageBox.Show(
                "Game paused, Click OK to continue", "PAUSE");
            gridOthello.Visibility = Visibility.Visible;
            game.RestartTimer();
        }
    }
}
