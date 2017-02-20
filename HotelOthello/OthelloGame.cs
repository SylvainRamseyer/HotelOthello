using Newtonsoft.Json;
using OthelloConsole;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Timers;

namespace HotelOthello
{
    public class OthelloGame : INotifyPropertyChanged, IPlayable
    {
        // Permet le databinding. Cet évènement est invoqué dans la méthode RaisePropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private Stack<int[,]> history;
        public const int SIZE_GRID = 8;

        // tableau 2d d'entiers représentant le plateau de jeu
        // -1 : case libre
        //  0 : blanc
        //  1 : noir
        private int[,] tiles;
        public int[,] Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        // clé: string représentant un mouvement, exemple : "07"
        // valeur: liste des tuiles capturées si ce mouvement est effectué
        private Dictionary<String,List<Tuple<int, int>>> possibleMoves;
        public bool CanMove { get { return possibleMoves.Count > 0; } }

        private int currentPlayer = 1; // 1=black
        public int CurrentPlayer{ get { return currentPlayer; } }

        // raccourci pour obtenir la couleur qui doit jouer
        public string PlayerColor
        {
            get{ return CurrentPlayer == 1 ? "Black" : "White";}
        }
        
        private int[] scores = { 2, 2 };

        // Ces deux propriétés Lancent l'évènement PropertyChanged qui actualise l'affichage
        public int BlacksScore
        {
            get { return scores[1]; }
            set { scores[1] = value; RaisePropertyChanged("BlacksScore"); }
        }

        public int WhitesScore
        {
            get { return scores[0]; }
            set { scores[0] = value; RaisePropertyChanged("WhitesScore"); }
        }

        // Tableau contenant les deux horloges
        TimeSpan[] chronos = { TimeSpan.Zero, TimeSpan.Zero };

        // Les "temps de références" pour chaque joueur,
        // ces temps indiquent le moment ou le tour d'un joueur commence
        DateTime[] referenceTimes = { DateTime.Now, DateTime.Now };

        // Ce timer ne sert qu'à actualiser l'affichage des chronomètres
        private Timer timer;

        string whitesTimeString = "00:00.000";
        string blacksTimeString = "";
        
        // Ces deux propriétés sont elle aussi data-bindés avec l'interface graphique
        public string WhitesTimeString
        {
            get { return whitesTimeString; }
            set { whitesTimeString = value; RaisePropertyChanged("WhitesTimeString"); }
        }

        public string BlacksTimeString
        {
            get { return blacksTimeString; }
            set { blacksTimeString = value; RaisePropertyChanged("BlacksTimeString"); }
        }
        
        // Lance l'évènement qui indique l'actualisation de la propriété qui le nom v.
        // Permet la mise à jour de l'interface graphique par data-binding
        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        // Constructeur du jeu
        public OthelloGame()
        {
            // Intialisation
            tiles = new int[SIZE_GRID, SIZE_GRID];
            history = new Stack<int[,]>();
            possibleMoves = new Dictionary<String, List<Tuple<int, int>>>();
            
            for (int i = 0; i < SIZE_GRID; i++)
            {
                for (int j = 0; j < SIZE_GRID; j++)
                {
                    tiles[i, j] = -1; // -1 : empty tile
                }
            }

            tiles[3, 4] = tiles[4, 3] = 1; // 1 : black
            tiles[3, 3] = tiles[4, 4] = 0; // 0 : white
            
            ComputePossibleMoves();

            timer = new Timer(25); // 40 rafraichissements par secondes
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Start();
        }

        /**
         * TIMER
         */

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            TimeSpan t = chronos[currentPlayer] + (DateTime.Now - referenceTimes[currentPlayer]);
            if (currentPlayer == 0)
                WhitesTimeString = timeSpanToString(t);
            else
                BlacksTimeString = timeSpanToString(t);
        }

        private static string timeSpanToString(TimeSpan t)
        {
            return String.Format("{0:D2}:{1:D2}.{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
        }

        public void StopTimer()
        {
            timer.Stop();
            chronos[currentPlayer] += (DateTime.Now - referenceTimes[currentPlayer]);
        }

        public void RestartTimer()
        {
            referenceTimes[currentPlayer] = DateTime.Now;
            timer.Start();
        }

        /**
         *  LOGIQUE DU JEU
         */

        /// <summary>
        /// Peuple le dictionnaire possibleMoves avec tous les mouvements possibles pour ce tour,
        /// pour le joueur actuel. Chaque mouvement est associé à une liste de tuples qui indique
        /// les cases capturées si le mouvement est joué.
        /// On refait ce calcul à chaque tour.
        /// </summary>
        public void ComputePossibleMoves()
        {
            possibleMoves.Clear();
            
            for (int column = 0; column < SIZE_GRID; column++)
            {
                for (int line = 0; line < SIZE_GRID; line++)
                {
                    computeMove(column, line);
                }
            }
        }

        /// <summary>
        /// Si le mouvement donné est possible, il est ajouté au dictionnaire possibleMoves.
        /// Un tuile est jouable si :
        /// - l'une de ses voisines est occupée par un ennemi
        /// - l'autre extrémité est occupée par le joueur
        /// </summary>
        private void computeMove(int column, int line)
        {
            // Retourne si la tuile est déjà occupée
            if (tiles[column, line] != -1)
                return;

            int ennemi = 1-currentPlayer;

            // Liste contenant les coordonéées des cases voisines qui sont occupées par un ennemis 
            List<Tuple<int, int>> voisins = new List<Tuple<int, int>>();

            // Parcoure les tuiles autour de cette tuile, ajoute celles qui sont occupées par un ennemi
            // à la liste des voisins
            for (int i = column - 1; i <= column + 1; i++)
            {
                for (int j = line - 1; j <= line + 1; j++)
                {
                    bool outside = i < 0 || i >= SIZE_GRID || j < 0 || j >= SIZE_GRID;
                    if (!outside && Tiles[i, j] == ennemi)
                        voisins.Add(new Tuple<int, int>(i, j));
                }
            }

            // aucune des cases voisines n'est occupée par l'ennemi, cette tuile n'est pas jouable
            if (voisins.Count == 0)
                return;

            // Cette liste contiendra toutes les cases capturées par ce mouvement
            List<Tuple<int, int>> catchedTiles = new List<Tuple<int, int>>();

            // Pour chaque voisin ennemi
            foreach (Tuple<int, int> voisin in voisins)
            {
                int x = voisin.Item1;
                int y = voisin.Item2;
                // dx et dy indiquent la direction de la ligne sur laquelles les cases ennemies seront captuées 
                int dx = x - column;
                int dy = y - line;

                // liste des cases potentiellement capturées
                // (on ne sait pas encore si l'autre extrémité de la ligne est occupée par le joueur)
                List<Tuple<int, int>> temp = new List<Tuple<int, int>>();
                try
                {
                    // on suit la ligne et on ajoute les cases tant qu'on est sur un ennemi
                    while (tiles[x, y] == ennemi)
                    {
                        temp.Add(new Tuple<int, int>(x, y));
                        // on se déplace selon la direction de la ligne
                        x = x + dx;
                        y = y + dy;
                    }
                    // on sort de la boucle while lorsque la case x,y n'est pas un ennemi,
                    // maintenant, soit c'est une case vide et on ne fait rien, soit c'est une case
                    // du joueur et donc on capture toutes les cases mise en attente dans temp
                    if (tiles[x, y] == currentPlayer)
                        catchedTiles.AddRange(temp);
                }
                catch (Exception)
                {
                    // si hors de la grille de jeux --> n'est pas valide, rien à faire
                }
            }

            // si on a rien capturé dans la boucle foreach, le coup n'est pas valide
            if (catchedTiles.Count == 0)
                return;

            // Le coup est valide, on l'ajoute au dictionnaire
            possibleMoves.Add(tupleToString(column, line), catchedTiles);
            return;
        }

        public void ChangePlayer()
        {
            // Ajoute le temps de reflexion du joueur à son chrono
            chronos[currentPlayer] += DateTime.Now - referenceTimes[currentPlayer];
            // Change de joueur
            currentPlayer = 1 - currentPlayer;
            // "Démarre" le chrono du prochain joueur
            referenceTimes[currentPlayer] = DateTime.Now;
        }

        private void score(int player, int delta)
        {
            if (player == 1)
                BlacksScore += delta;
            else if (player == 0)
                WhitesScore += delta;
        }

        /**
         *  HISTORY
         */

        public void Undo()
        {
            if (history.Count != 0)
            {
                tiles = history.Pop();
                ChangePlayer();
                updateScore();
                ComputePossibleMoves();
            }
        }

        private void SaveInHistory()
        {
            int[,] copyTab = new int[SIZE_GRID, SIZE_GRID];

            for (int i = 0; i < SIZE_GRID; i++)
            {
                for (int j = 0; j < SIZE_GRID; j++)
                {
                    copyTab[i, j] = tiles[i, j];
                }
            }

            history.Push(copyTab);
        }

        private void updateScore()
        {
            WhitesScore = BlacksScore = 0;

            for (int y = 0; y < SIZE_GRID; y++)
            {
                for (int x = 0; x < SIZE_GRID; x++)
                {
                    if (tiles[x, y] != -1)
                        score(tiles[x, y], 1);
                }
            }
        }

        public bool Save(String fileName)
        {

            SavebleBoard board = new SavebleBoard();
            board.CurrentPlayer = this.CurrentPlayer;
            board.Tiles = this.tiles;
            board.BlackTimer = this.chronos[1];
            board.WhiteTimer = this.chronos[0];

            string json = JsonConvert.SerializeObject(board);
            System.IO.File.WriteAllText(fileName, json);
            return true;
        }

        public bool Load(String fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                SavebleBoard board = JsonConvert.DeserializeObject<SavebleBoard>(json);

                Console.WriteLine(board.CurrentPlayer);
                this.tiles = board.Tiles;
                this.chronos[1] = board.BlackTimer;
                this.chronos[0] = board.WhiteTimer;
                this.currentPlayer = board.CurrentPlayer;
                this.ComputePossibleMoves();
                this.updateScore();
            }
            return true;
        }

        /**
         *  TOOLS
         */

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("  0 1 2 3 4 5 6 7\n");
            for (int y = 0; y < SIZE_GRID; y++)
            {
                str.Append($"{y} ");
                for (int x = 0; x < SIZE_GRID; x++)
                {
                    string tile = tiles[x, y] == -1 ? "_" : (tiles[x, y] == 0 ? "w" : "b");
                    if (possibleMoves.ContainsKey(tupleToString(x, y)))
                        tile = ".";
                    str.Append($"{tile} ");
                }
                str.Append("\n");
            }
            return str.ToString();
        }

        public string GetWinnerString()
        {
            if(scores[0] > scores[1])
            {
                return "White wins !";
            }
            else if(scores[0] < scores[1])
            {
                return "Black wins !";
            }
            else if (chronos[0] < chronos[1])
            {
                return "Ex-aequo, but White has a better time";
            }
            else if (chronos[0] > chronos[1])
            {
                return "Ex-aequo, but Black has a better time";
            }
            else
            {
                return "Completely Ex-aequo... that's amazing !";
            }
        }

        private String tupleToString(Tuple<int, int> tuple)
        {
            return tupleToString(tuple.Item1, tuple.Item2);
        }

        private String tupleToString(int x, int y)
        {
            return $"{x}{y}";
        }

        /***
         * IPlayable integration
         */

        /// <summary>
        /// Returns true if the move is valid for specified color
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <returns></returns>
        public bool isPlayable(int column, int line, bool isWhite=true)
        {
            return possibleMoves.ContainsKey(tupleToString(column, line));
        }

        /// <summary>
        /// Asks the game engine next (valid) move given a game position
        /// The board assumes following standard move notation:
        /// 
        ///         A B C D E F G H
        ///       1
        ///       2
        ///       3
        ///       4
        ///       5
        ///       6
        ///       7
        ///       8
        ///       
        ///          Column Line
        ///  E.g.: D3, F4
        /// </summary>
        /// <param name="game">a 2D board with 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="level">an integer value to set the level of the IA</param>
        /// <param name="whiteTurn">true if white players turn, false otherwise</param>
        /// <returns></returns>
        public Tuple<char, int> getNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Will update the board status if the move is valid and return true
        /// Will return false otherwise (board is unchanged)
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite">true for white move, false for black move</param>
        /// <returns></returns>
        public bool playMove(int column, int line, bool isWhite=true)
        {
            if (isPlayable(column, line, isWhite))
            {
                SaveInHistory();

                // pose une pièce sur la case jouée
                tiles[column, line] = currentPlayer;
                score(currentPlayer, 1);

                // retourne les cases ennemies capturées par ce mouvement
                foreach (Tuple<int, int> item in possibleMoves[tupleToString(column, line)])
                {
                    tiles[item.Item1, item.Item2] = currentPlayer;
                    // incrémente le score du joueur
                    score(currentPlayer, 1);
                    // décrémente le score de l'ennemi
                    score(1 - currentPlayer, -1);
                }

                ChangePlayer();

                // Calcul les coups possibles pour le prochain tour
                ComputePossibleMoves();
                return true;
            }
            Console.WriteLine($"can't make the move : {column}:{line}");
            return false;
        }


        /// <summary>
        /// Returns the number of white tiles on the board
        /// </summary>
        /// <returns></returns>
        public int getWhiteScore()
        {
            return WhitesScore;
        }

        /// <summary>
        /// Returns the number of black tiles
        /// </summary>
        /// <returns></returns>
        public int getBlackScore()
        {
            return BlacksScore;
        }
    }
}