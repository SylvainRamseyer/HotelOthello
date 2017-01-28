using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Timers;

namespace HotelOthello
{
    public class OthelloGame : INotifyPropertyChanged
    {
        // premet le databinding
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
        // valeur: liste des tuiles capturées si ce mouvement est efefctuées
        private Dictionary<String,List<Tuple<int, int>>> possibleMoves;
        public bool CanMove { get { return possibleMoves.Count > 0; } }

        private int currentPlayer = 1; // 1=black
        public int CurrentPlayer{ get { return currentPlayer; } }

        // raccourci pour obtenir la couleur qui doit jouer
        public string PlayerColor
        {
            get
            {
                return CurrentPlayer == 1 ? "Black" : "White";
            }
        }

        private int[] scores = { 2, 2 };
        
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


        TimeSpan[] playersTimes = { TimeSpan.Zero, TimeSpan.Zero };
        DateTime[] referenceTimes = { DateTime.Now, DateTime.Now };

        string whitesTimeString = "";
        string blacksTimeString = "";
        private Timer timer;

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

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public OthelloGame()
        {
            // intialisation
            tiles = new int[SIZE_GRID, SIZE_GRID];
            history = new Stack<int[,]>();
            possibleMoves = new Dictionary<String, List<Tuple<int, int>>>();

            // if wee need tiles to know their positions
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

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            computeTime(currentPlayer);
            if (currentPlayer == 0)
                WhitesTimeString = timeSpanToString(playersTimes[0]);
            else if (currentPlayer == 1)
                BlacksTimeString = timeSpanToString(playersTimes[1]);
        }

        private void computeTime(int player)
        {
            playersTimes[player] += DateTime.Now - referenceTimes[player];
            referenceTimes[player] = DateTime.Now;
        }

        private static string timeSpanToString(TimeSpan t)
        {
            return String.Format("{0:D2}:{1:D2}.{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        public void RestartTimer()
        {
            referenceTimes[currentPlayer] = DateTime.Now;
            timer.Start();
        }

        public void ComputePossibleMoves()
        {
            possibleMoves.Clear();

            // adds all available tiles
            for (int column = 0; column < SIZE_GRID; column++)
            {
                for (int line = 0; line < SIZE_GRID; line++)
                {
                    computeMove(column, line);
                }
            }
        }

        private void computeMove(int column, int line)
        {
            if (tiles[column, line] != -1)
                return;

            int ennemi = 1-currentPlayer;
            List<Tuple<int, int>> voisins = new List<Tuple<int, int>>();

            for (int i = column - 1; i <= column + 1; i++)
            {
                for (int j = line - 1; j <= line + 1; j++)
                {
                    try
                    {
                        if (Tiles[i, j] == ennemi)
                            voisins.Add(new Tuple<int, int>(i, j));
                    }
                    catch (Exception)
                    {
                        // si la case n'existe pas on ne fait rien
                    }
                }
            }

            if (voisins.Count == 0)
                return;

            List<Tuple<int, int>> catchedTiles = new List<Tuple<int, int>>();

            foreach (Tuple<int, int> voisin in voisins)
            {
                int dx = voisin.Item1 - column;
                int dy = voisin.Item2 - line;
                int x = voisin.Item1;
                int y = voisin.Item2;
                List<Tuple<int, int>> temp = new List<Tuple<int, int>>();
                try
                {
                    while (tiles[x, y] == ennemi)
                    {
                        temp.Add(new Tuple<int, int>(x, y));
                        x = x + dx;
                        y = y + dy;
                    }

                    if (tiles[x, y] == currentPlayer)
                        catchedTiles.AddRange(temp);
                }
                catch (Exception)
                {
                    // si hors de la grille de jeux --> n'est pas valide
                }
            }
            if (catchedTiles.Count == 0)
                return;

            possibleMoves.Add(tupleToString(column, line), catchedTiles);

            return;
        }

        private String tupleToString(Tuple<int, int> tuple)
        {
            return tupleToString(tuple.Item1, tuple.Item2);
        }

        private String tupleToString(int x, int y)
        {
            return $"{x}{y}";
        }


        public bool Save(String fileName)
        {

            SavebleBoard board = new SavebleBoard();
            board.CurrentPlayer = this.CurrentPlayer;
            board.Tiles = this.tiles;
            board.BlackTimer = this.playersTimes[1];
            board.WhiteTimer = this.playersTimes[0];

            string json = JsonConvert.SerializeObject(board);
            System.IO.File.WriteAllText(fileName, json);
            return true;
        }

        public bool load(String fileName)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                SavebleBoard board = JsonConvert.DeserializeObject<SavebleBoard>(json);

                Console.WriteLine(board.CurrentPlayer);
                this.tiles = board.Tiles;
                this.playersTimes[1] = board.BlackTimer;
                this.playersTimes[0] = board.WhiteTimer;
                this.currentPlayer = board.CurrentPlayer;
                this.ComputePossibleMoves();
                this.updateScore();
            }
            return true;
        }

        /// <summary>
        /// Returns true if the move is valid for specified color
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <returns></returns>
        public bool IsPlayable(int column, int line)
        {
            return possibleMoves.ContainsKey(tupleToString(column, line));
        }

        private void score(int player, int delta)
        {
            if (player == 1)
                BlacksScore += delta;
            else if (player == 0)
                WhitesScore += delta;
        }

        /// <summary>
        /// Will update the board status if the move is valid and return true
        /// Will return false otherwise (board is unchanged)
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <returns></returns>
        public bool PlayMove(int column, int line)
        {
            if(IsPlayable(column, line))
            {
                SaveInHistory();

                // pose une pièce sur la case jouée
                tiles[column, line] = currentPlayer;
                //scores[currentPlayer]++;
                score(currentPlayer, 1);

                // retourne les cases capturées par ce mouvement
                foreach (Tuple<int, int> item in possibleMoves[tupleToString(column,line)])
                {
                    tiles[item.Item1, item.Item2] = currentPlayer;
                    //scores[currentPlayer]++;
                    //scores[1-currentPlayer]--;
                    score(currentPlayer, 1);
                    score(1-currentPlayer, -1);
                }

                ChangePlayer();

                // calcul les coups possibles pour le prochain tour
                ComputePossibleMoves();
                return true;
            }
            Console.WriteLine($"can't make the move : {column}:{line}");
            return false;
        }

        public void ChangePlayer()
        {
            currentPlayer = 1 - currentPlayer;
            referenceTimes[currentPlayer] = DateTime.Now;
        }

        public int GetScore(bool IsForWhite)
        {
            //return scores[IsForWhite ? 0 : 1];
            if (IsForWhite)
                return WhitesScore;
            return BlacksScore;
        }

        private void updateScore()
        {
            //scores[0] = scores[1] = 0;
            WhitesScore = BlacksScore = 0;

            for (int y = 0; y < SIZE_GRID; y++)
            {
                for (int x = 0; x < SIZE_GRID; x++)
                {
                    if (tiles[x, y] != -1)
                        //scores[tiles[x, y]]++;
                        score(tiles[x, y], 1);
                }
            }
        }
        
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

        public void Undo()
        {
            if (history.Count != 0)
            {
                tiles = history.Pop();
                currentPlayer = 1 - currentPlayer;
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

    }
}