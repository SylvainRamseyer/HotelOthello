using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPlayable;

namespace OthelloIA9
{
    class OthelloBoard : IPlayable.IPlayable
    {

        /*
         * matrice d'indication des poids de chaque cases
        500  -150 30 10 10 30 -150 500
        -150 -250 0  0  0  0  -250 -150
        30   0    1  2  2  1  0    30
        10   0    2  16 16 2  0    10
        10   0    2  16 16 2  0    10
        30   0    1  2  2  1  0    30
        -150 -250 0  0  0  0  -250 -150
        500  -150 30 10 10 30 -150 500
        */
        public static readonly int[,] WEIGHT_MATRIX = {
            {500, -150, 30, 10, 10, 30, -150, 500},
            {-150, -250, 0, 0, 0, 0, -250, -150},
            {30, 0, 1, 2, 2, 1, 0, 30},
            {10, 0, 2, 16, 16, 2, 0, 10},
            {10, 0, 2, 16, 16, 2, 0, 10},
            {30, 0, 1, 2, 2, 1, 0, 30},
            {-150, -250, 0, 0, 0, 0, -250, -150},
            {500, -150, 30, 10, 10, 30, -150, 500}
        };

        private int[] scores = { 2, 2 };

        // Ces deux propriétés Lancent l'évènement PropertyChanged qui actualise l'affichage
        public int BlacksScore
        {
            get { return scores[1]; }
            set { scores[1] = value;}
        }

        public int WhitesScore
        {
            get { return scores[0]; }
            set { scores[0] = value; }
        }

        // tableau 2d d'entiers représentant le plateau de jeu
        // -1 : case libre
        //  0 : blanc
        //  1 : noir
        private int[,] tiles;

        public const int SIZE_GRID = 8;

        public OthelloBoard()
        {
            // Intialisation
            tiles = new int[SIZE_GRID, SIZE_GRID];

            for (int i = 0; i < SIZE_GRID; i++)
            {
                for (int j = 0; j < SIZE_GRID; j++)
                {
                    tiles[i, j] = -1; // -1 : empty tile
                }
            }

            tiles[3, 4] = tiles[4, 3] = 1; // 1 : black
            tiles[3, 3] = tiles[4, 4] = 0; // 0 : white
        }

        public string GetName()
        {
            return "IA9 PerezRamseyer";
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            tiles[column, line] = isWhite ? 0 : 1 ;
            return true;
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public int[,] GetBoard()
        {
            return tiles;
        }

        public int GetWhiteScore()
        {
            return WhitesScore;
        }

        public int GetBlackScore()
        {
            return BlacksScore;
        }

        private void score(int player, int delta)
        {
            if (player == 1)
                BlacksScore += delta;
            else if (player == 0)
                WhitesScore += delta;
        }
    }
}
