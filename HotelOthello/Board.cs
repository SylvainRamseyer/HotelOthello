using OthelloConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelOthello
{
    public class Board : IPlayable
    {
        private int[,] tiles;
        private int currentPlayer = 1; // black
        public enum Pawn { WHITE, BLACK};

        public int[,] Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        public Board()
        {
            // intialisation
            tiles = new int[8, 8];
            
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
                HashSet<string> possibleMoves = getPossibleMoves();

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
                        if (!possibleMoves.Contains(input))
                            Console.WriteLine($"{input} is not a legal move, choose something else");
                        else legalMove = true;
                    } while (!legalMove);

                    // make the move
                    makeMove(input);
                }

                // on change de tour
                currentPlayer = 1 - currentPlayer;

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
            bool isWhite = currentPlayer == 1 ? true : false;
            playMove(i, j, isWhite);
        }

        // il faudrait que cette méthode retourne un dictionnaire avec par exemple
        // clés:tuiles possibles et valeurs:tuiles capturées par ce coup
        private HashSet<string> getPossibleMoves()
        {
            HashSet<string> moves = new HashSet<string>();

            // adds all available tiles
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Tiles[i, j] == -1)
                        moves.Add($"{i}{j}");
                }
            }

            return moves;
        }

        //// provisoire : affecte le player à la tuile en fonction du paramètre tileCoords représentant la tuile sous forme de string "yx"
        //private void setTile(string tileCoords, string player)
        //{
        //    char[] ij = tileCoords.ToCharArray();
        //    int i = ij[0] - '0';
        //    int j = ij[1] - '0';
        //    tiles[i, j].set(player);
        //}

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("  A B C D E F G H\n");
            for (int y = 0; y < 8; y++)
            {
                str.Append($"{y+1} ");
                for (int x = 0; x < 8; x++)
                {
                    string tile = tiles[x, y] == -1 ? "_" : (tiles[x,y] == 0 ? "w": "b");
                    str.Append($"{tile} ");
                }
                str.Append("\n");
            }
            return str.ToString();
        }


        /* 
         * 
         *  IPlayble implementation
         *
         */

        /// <summary>
        /// Returns true if the move is valid for specified color
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public bool isPlayable(int column, int line, bool isWhite)
        {
            if (tiles[column, line] != -1)
                return false;

            // TODO regarder si les conditions fonctionne pour joué là


            int colorToCheck = isWhite ? 0 : 1;
            List<Tuple<int, int>> voisins = new List<Tuple<int, int>>();



            for (int i = column-1; i <= column + 1; i++)
            {
                for (int j = line-1; j <= line+1; j++)
                {
                    try
                    {
                        if (Tiles[i, j] == colorToCheck)
                            voisins.Add(new Tuple<int, int>(i, j));
                    }
                    catch (Exception)
                    {
                        // si la case n'existe pas on ne fait rien
                    }
                }
            }

            if (voisins.Count == 0)
                return false;

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
                    while (tiles[x, y] == colorToCheck)
                    {
                        temp.Add(new Tuple<int, int>(x, y));
                        x = x + dx;
                        y = y + dy;
                    }

                    if (tiles[x, y] == 1 - colorToCheck)
                        catchedTiles.AddRange(temp);
                }
                catch (Exception)
                {
                    // si hors de la grille de jeux --> n'est pas valide
                }             
            }
            if (catchedTiles.Count == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Will update the board status if the move is valid and return true
        /// Will return false otherwise (board is unchanged)
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite">true for white move, false for black move</param>
        /// <returns></returns>
        public bool playMove(int column, int line, bool isWhite)
        {
            if(isPlayable(column, line, isWhite))
            {
                tiles[column, line] = isWhite ? 0 : 1;
                return true;
            }
            Console.WriteLine($"can't make the move : {column}:{line}");
            return false;

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
        /// Returns the number of white tiles on the board
        /// </summary>
        /// <returns></returns>
        public int getWhiteScore()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the number of black tiles
        /// </summary>
        /// <returns></returns>
        public int getBlackScore()
        {
            throw new NotImplementedException();
        }
    }
}