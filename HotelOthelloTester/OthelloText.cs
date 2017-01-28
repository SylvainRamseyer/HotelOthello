using HotelOthello;
using System;

namespace HotelOthelloTester
{
    internal class OthelloText
    {
        OthelloGame game;

        public OthelloText()
        {
            game = new OthelloGame();
            play();
            Console.WriteLine("Game over");
            Console.ReadKey();
        }


        private string play()
        {
            bool gameover = false;
            bool noMovement = false;
            string input;
            do
            {
                Console.WriteLine($"{game.PlayerColor}s turn");

                // le joueur n'a pas de possibilité, il passe son tour
                if (!game.CanMove)
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

                    Console.WriteLine(game);
                    Console.WriteLine("To make a move, type x and y coordinates. For example : 70 for the top right tile");
                    do
                    {
                        input = Console.ReadLine();
                    }
                    // try to make the move, retry if input is not a valid move
                    while (!makeMove(input));
                }

                Console.WriteLine("**************************************************");

            } while (!gameover);

            // compter les pions pour donner un vainqueur
            return "black";
        }

        private bool makeMove(string input)
        {
            char[] ij = input.ToCharArray();
            int i = ij[0] - '0';
            int j = ij[1] - '0';
            //tiles[i, j] = currentPlayer;
            return game.PlayMove(i, j);
        }

    }
}