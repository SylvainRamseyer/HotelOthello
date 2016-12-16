using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelOthello;

namespace HotelOthelloTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board();
            Console.WriteLine(board.ToString());
            board.Tiles[2, 5].IsTaken = true;
            Console.WriteLine(board.ToString());
            Console.ReadKey();  
        }
    }
}
