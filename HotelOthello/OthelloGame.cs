using OthelloConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelOthello
{
    public class OthelloGame
    {
        public const int SIZE = 8;

        private int[,] tiles;
        private int currentPlayer = 1; // black
        private Dictionary<String,List<Tuple<int, int>>> possibleMoves;

        bool noMovement = false;
        bool gameover = false;

        public bool GameOver { get { return gameover; } }
        public bool NoMovement { get { return noMovement; } }
        public Dictionary<String, List<Tuple<int, int>>> PossibleMoves { get { return possibleMoves; } }

        public int CurrentPlayer{ get { return currentPlayer; } }
        public string PlayerColor
        {
            get
            {
                return CurrentPlayer == 1 ? "Black" : "White";
            }
        }
        
        public int[,] Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        public OthelloGame()
        {
            // intialisation
            tiles = new int[SIZE, SIZE];
            possibleMoves = new Dictionary<String, List<Tuple<int, int>>>();

            // if wee need tiles to know their positions
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    tiles[i, j] = -1; // -1 : empty tile
                }
            }

            tiles[3, 4] = tiles[4, 3] = 1; // 1 : black
            tiles[3, 3] = tiles[4, 4] = 0; // 0 : white
            
            ComputePossibleMoves();
        }

        private string play()
        {
            bool gameover = false;
            bool noMovement = false;
            string input;
            do
            {
                string color = currentPlayer == 1 ? "black" : "white";
                Console.WriteLine($"{color} to play");

                ComputePossibleMoves();

                // le joueur n'a pas de possibilité, il passe son tour
                if (possibleMoves.Count == 0)
                {
                    Console.WriteLine("you can't move");
                    // si on passe deux fois de suite ici, ça signifie qu'aucun joueur ne peut jouer, c'est fini
                    if (noMovement)
                        gameover = true;
                    else
                        noMovement = true;
                }
                else
                {
                    noMovement = false;

                    Console.WriteLine(this);
                    Console.WriteLine("To make a move, type x and y coordinates. For example : 70 for the top right tile");
                    bool legalMove = false;
                    do
                    {
                        input = Console.ReadLine();
                        if (!possibleMoves.ContainsKey(input))
                            Console.WriteLine($"{input} is not a legal move, choose something else");
                        else legalMove = true;
                    } while (!legalMove);

                    // make the move
                    makeMove(input);
                }

                Console.WriteLine("**************************************************");

            } while (!gameover);

            // compter les pions pour donner un vainqueur
            return "black";
        }

        private void makeMove(string input)
        {
            // en plus d'ajouter le pion, il faudra inverser les pions capturés
            // on récupère les tuiles capturées dans getPossibleMoves
            char[] ij = input.ToCharArray();
            int i = ij[0] - '0';
            int j = ij[1] - '0';
            //tiles[i, j] = currentPlayer;
            PlayMove(i, j);
        }

        // il faudrait que cette méthode retourne un dictionnaire avec par exemple
        // clés:tuiles possibles et valeurs:tuiles capturées par ce coup
        public void ComputePossibleMoves()
        {
            possibleMoves.Clear();

            // adds all available tiles
            for (int column = 0; column < SIZE; column++)
            {
                for (int line = 0; line < SIZE; line++)
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

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("  0 1 2 3 4 5 6 7\n"); 
            for (int y = 0; y < SIZE; y++)
            {
                str.Append($"{y} ");
                for (int x = 0; x < SIZE; x++)
                {
                    string tile = tiles[x, y] == -1 ? "_" : (tiles[x,y] == 0 ? "w": "b");
                    if (possibleMoves.ContainsKey(tupleToString(x, y)))
                        tile = ".";
                    str.Append($"{tile} ");
                }
                str.Append("\n");
            }
            return str.ToString();
        }


        private String tupleToString(Tuple<int, int> tuple)
        {
            return tuple.Item1.ToString() + tuple.Item2.ToString();
        }

        private String tupleToString(int x, int y)
        {
            return x.ToString() + y.ToString();
        }

        private Tuple<int, int> stringToTuple(string str)
        {
            char[] ij = str.ToCharArray();
            int i = ij[0] - '0';
            int j = ij[1] - '0';

            return new Tuple<int, int>(i, j);
        }


        public bool Save(String fileName)
        {
            // TODO save current player and board
            return false;
        }

        public bool load(String fileName)
        {
            // TODO load current player and board and dont forget to computePossibleMoves
            return false;
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
                // pose une pièce sur la case jouée
                tiles[column, line] = currentPlayer;

                // retourne les cases capturées par ce mouvement
                foreach (Tuple<int, int> item in possibleMoves[tupleToString(column,line)])
                {
                    tiles[item.Item1, item.Item2] = currentPlayer;
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
        }

        public int GetScore(bool IsForWhite)
        {
            int cpt = 0;
            int color = IsForWhite ? 0 : 1;

            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    if (tiles[x, y] == color)
                        cpt++;
                }
            }
            return cpt;
        }

    }
}