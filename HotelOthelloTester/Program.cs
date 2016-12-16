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
            Console.WriteLine(board);
            board.Tiles[2, 3].W();
            Console.WriteLine(board);
            board.Tiles[2, 3].B();
            Console.WriteLine(board);
            Console.ReadKey();  
        }
    }
}
