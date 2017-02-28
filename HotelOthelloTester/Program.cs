using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelOthello;

using OthelloIA9;
using OthelloIA2;

namespace HotelOthelloTester
{
    class Program
    {
        static void Main(string[] args)
        {
            // new OthelloText();
            OthelloBoard[] IAs = { new OthelloBoard(), new OthelloBoard() };
        
            int[,] tiles = new int[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    tiles[i, j] = -1; // -1 : empty tile
                }
            }

            tiles[3, 4] = tiles[4, 3] = 1; // 1 : black
            tiles[3, 3] = tiles[4, 4] = 0; // 0 : white

            bool whitesTurn = false;
            int passCount = 0;
            while (passCount < 2)
            {
                debug(tiles);
                Console.ReadKey();

                var move = IAs[whitesTurn ? 1 : 0].GetNextMove(tiles, whitesTurn ? 0 : 5, whitesTurn);
                if(move.Item1 == -1 && move.Item2 == -1)
                {
                    passCount++;
                }
                else
                {
                    IAs[0].PlayMove(move.Item1, move.Item2, whitesTurn);
                    IAs[1].PlayMove(move.Item1, move.Item2, whitesTurn);
                    passCount = 0;
                }
                tiles = IAs[0].GetBoard();
                whitesTurn = !whitesTurn;

            }

            string winner = IAs[0].GetWhiteScore() > IAs[0].GetBlackScore() ? "white wins" : "black wins";
            Console.WriteLine($"finished, {winner}");
            Console.ReadKey();

        }


        /**
         *  TOOLS
         */

        static void debug(int[,] tiles)
        {
            StringBuilder str = new StringBuilder();
            str.Append("  0 1 2 3 4 5 6 7\n");
            for (int y = 0; y < 8; y++)
            {
                str.Append($"{y} ");
                for (int x = 0; x < 8; x++)
                {
                    string tile = tiles[x, y] == -1 ? "_" : (tiles[x, y] == 0 ? "w" : "b");
                    str.Append($"{tile} ");
                }
                str.Append("\n");
            }
            Console.WriteLine(str.ToString());
        }
    }
}
