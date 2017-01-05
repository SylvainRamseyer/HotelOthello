using System;
using System.Collections.Generic;
using System.Text;

namespace HotelOthello
{
    public class Board
    {
        private Tile[,] tiles;
        private string currentPlayer = "black";

        public Tile[,] Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        public Board()
        {
            // intialisation
            tiles = new Tile[8, 8];
            /*
            // if wee need tiles to know their positions
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    tiles[i, j] = new Tile(i, j);
                }
            }
            */
            tiles[3, 4].Black = tiles[4, 3].Black = true;
            tiles[3, 3].White = tiles[4, 4].White = true;

            play();
        }

        private string play()
        {
            bool gameover = false;
            bool noMovement = false;
            string input;
            do
            {
                Console.WriteLine($"{currentPlayer} to play");
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
                    Console.WriteLine("To make a move, type y and x coordinates. For example : 07 for the top right tile");
                    bool legalMove = false;
                    do
                    {
                        input = Console.ReadLine();
                        if (!possibleMoves.Contains(input))
                            Console.WriteLine($"{input} is not a legal move, choose somthing else");
                        else legalMove = true;
                    } while (!legalMove);

                    // make the move
                    makeMove(input);
                }

                // on change de tour
                currentPlayer = (currentPlayer == "black") ? "white" : "black";

                Console.WriteLine("**************************************************");

            } while (!gameover);

            // compter les pions pour donner un vainqueur
            return "black";
        }

        private void makeMove(string input)
        {
            // en plus d'ajouter le pion, il faudra inverser les pions capturés
            // on récupère les tuiles capturées dans getPossibleMoves
            setTile(input, currentPlayer);
        }

        // il faudrait que cette méthode retourne un dictionnaire avec par exemple
        // clés:tuiles possibles et valeurs:tuiles capturées par ce coup
        // il faudrait donc que les tuiles contiennent contiennent leurs coordonéées
        // ou alors faire une nouvelle classe Move ?
        // pour l'instant ça retourne toutes les cases libres sous formes de set de string "yx"
        // pas idéal, c'était pour simplifier les choses en ligne de commande
        private HashSet<string> getPossibleMoves()
        {
            HashSet<string> moves = new HashSet<string>();

            // adds all available tiles
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!Tiles[i, j].IsTaken)
                        moves.Add($"{i}{j}");
                }
            }

            return moves;
        }

        // provisoire : affecte le player à la tuile en fonction du paramètre tileCoords représentant la tuile sous forme de string "yx"
        private void setTile(string tileCoords, string player)
        {
            char[] ij = tileCoords.ToCharArray();
            int i = ij[0] - '0';
            int j = ij[1] - '0';
            tiles[i, j].set(player);
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("  0 1 2 3 4 5 6 7\n");
            for (int i = 0; i < 8; i++)
            {
                str.Append($"{i} ");
                for (int j = 0; j < 8; j++)
                {
                    str.Append($"{tiles[i, j]} ");
                }
                str.Append("\n");
            }
            return str.ToString();
        }

    }
}