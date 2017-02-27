using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelOthello
{
    class PerezRamseyer
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
        Tuple<char, int> getNextMove(int[,] game, int level, bool whiteTurn)
        {
            return null;
        }


        /*def alphabeta2(root , depth , minOrMax , parentValue) :
        # minOrMax = 1 : maximize
        # minOrMax = 􀀀1 : minimize
            if(depth == 0 or root . final () ) :
            return root.eval() , None
            optVal = minOrMax  􀀀inf
            optOp = None
            for op in root.ops() :
            new = root.apply(op)
            val, dummy = alphabeta2(new , depth􀀀1,􀀀  - minOrMax, optVal)
            if val  minOrMax > optVal  minOrMax :
                optVal , optOp = val , op
                if optVal  minOrMax > parentValue - minOrMax :
                    break
            return optVal , optOp */

    }
}
