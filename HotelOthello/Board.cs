using OthelloConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelOthello
{
    public class Board
    {
        private int[,] tiles;
        private int currentPlayer = 1; // black
        private Dictionary<String,List<Tuple<int, int>>> possibleMoves;
        public enum Pawn { WHITE, BLACK};

        public int CurrentPlayer
        {
            get { return currentPlayer; }
        }
        
        public int[,] Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        public Board()
        {
            // intialisation
            tiles = new int[8, 8];
            possibleMoves = new Dictionary<String, List<Tuple<int, int>>>();

            // if wee need tiles to know their positions
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    tiles[i, j] = -1; // -1 : empty tile
                }
            }
            
            tiles[3, 4] = tiles[4, 3] = 1; // 1 : black
            tiles[3, 3] = tiles[4, 4] = 0; // 0 : white

            play();
            Console.ReadKey();
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
                possibleMoves.Clear();
                computePossibleMoves();

                // le joueur n'a pas de possibilité, il passe son tour
                if (possibleMoves.Count == 0)
                {
                    Console.WriteLine("you can't move");
                    // si on passe deux fois de suite ici, ça signifie qu'aucun joueur ne peut jouer, c'est fini
                    if (noMovement) gameover = true;
                    else noMovement = true;
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
            playMove(i, j);
        }

        // il faudrait que cette méthode retourne un dictionnaire avec par exemple
        // clés:tuiles possibles et valeurs:tuiles capturées par ce coup
        private void computePossibleMoves()
        {

            // adds all available tiles
            for (int column = 0; column < 8; column++)
            {
                for (int line = 0; line < 8; line++)
                {
                    computeMove(column, line);
                }
            }

        }

        private void computeMove(int column, int line)
        {
            if (tiles[column, line] != -1)
                return;

            // TODO regarder si les conditions fonctionne pour joué là


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
            for (int y = 0; y < 8; y++)
            {
                str.Append($"{y} ");
                for (int x = 0; x < 8; x++)
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


        /// <summary>
        /// Returns true if the move is valid for specified color
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <returns></returns>
        public bool isPlayable(int column, int line)
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
        public bool playMove(int column, int line)
        {
            if(isPlayable(column, line))
            {
                tiles[column, line] = currentPlayer;

                foreach (Tuple<int, int> item in possibleMoves[tupleToString(column,line)])
                {
                    tiles[item.Item1, item.Item2] = currentPlayer;
                }

                // on change de tour
                currentPlayer = 1 - currentPlayer;

                return true;
            }
            Console.WriteLine($"can't make the move : {column}:{line}");
            return false;
        }

    }
}