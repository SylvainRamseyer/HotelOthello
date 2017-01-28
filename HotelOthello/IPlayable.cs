using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloConsole
{
    interface IPlayable
    {
        /// <summary>
        /// Returns true if the move is valid for specified color
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        bool isPlayable(int column, int line, bool isWhite);

        /// <summary>
        /// Will update the board status if the move is valid and return true
        /// Will return false otherwise (board is unchanged)
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite">true for white move, false for black move</param>
        /// <returns></returns>
        bool playMove(int column, int line, bool isWhite);

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
        Tuple<char, int> getNextMove(int[,] game, int level, bool whiteTurn);

        /// <summary>
        /// Returns the number of white tiles on the board
        /// </summary>
        /// <returns></returns>
        int getWhiteScore();

        /// <summary>
        /// Returns the number of black tiles
        /// </summary>
        /// <returns></returns>
        int getBlackScore();

    }
}
