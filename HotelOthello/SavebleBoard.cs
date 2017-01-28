using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelOthello
{
    class SavebleBoard
    {
        public int CurrentPlayer { get; set; }
        public Decimal BlackTimer { get; set; }
        public Decimal WhiteTimer { get; set; }
        public int[,] Tiles { get; set; }
    }
}
