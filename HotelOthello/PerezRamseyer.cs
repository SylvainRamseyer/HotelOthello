using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelOthello
{
    class PerezRamseyer
    {
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

        }
    }
}
